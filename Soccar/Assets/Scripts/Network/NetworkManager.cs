using System;
using UnityEngine;
using socket.io;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    /* 서버 접속에 관한 요소 */
    //private const string Url = "http://10.21.20.20:9090";
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    private const string Ip = "http://15.164.220.253:";
#elif UNITY_WEBGL
    private const string Ip = "http://15.164.220.253:";
#endif
    private Socket _socket;

    /* 동기화를 위한 요소 */
    private long _rtt;

    /* 서버로부터의 Ack 확인 */
    public string GameStart { get; private set; }
    public string RequestPlayerIndex { get; set; }

    private RoomManager _roomManager;
    private Room _room;
    private GameObject _roomPanel;

    // 경고 창
    private GameObject _alertPanel;
    private Text _alertMessage;

    // 씬 전환을 위한 객체
    [SerializeField]
    private GameObject SceneMediumObject;
    private SceneMedium _sceneMedium;

    // Start 함수에서 호출되어야 함
    public void SetWebSocket(bool isGameScene, int port, LobbyNetworkLinker lobbyNetworkLinker = null)
    {
        string url = Ip + port;
        _socket = Socket.Connect(url);
        _sceneMedium = SceneMediumObject.GetComponent<SceneMedium>();

        // 게임
        if(isGameScene)
        {
            GameStart = "";
            RequestPlayerIndex = "";

            /* 서버로부터 메시지 수신 */
            _socket.On("start_button", (string data) =>
            {
                GameStart = data.Substring(1, 5);
                Debug.Log("==Game Start Message: " + GameStart);
            });

            _socket.On("request_player_index", (string data) =>
            {
                PlayerController.PlayerIndex = int.Parse(data.Substring(1, data.Length - 2));
                Debug.Log("==Received Player Index: " + PlayerController.PlayerIndex);
            });

            // 상대좌표 + 공
            _socket.On("relative_position", (string data) =>
            {
                //Debug.Log("FRAME " + GameLauncher.Frame + " 상대좌표");
                //long timestamp = GetTimestamp();

                data = ToJsonFormat(data);

                // 캐릭터 이동
                Packet.ReceivingPositions receivingPositions = JsonUtility.FromJson<Packet.ReceivingPositions>(data);
                PlayerController.Move(receivingPositions.PlayerPositions, PlayerController.Relative);

                if(PlayerController.PlayerIndex != 0)
                {
                    // 공 이동
                    for(int i = 0; i < 2; i++)
                    {
                        GameLauncher.Balls[i].transform.position = receivingPositions.BallPositions[i];
                    }
                }
            });

            // 절대 좌표 + 공
            _socket.On("absolute_position", (string data) =>
            {
                data = ToJsonFormat(data);

                // 캐릭터 및 공 이동
                Packet.ReceivingPositions receivingPositions = JsonUtility.FromJson<Packet.ReceivingPositions>(data);
                GameLauncher.RoutineScheduler.StopMoving();
                GameLauncher.RoutineScheduler.StartMoving(receivingPositions);
                //PlayerController.Move(receivingPositions.PlayerPositions, PlayerController.Absolute);
            });
        }

        // 로비
        else
        {
            _roomManager = lobbyNetworkLinker.RoomManager;
            _room = lobbyNetworkLinker.Room;
            _roomPanel = lobbyNetworkLinker.RoomPanel;

            _alertPanel = lobbyNetworkLinker.AlertPanel;
            _alertMessage = _alertPanel.transform.Find("Message").GetComponent<Text>();

            /* 서버로부터 메시지 수신 */
            // 로그인 시 소켓 아이디를 받음
            _socket.On("login", (string data) =>
            {
                Debug.Log("Login " + data);
                LobbyManager.SocketId = data.Substring(1, data.Length - 2);
                Debug.Log("Login Socket ID: " + LobbyManager.SocketId);
            });

            // 로그인 시 첫 화면, 새로고침 버튼 클릭 시
            _socket.On("room_list", (string data) =>
            {
                data = ToJsonFormat(data);

                // 로비에서 보이는 방 리스트를 업데이트
                Packet.ReceivingRoomList receivingRoomList = JsonUtility.FromJson<Packet.ReceivingRoomList>(data);
                _roomManager.SetRoomList(receivingRoomList);

                LobbyManager.CurrentPanel = LobbyManager.OnLobbyPanel;
            });

            // 방 생성, 방 입장, 대기 중인 방에 다른 플레이어가 들어올 때
            _socket.On("room_info", (string data) =>
            {
                data = ToJsonFormat(data);

                // 방 안에서 보이는 방 정보를 업데이트
                Packet.ReceivingRoomInfo receivingRoomInfo = JsonUtility.FromJson<Packet.ReceivingRoomInfo>(data);
                _room.SetRoomInfo(receivingRoomInfo);

                LobbyManager.CurrentPanel = LobbyManager.OnRoomPanel;
            });

            // 방 입장 실패(꽉찬 방, 게임이 시작한 방, 사라진 방)
            _socket.On("fail_enter_room", (string data) =>
            {
                _roomPanel.GetComponent<Animator>().Play("Exit Room");
                _alertMessage.text = "You cannot enter this room because it room is full or game is already started or does not exist now.";
                _alertPanel.GetComponent<Animator>().Play("Open Alert");
            });

            // 채팅 수신
            _socket.On("chat", (string data) =>
            {
                data = ToJsonFormat(data);

                Packet.ReceivingChat receivingChat = JsonUtility.FromJson<Packet.ReceivingChat>(data);
                _room.ShowSpeechBubble(receivingChat);
            });

            // 게임 시작, 게임씬 전환
            _socket.On("start_game", (string data) =>
            {
                data = ToJsonFormat(data);

                Packet.ReceivingGameStart receivingGameStart = JsonUtility.FromJson<Packet.ReceivingGameStart>(data);
                _sceneMedium.Port = receivingGameStart.Port;
                _sceneMedium.Headcount = receivingGameStart.Headcount;

                SceneManager.LoadScene("GoalTestScene");
            });

            // 게임 시작 실패(인원수가 적음)
            _socket.On("fail_start_game", (string data) =>
            {
                _alertMessage.text = "You cannot start game. It requires 4 ~ 6 players.";
                _alertPanel.GetComponent<Animator>().Play("Open Alert");
            });
        }  
    }

    public void Send(string header, string message)
    {
        _socket.Emit(header, message);
    }

    // 구조체 전송
    public void Send<T>(string header, object body)
    {
        T packetBody = (T)body;
        // 현재 시스템 시간 전송
        //packetBody.Timestamp = GetTimestamp();
        string json = JsonUtility.ToJson(packetBody);
        //Debug.Log("json " + json);
        _socket.EmitJson(header, json);
    }

    public long GetTimestamp()
    {
        TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
        return (long)(timeSpan.TotalSeconds * 1000);
    }

    private string ToJsonFormat(string str)
    {
        string data = str.Replace("\\", "");
        return data.Substring(1, data.Length - 2);
    }
}
