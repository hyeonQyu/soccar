using System;
using UnityEngine;
using socket.io;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class NetworkManager : MonoBehaviour
{
    /* 서버 접속에 관한 요소 */
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    private const string Ip = "http://15.164.59.198:";
#elif UNITY_WEBGL
    private const string Ip = "http://15.164.59.198:";
#endif
    public Socket Socket { get; private set; }

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

    // 플레이어들의 점수
    private Text[] _scores;

    // 로비
    public void SetWebSocket(SceneMedium sceneMedium, LobbyNetworkLinker lobbyNetworkLinker = null)
    {
        string url = Ip + sceneMedium.Port;
        Socket = Socket.Connect(url);

        _roomManager = lobbyNetworkLinker.RoomManager;
        _room = lobbyNetworkLinker.Room;
        _roomPanel = lobbyNetworkLinker.RoomPanel;

        _alertPanel = lobbyNetworkLinker.AlertPanel;
        _alertMessage = _alertPanel.transform.Find("Message").GetComponent<Text>();

        /* 서버로부터 메시지 수신 */
        // 로그인 시 소켓 아이디를 받음
        Socket.On("login", (string data) =>
        {
            Debug.Log("Login " + data);
            LobbyManager.SocketId = data.Substring(1, data.Length - 2);
            Debug.Log("Login Socket ID: " + LobbyManager.SocketId);
        });

        // 로그인 시 첫 화면, 새로고침 버튼 클릭 시
        Socket.On("room_list", (string data) =>
        {
            data = ToJsonFormat(data);

            // 로비에서 보이는 방 리스트를 업데이트
            Packet.ReceivingRoomList receivingRoomList = JsonUtility.FromJson<Packet.ReceivingRoomList>(data);
            _roomManager.SetRoomList(receivingRoomList);

            LobbyManager.CurrentPanel = LobbyManager.OnLobbyPanel;
        });

        // 방 생성, 방 입장, 대기 중인 방에 다른 플레이어가 들어올 때
        Socket.On("room_info", (string data) =>
        {
            data = ToJsonFormat(data);

            // 방 안에서 보이는 방 정보를 업데이트
            Packet.ReceivingRoomInfo receivingRoomInfo = JsonUtility.FromJson<Packet.ReceivingRoomInfo>(data);
            _room.SetRoomInfo(receivingRoomInfo);

            LobbyManager.CurrentPanel = LobbyManager.OnRoomPanel;
        });

        // 방 입장 실패(꽉찬 방, 게임이 시작한 방, 사라진 방)
        Socket.On("fail_enter_room", (string data) =>
        {
            _roomPanel.GetComponent<Animator>().Play("Exit Room");
            _alertMessage.text = "You cannot enter this room because it room is full or game is already started or does not exist now.";
            _alertPanel.GetComponent<Animator>().Play("Open Alert");
        });

        // 채팅 수신
        Socket.On("chat", (string data) =>
        {
            data = ToJsonFormat(data);

            Packet.ReceivingChat receivingChat = JsonUtility.FromJson<Packet.ReceivingChat>(data);
            _room.ShowSpeechBubble(receivingChat);
        });

        // 게임 시작, 게임씬 전환
        Socket.On("start_game", (string data) =>
        {
            data = ToJsonFormat(data);

            Packet.ReceivingGameStart receivingGameStart = JsonUtility.FromJson<Packet.ReceivingGameStart>(data);
            sceneMedium.Port = receivingGameStart.Port;
            sceneMedium.Headcount = receivingGameStart.Headcount;
            sceneMedium.PlayerIndex = _room.GetPlayerIndex();
            sceneMedium.PlayerName = LobbyManager.PlayerName;

            // 로비 서버와 연결 해제
            Send("disconnection", "");

            // 웹소켓 관련 요소 삭제
            Destroy(GameObject.Find("(singleton) socket.io.SocketManager"));
            Destroy(GameObject.Find("MainThreadDispatcher"));

            SceneManager.LoadScene("Game Scene");
        });

        // 게임 시작 실패(인원수가 적음)
        Socket.On("fail_start_game", (string data) =>
        {
            _alertMessage.text = "You cannot start game. It requires 4 ~ 6 players.";
            _alertPanel.GetComponent<Animator>().Play("Open Alert");
        });
    }

    // 게임 씬
    public void SetWebSocket(SceneMedium sceneMedium, GameObject[] scoreBoard)
    {
        string url = Ip + sceneMedium.Port;
        Socket = Socket.Reconnect(url);

        GameStart = "";
        RequestPlayerIndex = "";

        _scores = new Text[scoreBoard.Length];
        for(int i = 0; i < scoreBoard.Length; i++)
        {
            _scores[i] = scoreBoard[i].transform.Find("Score").GetComponent<Text>();
        }

        /* 서버로부터 메시지 수신 */
        Socket.On("player_index", (string data) =>
        {
            PlayerController.PlayerIndex = int.Parse(data.Substring(1, data.Length - 2));
        });

        // 절대 좌표 + 공
        Socket.On("absolute_position", (string data) =>
        {
            data = ToJsonFormat(data);

            // 캐릭터 및 공 이동
            Packet.ReceivingPositions receivingPositions = JsonUtility.FromJson<Packet.ReceivingPositions>(data);
            GameLauncher.RoutineScheduler.StopMoving();
            GameLauncher.RoutineScheduler.StartMoving(receivingPositions);
            //PlayerController.Move(receivingPositions.PlayerPositions, PlayerController.Absolute);
        });

        Socket.On("kick_off", (string data) =>
        {
            data = ToJsonFormat(data);
            Debug.Log("Kick off 들어옴 " + data);
            Packet.ReceivingKickOff receivingKickOff = JsonUtility.FromJson<Packet.ReceivingKickOff>(data);
            for(int i = 0; i < receivingKickOff.PlayerNames.Length; i++)
            {
                Debug.Log("Kick off " + i);
                Debug.Log(receivingKickOff.PlayerNames[i]);
                PlayerController.PlayerInformations[i].PlayerName = receivingKickOff.PlayerNames[i];
                scoreBoard[i].transform.Find("Name").GetComponent<Text>().text = receivingKickOff.PlayerNames[i];
            }

            GameLauncher.IsReadyToKickOff = true;
        });

        Socket.On("score", (string data) =>
        {
            data = ToJsonFormat(data);

            Packet.ReceivingScore receivingScore = JsonUtility.FromJson<Packet.ReceivingScore>(data);
            for(int i = 0; i < sceneMedium.Headcount; i++)
            {
                _scores[i].text = receivingScore.ScoreBoard[i].ToString();
            }
        });

        Socket.On("disconnection", (string data) =>
        {
            int disconnectPlayerIndex = int.Parse(data.Substring(1, data.Length - 2));

            PlayerController.IsConnectPlayers[disconnectPlayerIndex] = false;
            GameLauncher.Headcount--;

            // 게임에 혼자만 남음
            if(GameLauncher.Headcount == 1)
            {

            }

            // 슈퍼 클라이언트 인덱스 변경
            if(disconnectPlayerIndex == PlayerController.SuperClientIndex)
            {
                int index = disconnectPlayerIndex + 1;

                for(; index < sceneMedium.Headcount; index++)
                {
                    if(PlayerController.IsConnectPlayers[index])
                    {
                        PlayerController.SuperClientIndex = index;
                        break;
                    }
                }
            }
            Debug.Log("Super Client: " + PlayerController.SuperClientIndex);

            // 연결이 끊어진 플레이어에 대한 오브젝트 삭제
            Destroy(PlayerController.Players[disconnectPlayerIndex]);
            Destroy(PlayerController.GoalPosts[disconnectPlayerIndex]);
            Destroy(PlayerController.MiniMapManager.Players[disconnectPlayerIndex]);
        });
    }

    public void Send(string header, string message)
    {
        Socket.Emit(header, message);
    }

    // 구조체 전송
    public void Send<T>(string header, object body)
    {
        T packetBody = (T)body;
        // 현재 시스템 시간 전송
        //packetBody.Timestamp = GetTimestamp();
        string json = JsonUtility.ToJson(packetBody);
        //Debug.Log("json " + json);
        Socket.EmitJson(header, json);
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
