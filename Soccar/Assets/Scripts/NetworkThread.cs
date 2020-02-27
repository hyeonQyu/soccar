using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Runtime.InteropServices;

public struct SyncInformation
{
    public int PlayerIndex;
    public float X;
    public float Y;
    public float Z;

    public SyncInformation(int playerIndex, float x, float y, float z)
    {
        PlayerIndex = playerIndex;
        X = x;
        Y = y;
        Z = z;
    }
}

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
    
    private const string IP = "10.14.4.23";
    private const int Port = 6666;
    private const int UninitializedPlayerIndex = 9;

    public const int GameStartPacket = 2015;
    public const int RequestPlayerIndex = 8282;

    private SyncInformation _sendPosition;
    public SyncInformation SendPosition
    {
        get
        {
            return _sendPosition;
        }
        set
        {
            _sendPosition = value;
        }
    }
    private SyncInformation _receivePosition;
    public SyncInformation ReceivePosition
    {
        get
        {
            return _receivePosition;
        }
        set
        {
            _receivePosition = value;
        }
    }

    public NetworkThread()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        _socket.Connect(IPAddress.Parse(IP), Port);
        _sendPosition = new SyncInformation(UninitializedPlayerIndex, 0, 0, 0);
        _receivePosition = new SyncInformation(UninitializedPlayerIndex, 0, 0, 0);
    }

    private void RunNetworkThread()
    {
        while(_isThreadRun)
        {
            Debug.Log("@@@@@@@@@@@@@ Receive Data From Server @@@@@@@@@@@@@@@@@@@@@@@");
            byte[] receivedData = new byte[16];
            _socket.Receive(receivedData, 16, SocketFlags.None);
            _receivePosition = (SyncInformation)ByteToStructure(receivedData, typeof(SyncInformation));
        }
    }

    public void StartThread()
    {
        _thread = new Thread(new ThreadStart(RunNetworkThread));
        _thread.Start();
        _isThreadRun = true;
    }

    public void SendPositionToServer(Vector3 positon, int myPlayerIndex)
    {
        // 여기서 나의 캐릭터 인덱스를 넣어야함. -> _myPlayerIndex;
        _sendPosition.PlayerIndex = myPlayerIndex;
        _sendPosition.X = positon.x;
        _sendPosition.Y = positon.y;
        _sendPosition.Z = positon.z;
        //Debug.Log("SendPositionToServer = " +StructureToByte(sendPosition).Length);

        Action<byte[]> Send = (b) =>
        {
            _socket.Send(BitConverter.GetBytes(b.Length), 4, SocketFlags.None);
            _socket.Send(b, b.Length, SocketFlags.None);
        };

        Debug.Log("Send !!~~~~");
        Send(StructureToByte(_sendPosition));
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
}
