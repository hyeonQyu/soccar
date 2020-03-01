using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Runtime.InteropServices;

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
    
    private const string IP = "172.30.1.1";
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
                Receive();
            }
        }
    }

    public void StartThread()
    {
        _thread = new Thread(new ThreadStart(RunNetworkThread));
        _thread.Start();
        _isThreadRun = true;
    }

    private static byte[] StructureToByte(object obj)
    {
        int datasize = Marshal.SizeOf(obj);//((PACKET_DATA)obj).TotalBytes; // 구조체에 할당된 메모리의 크기를 구한다.
        IntPtr buff = Marshal.AllocHGlobal(datasize); // 비관리 메모리 영역에 구조체 크기만큼의 메모리를 할당한다.
        Marshal.StructureToPtr(obj, buff, false); // 할당된 구조체 객체의 주소를 구한다.
        byte[] data = new byte[datasize]; // 구조체가 복사될 배열
        Marshal.Copy(buff, data, 0, datasize); // 구조체 객체를 배열에 복사
        Marshal.FreeHGlobal(buff); // 비관리 메모리 영역에 할당했던 메모리를 해제함
        return data; // 배열을 리턴
    }

    private static object ByteToStructure(byte[] data, Type type)
    {
        IntPtr buff = Marshal.AllocHGlobal(data.Length); // 배열의 크기만큼 비관리 메모리 영역에 메모리를 할당한다.
        Marshal.Copy(data, 0, buff, data.Length); // 배열에 저장된 데이터를 위에서 할당한 메모리 영역에 복사한다.
        object obj = Marshal.PtrToStructure(buff, type); // 복사된 데이터를 구조체 객체로 변환한다.
        Marshal.FreeHGlobal(buff); // 비관리 메모리 영역에 할당했던 메모리를 해제함

        if(Marshal.SizeOf(obj) != data.Length)// (((PACKET_DATA)obj).TotalBytes != data.Length) // 구조체와 원래의 데이터의 크기 비교
        {
            return null; // 크기가 다르면 null 리턴
        }
        return obj; // 구조체 리턴
    }

    public void Send(int msgType, object obj)
    {
        Packet.Header header = new Packet.Header(msgType);

        // 헤더와 패킷 구조체를 바이트화
        byte[] byteBody = StructureToByte(obj);
        header.Length = byteBody.Length;
        byte[] byteHeader = StructureToByte(header);

        // 최종 전송할 패킷을 바이트화
        byte[] packet = new byte[byteHeader.Length + byteBody.Length];
        Buffer.BlockCopy(byteHeader, 0, packet, 0, byteHeader.Length);
        Buffer.BlockCopy(byteBody, 0, packet, byteHeader.Length, byteBody.Length);

        // 패킷 전송
        _socket.Send(packet, packet.Length, SocketFlags.None);
    }

    public void Receive()
    {
        // 헤더를 받음
        Packet.Header header = new Packet.Header(-1);
        byte[] byteHeader = new byte[Marshal.SizeOf(header)];
        _socket.Receive(byteHeader, byteHeader.Length, SocketFlags.None);
        header = (Packet.Header) ByteToStructure(byteHeader, typeof(Packet.Header));

        byte[] byteBody = new byte[header.Length];
        _socket.Receive(byteBody, byteBody.Length, SocketFlags.None);

        object packet = new object();
        switch(header.MsgType)
        {
            //case Packet.TypeStartButtonSend:
            //    packet = (Packet.StartButtonSend)ByteToStructure(byteBody, typeof(Packet.StartButtonSend));
            //    break;
            case Packet.TypeStartButtonAck:
                StartButtonAck = (Packet.StartButtonAck)ByteToStructure(byteBody, typeof(Packet.StartButtonAck));
                break;
            //case Packet.TypeRequestPlayerIndexSend:
            //    packet = (Packet.RequestPlayerIndexSend)ByteToStructure(byteBody, typeof(Packet.RequestPlayerIndexSend));
            //    break;
            case Packet.TypeRequestPlayerIndexAck:
                RequestplayerIndexAck = (Packet.RequestPlayerIndexAck)ByteToStructure(byteBody, typeof(Packet.RequestPlayerIndexAck));
                break;
            //case Packet.TypePlayerMotionSend:
            //    packet = (Packet.PlayerMotionSend)ByteToStructure(byteBody, typeof(Packet.PlayerMotionSend));
            //    break;
            case Packet.TypePlayerMotionAck:
                PlayerMotionAck = (Packet.PlayerMotionAck)ByteToStructure(byteBody, typeof(Packet.PlayerMotionAck));
                break;
        }
    }
}
