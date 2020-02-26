using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

struct SyncInformation
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

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _walkSpeed;
    private float _runSpeed;
    private float _playerSpeed;

    [SerializeField]
    GameObject _waitingText;
    // 99 = 서버로 부터 값을 받지 않음.
    private static int _myPlayerIndex = 99;
    public static int MyPlayerIndex
    {
        get
        {
            return _myPlayerIndex;
        }
    }

    private bool _isMoved = false; // 움직임 발생시 true로 변환하여 서버로 패킷전송

    private static bool _isConnected = false;
    public static bool IsConnected
    {
        get
        {
            return _isConnected;
        }
        set
        {
            _isConnected = value;
        }
    }

    private bool _isFull = false;

    public static bool IsClickedStart { get => _isClickedStart; set => _isClickedStart = value; }
    private static bool _isClickedStart = false;

    private Socket _socket = null;
    private const int GameStartPacket = 2015;
    private const int RequestPlayerIndex = 8282;


    SyncInformation _sendPosition;
    SyncInformation _receivePosition;

    // Use this for initialization
    void Start()
    {
        _runSpeed = _walkSpeed * 2;
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        _socket.Connect(IPAddress.Parse("127.0.0.1"), 6666);
        _sendPosition = new SyncInformation(9, 0, 0, 0);
        _receivePosition = new SyncInformation(9, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isClickedStart)
        {
            Debug.Log("Start button clicked");
            byte[] startPacket = BitConverter.GetBytes(GameStartPacket);
            _socket.Send(BitConverter.GetBytes(startPacket.Length), 4, SocketFlags.None);
            _socket.Send(startPacket, startPacket.Length, SocketFlags.None);
            byte[] startPacketAck = new byte[4];
            _socket.Receive(startPacketAck, 4, SocketFlags.None);

            if (GameStartPacket == BitConverter.ToInt32(startPacketAck, 0))
            { // 게임시작 버튼 클릭 정상적으로 완료
                Debug.Log("GameStart. Received right ACK from server");
                byte[] indexPacket = BitConverter.GetBytes(RequestPlayerIndex);
                _socket.Send(BitConverter.GetBytes(indexPacket.Length), 4, SocketFlags.None);
                _socket.Send(indexPacket, indexPacket.Length, SocketFlags.None);
                _isClickedStart = false;
                _isFull = true;
                _waitingText.SetActive(true);
            }
        }

        if (_isFull && _socket.Poll(0, SelectMode.SelectRead)) {
            byte[] receivedIndex = new byte[4];
            _socket.Receive(receivedIndex, 4, SocketFlags.None);
            _myPlayerIndex = BitConverter.ToInt32(receivedIndex, 0);
            // In Camera.cs for checking received clientIndex from server
            _waitingText.SetActive(false);
            _isConnected = true;
            _isFull = false;
            Debug.Log("Receive Index From Server = " + _myPlayerIndex);
        }

        if (_isConnected)
        {
            KeyDowned();
            Move();
        }
    }

    private void KeyDowned()
    {
        Vector3 myPosition = new Vector3(0, 0, 0);
        if(Input.GetKey(KeyCode.LeftShift))
        {
            _playerSpeed = _runSpeed;
        }
        else
        {
            _playerSpeed = _walkSpeed;
        }

        if(Input.GetKey(KeyCode.A))
        {
            switch(_myPlayerIndex)
            {
                case 0:
                    myPosition += (Vector3.left * _playerSpeed * Time.deltaTime);
                    break;
                case 1:
                    myPosition += (Vector3.forward * _playerSpeed * Time.deltaTime);
                    break;
                case 2:
                    myPosition += (Vector3.right * _playerSpeed * Time.deltaTime);
                    break;
                case 3:
                    myPosition += (Vector3.back * _playerSpeed * Time.deltaTime);
                    break;
            }
            _isMoved = true;
        }
        if(Input.GetKey(KeyCode.D))
        {
            switch(_myPlayerIndex)
            {
                case 0:
                    myPosition += (Vector3.right * _playerSpeed * Time.deltaTime);
                    break;
                case 1:
                    myPosition += (Vector3.back * _playerSpeed * Time.deltaTime);
                    break;
                case 2:
                    myPosition += (Vector3.left * _playerSpeed * Time.deltaTime);
                    break;
                case 3:
                    myPosition += (Vector3.forward * _playerSpeed * Time.deltaTime);
                    break;
            }
            _isMoved = true;
        }
        if(Input.GetKey(KeyCode.W))
        {
            switch(_myPlayerIndex)
            {
                case 0:
                    myPosition += (Vector3.forward * _playerSpeed * Time.deltaTime);
                    break;
                case 1:
                    myPosition += (Vector3.right * _playerSpeed * Time.deltaTime);
                    break;
                case 2:
                    myPosition += (Vector3.back * _playerSpeed * Time.deltaTime);
                    break;
                case 3:
                    myPosition += (Vector3.left * _playerSpeed * Time.deltaTime);
                    break;
            }
            _isMoved = true;
        }
        if(Input.GetKey(KeyCode.S))
        {
            switch(_myPlayerIndex)
            {
                case 0:
                    myPosition += (Vector3.back * _playerSpeed * Time.deltaTime);
                    break;
                case 1:
                    myPosition += (Vector3.left * _playerSpeed * Time.deltaTime);
                    break;
                case 2:
                    myPosition += (Vector3.forward * _playerSpeed * Time.deltaTime);
                    break;
                case 3:
                    myPosition += (Vector3.right * _playerSpeed * Time.deltaTime);
                    break;
            }
            _isMoved = true;
        }

        if(_isMoved)
        {
            SendPositionToServer(myPosition);
            _isMoved = false;
        }
    }

    private void Move()
    {
        if (_socket.Poll(0, SelectMode.SelectRead))
        {
            Debug.Log("@@@@@@@@@@@@@ Receive Data From Server @@@@@@@@@@@@@@@@@@@@@@@");
            byte[] receivedData = new byte[16];
            Debug.Log("11111111");
            _socket.Receive(receivedData, 16, SocketFlags.None);
            Debug.Log("222222222");
            _receivePosition = (SyncInformation)ByteToStructure(receivedData, typeof(SyncInformation));
            Debug.Log("receivedPosition x = " + _receivePosition.X);
            Vector3 vector3 = new Vector3(_receivePosition.X, _receivePosition.Y, _receivePosition.Z);
            Camera.PlayerList[_receivePosition.PlayerIndex].transform.Translate(vector3);
        }
    }  

    public static byte[] StructureToByte(object obj)
    {
        int datasize = Marshal.SizeOf(obj);//((PACKET_DATA)obj).TotalBytes; // 구조체에 할당된 메모리의 크기를 구한다.
        IntPtr buff = Marshal.AllocHGlobal(datasize); // 비관리 메모리 영역에 구조체 크기만큼의 메모리를 할당한다.
        Marshal.StructureToPtr(obj, buff, false); // 할당된 구조체 객체의 주소를 구한다.
        byte[] data = new byte[datasize]; // 구조체가 복사될 배열
        Marshal.Copy(buff, data, 0, datasize); // 구조체 객체를 배열에 복사
        Marshal.FreeHGlobal(buff); // 비관리 메모리 영역에 할당했던 메모리를 해제함
        return data; // 배열을 리턴
    }

    public static object ByteToStructure(byte[] data, Type type)
    {
        IntPtr buff = Marshal.AllocHGlobal(data.Length); // 배열의 크기만큼 비관리 메모리 영역에 메모리를 할당한다.
        Marshal.Copy(data, 0, buff, data.Length); // 배열에 저장된 데이터를 위에서 할당한 메모리 영역에 복사한다.
        object obj = Marshal.PtrToStructure(buff, type); // 복사된 데이터를 구조체 객체로 변환한다.
        Marshal.FreeHGlobal(buff); // 비관리 메모리 영역에 할당했던 메모리를 해제함

        if (Marshal.SizeOf(obj) != data.Length)// (((PACKET_DATA)obj).TotalBytes != data.Length) // 구조체와 원래의 데이터의 크기 비교
        {
            return null; // 크기가 다르면 null 리턴
        }
        return obj; // 구조체 리턴
    }

    public void SendPositionToServer(Vector3 positon)
    {
        // 여기서 나의 캐릭터 인덱스를 넣어야함. -> _myPlayerIndex;
        _sendPosition.PlayerIndex = _myPlayerIndex;
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

    private void OnApplicationQuit()
    {
        Debug.Log("Quit");
        _socket.Disconnect(true);
        _socket.Close();
    }

}
