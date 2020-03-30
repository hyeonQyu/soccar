using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using socket.io;

public static class NetworkManager
{
    private const string Url = "http://54.180.145.171:9090";//"http://127.0.0.1:9090/";
    private static Socket _socket;

    // 현재 플레이어가 컨트롤 하는 캐릭터의 움직임
    public static Packet.PlayerMotion MyPlayerMotion { get; set; }

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
            // 상대방 캐릭터를 이동시킴
            Packet.PlayerMotion playerMotionFromServer = JsonUtility.FromJson<Packet.PlayerMotion>(data);
            PlayerController.Move(playerMotionFromServer);
        });
    }

    public static void Send(string header, string message)
    {
        _socket.Emit(header, message);
    }

    // 구조체 전송
    public static void Send(string header, object body)
    {
        string json = JsonUtility.ToJson(body);
        _socket.EmitJson(header, json);
    }

    public static void Destroy()
    {
        _socket = null;
        MyPlayerMotion = null;
    }
}
