using System;
using UnityEngine;
using socket.io;

public class NetworkManager
{
    /* 서버 접속에 관한 요소 */
    //private const string Url = "http://10.21.20.20:9090";
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    private const string Url = "http://15.164.220.253:9090/";
#elif UNITY_WEBGL
    private const string Url = "http://15.164.220.253:9090/";
#endif
    private static Socket _socket;

    /* 동기화를 위한 요소 */
    private static long _rtt;

    /* 서버로 전송할 패킷 */
    public static Packet.PersonalPosition MyPosition { get; set; }
    public static Packet.BallsPosition BallsPosition { get; set; }

    /* 서버로부터의 Ack 확인 */
    public static string GameStart { get; private set; }
    public static string RequestPlayerIndex { get; set; }

    public NetworkManager(bool isGameScene)
    {
        SetWebSocket(isGameScene);
    }

    // Start 함수에서 호출되어야 함
    public void SetWebSocket(bool isGameScene)
    {
        _socket = Socket.Connect(Url);

        // 게임
        if(isGameScene)
        {
            GameStart = "";
            RequestPlayerIndex = "";

            BallsPosition = new Packet.BallsPosition();

            /* 서버로부터 메시지 수신 */
            _socket.On("start_button", (string data) =>
            {
                GameStart = data.Substring(1, 5);
                Debug.Log("==Game Start Message: " + GameStart);
            });

            _socket.On("request_player_index", (string data) =>
            {
                PlayerController.PlayerIndex = int.Parse(data.Substring(1, data.Length - 2));
                MyPosition = new Packet.PersonalPosition(PlayerController.PlayerIndex);
                Debug.Log("==Received Player Index: " + PlayerController.PlayerIndex);
            });

            // 상대좌표 + 공
            _socket.On("relative_position", (string data) =>
            {
                //Debug.Log("FRAME " + GameLauncher.Frame + " 상대좌표");
                //long timestamp = GetTimestamp();

                data = data.Replace("\\", "");
                data = data.Substring(1, data.Length - 2);

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
                data = data.Replace("\\", "");
                data = data.Substring(1, data.Length - 2);

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
            /* 서버로부터 메시지 수신 */
            _socket.On("room_list", (string data) =>
            {
                
            });
        }  
    }

    public static void Send(string header, string message)
    {
        _socket.Emit(header, message);
    }

    // 구조체 전송
    public static void Send<T>(string header, object body)
    {
        T packetBody = (T)body;
        // 현재 시스템 시간 전송
        //packetBody.Timestamp = GetTimestamp();
        string json = JsonUtility.ToJson(packetBody);
        //Debug.Log("json " + json);
        _socket.EmitJson(header, json);
    }

    public static long GetTimestamp()
    {
        TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
        return (long)(timeSpan.TotalSeconds * 1000);
    }

    public static void Destroy()
    {
        _socket = null;
        MyPosition = null;
    }
}
