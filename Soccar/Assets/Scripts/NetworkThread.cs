using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Runtime.InteropServices;
using System.Text;

//public struct SyncInformation
//{
//    public int PlayerIndex;
//    public float X;
//    public float Y;
//    public float Z;

//    public SyncInformation(int playerIndex, float x, float y, float z)
//    {
//        PlayerIndex = playerIndex;
//        X = x;
//        Y = y;
//        Z = z;
//    }
//}
public class NetworkThread
{
    private Thread _thread;
    private bool _isThreadRun = false;
    public bool IsThreadRun
    {
        set
        {
            _isThreadRun = value;
        }
    }

    private Socket _socket = null;
    public Socket Socket
    {
        get
        {
            return _socket;
        }
    }
    
    private const string IP = "127.0.0.1";
    private const int Port = 6666;
    private const int UninitializedPlayerIndex = 9;

    public const int GameStartPacket = 2015;
    public const int RequestPlayerIndex = 8282;

    //// 시작 버튼 확인
    //private Packet.StartButtonAck _ackStartButton;
    //public Packet.StartButtonAck AckStartButton
    //{
    //    get
    //    {
    //        return _ackStartButton;
    //    }
    //}

    public Packet.StartButtonAck StartButtonAck;
    public Packet.RequestPlayerIndexAck RequestplayerIndexAck;
    //private Packet.PlayerMotionAck[] _playerMotionAcks = new Packet.PlayerMotionAck[4];
    //public Packet.PlayerMotionAck[] PlayerMotion
    //{
    //    get
    //    {
    //        return _playerMotionAcks;
    //    }
    //}
    public Packet.PlayerMotionAck PlayerMotionAck;

    public NetworkThread()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        _socket.Connect(IPAddress.Parse(IP), Port);

        PlayerMotionAck = new Packet.PlayerMotionAck(UninitializedPlayerIndex, 0, 0, 0);

        //for(int i = 0; i < 4; i++)
        //{
        //    _playerMotionAcks[i] = new Packet.PlayerMotionAck();
        //}
        //_sendPosition = new SyncInformation(UninitializedPlayerIndex, 0, 0, 0);
        //_receivePosition = new SyncInformation(UninitializedPlayerIndex, 0, 0, 0);
    }

    private void RunNetworkThread()
    {
        while(_isThreadRun)
        {
            if(_socket.Poll(0, SelectMode.SelectRead))
            {
                Debug.Log("@@@@@@@@@@@@@ Receive Data From Server @@@@@@@@@@@@@@@@@@@@@@@");
                //byte[] receivedData = new byte[16];
                //_socket.Receive(receivedData, 16, SocketFlags.None);
                //_receivePosition = (SyncInformation)ByteToStructure(receivedData, typeof(SyncInformation));
                ReceivePacket();
            }
        }
    }

    public void StartThread()
    {
        _thread = new Thread(new ThreadStart(RunNetworkThread));
        _thread.Start();
        _isThreadRun = true;
    }

    public void SendPacket(int msgType, object body)
    {
        Packet.Header header = new Packet.Header(msgType);

        // 패킷을 Json으로 변환 후 byte 배열로 변환
        string jsonBody = JsonManager.ObjectToJson(body);
        byte[] byteBody = JsonManager.JsonToByte(jsonBody);

        // 헤더를 Json으로 변환 후 byte 배열로 변환
        header.Length = byteBody.Length;
        string jsonHeader = JsonManager.ObjectToJson(header);
        JsonManager.SetHeaderLength(header, ref jsonHeader);
        Debug.Log(jsonHeader + ". " + jsonHeader.Length);
        byte[] byteHeader = JsonManager.JsonToByte(jsonHeader);

        // 최종 전송할 패킷을 바이트화
        byte[] bytePacket = new byte[byteHeader.Length + byteBody.Length];
        Buffer.BlockCopy(byteHeader, 0, bytePacket, 0, byteHeader.Length);
        Buffer.BlockCopy(byteBody, 0, bytePacket, byteHeader.Length, byteBody.Length);

        // 패킷 전송
        _socket.Send(bytePacket, bytePacket.Length, SocketFlags.None);
    }

    public void ReceivePacket()
    {
        // 헤더를 받음
        byte[] byteHeader = new byte[Packet.LengthHeader];
        _socket.Receive(byteHeader, Packet.LengthHeader, SocketFlags.None);
        string jsonHeader = JsonManager.ByteToJson(byteHeader);
        Packet.Header header = JsonManager.JsonToObject<Packet.Header>(jsonHeader);

        // 패킷 내용을 받음
        byte[] byteBody = new byte[header.Length];
        _socket.Receive(byteBody, header.Length, SocketFlags.None);
        string jsonBody = JsonManager.ByteToJson(byteBody);

        //object packet = new object();
        switch(header.MsgType)
        {
            //case Packet.TypeStartButtonSend:
            //    packet = (Packet.StartButtonSend)ByteToStructure(byteBody, typeof(Packet.StartButtonSend));
            //    break;
            case Packet.TypeStartButtonAck:
                StartButtonAck = JsonManager.JsonToObject<Packet.StartButtonAck>(jsonBody);
                break;
            //case Packet.TypeRequestPlayerIndexSend:
            //    packet = (Packet.RequestPlayerIndexSend)ByteToStructure(byteBody, typeof(Packet.RequestPlayerIndexSend));
            //    break;
            case Packet.TypeRequestPlayerIndexAck:
                RequestplayerIndexAck = JsonManager.JsonToObject<Packet.RequestPlayerIndexAck>(jsonBody);
                break;
            //case Packet.TypePlayerMotionSend:
            //    packet = (Packet.PlayerMotionSend)ByteToStructure(byteBody, typeof(Packet.PlayerMotionSend));
            //    break;
            case Packet.TypePlayerMotionAck:
                PlayerMotionAck = JsonManager.JsonToObject<Packet.PlayerMotionAck>(jsonBody);
                break;
        }
    }
}
