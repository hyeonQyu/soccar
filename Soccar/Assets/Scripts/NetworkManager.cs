using System;
using UnityEngine;
using socket.io;

public static class NetworkManager
{
    /* 서버 접속에 관한 요소 */
    private const string Url = "http://127.0.0.1:9090/";
    //private const string Url = "http://54.180.145.171:9090";
    private static Socket _socket;

    /* 동기화를 위한 요소 */
    private static long _rtt;

    /* 서버로 전송할 패킷 */
    public static Packet.PlayerMotion MyPlayerMotion { get; set; }

    /* 서버로부터의 Ack 확인 */
    public static string GameStart { get; private set; }
    public static string RequestPlayerIndex { get; set; }

    // Start 함수에서 호출되어야 함
    public static void SetWebSocket()
    {
        GameStart = "";
        RequestPlayerIndex = "";

        _socket = Socket.Connect(Url);

        /* 서버로부터 메시지 수신 */
        _socket.On("start_button", (string data) =>
        {
            GameStart = data.Substring(1, 5);
            Debug.Log("==Game Start Message: " + GameStart);
        });

        _socket.On("request_player_index", (string data) =>
        {
            PlayerController.PlayerIndex = int.Parse(data.Substring(1, data.Length - 2));
            MyPlayerMotion = new Packet.PlayerMotion(PlayerController.PlayerIndex);
            Debug.Log("==Received Player Index: " + PlayerController.PlayerIndex);
        });

        _socket.On("player_motion", (string data) =>
        {
            long timestamp = GetTimestamp();
            
            data = data.Replace("\\", "");
            data = data.Substring(1, data.Length - 2);

            // 캐릭터 이동
            Packet.PlayersPosition playersPosition = JsonUtility.FromJson<Packet.PlayersPosition>(data);
            PlayerController.Move(playersPosition);

            //// 상대방 캐릭터를 이동시킴
            //Packet.PlayerMotion playerMotionFromServer = JsonUtility.FromJson<Packet.PlayerMotion>(data);
            //PlayerController.Move(playerMotionFromServer);

            //if(playerMotionFromServer.PlayerIndex == PlayerController.PlayerIndex)
            //{
            //    // RTT 계산
            //    _rtt = timestamp - playerMotionFromServer.Timestamp;
            //    Debug.Log("RTT: " + _rtt);
            //}
        });
    }

    public static void Send(string header, string message)
    {
        _socket.Emit(header, message);
    }

    // 구조체 전송
    public static void Send(string header, object body)
    {
        Packet.PlayerMotion packetBody = (Packet.PlayerMotion)body;
        // 현재 시스템 시간 전송
        packetBody.Timestamp = GetTimestamp();
        string json = JsonUtility.ToJson(packetBody);
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
        MyPlayerMotion = null;
    }
}
