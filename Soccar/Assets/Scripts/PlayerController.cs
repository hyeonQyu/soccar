using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _walkSpeed;

    private float _runSpeed;
    private float _playerSpeed;

    // 99 = 서버로 부터 값을 받지 않음.
    public static int _myPlayerIndex = 99;

    private Socket socket = null;

    private bool _isMoved = false;

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



    struct NetMove
    {
        public int playerIndex;
        public float x;
        public float y;
        public float z;

        public NetMove(int playerIndex, float x, float y, float z)
        {
            this.playerIndex = playerIndex;
            this.x = x;
            this.y = y;
            this.z = z;
        }
    };

    NetMove sendPosition;
    NetMove receivePosition;

    /*   현규가 만든 내용
    [SerializeField]
    private string _id;
    public string Id
    {
        get
        {
            return _id;
        }
    }
    private int _score = 0;
    public int Score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
        }
    }
    */

    // Use this for initialization
    void Start()
    {
        _runSpeed = _walkSpeed * 2;
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        socket.Connect(IPAddress.Parse("127.0.0.1"), 6666);
        sendPosition = new NetMove(9, 0, 0, 0);
        receivePosition = new NetMove(9, 0, 0, 0);

        byte[] receivedData = new byte[4];
        socket.Receive(receivedData, 4, SocketFlags.None);
        _myPlayerIndex = BitConverter.ToInt32(receivedData, 0);
        // In Camera.cs for checking receive clientIndex from server
        _isConnected = true;
        Debug.Log("Receive Index From Server = " + _myPlayerIndex);

    }

    // Update is called once per frame
    void Update()
    {
        KeyDowned();
        Move();
    }

    private void KeyDowned()
    {
            Vector3 myPosition = new Vector3(0, 0, 0);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _playerSpeed = _runSpeed;
            }
            else
            {
                _playerSpeed = _walkSpeed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                switch (_myPlayerIndex)
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
            if (Input.GetKey(KeyCode.D))
            {
                switch (_myPlayerIndex)
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
            if (Input.GetKey(KeyCode.W))
            {
                switch (_myPlayerIndex)
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
            if (Input.GetKey(KeyCode.S))
            {
                switch (_myPlayerIndex)
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
            if (_isMoved)
            {
                SendPositionToServer(myPosition);
                _isMoved = false;
            }
        }

    private void Move()
    {
        if (socket.Poll(0, SelectMode.SelectRead))
        {
            Debug.Log("@@@@@@@@@@@@@ Receive Data From Server @@@@@@@@@@@@@@@@@@@@@@@");
            byte[] receivedData = new byte[16];
            Debug.Log("11111111");
            socket.Receive(receivedData, 16, SocketFlags.None);
            Debug.Log("222222222");
            receivePosition = (NetMove)ByteToStructure(receivedData, typeof(NetMove));
            Debug.Log("receivedPosition x = " + receivePosition.x);
            Vector3 vector3 = new Vector3(receivePosition.x, receivePosition.y, receivePosition.z);
            Camera.GameObjectsList[receivePosition.playerIndex].transform.Translate(vector3);
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
        sendPosition.playerIndex = _myPlayerIndex;
        sendPosition.x = positon.x;
        sendPosition.y = positon.y;
        sendPosition.z = positon.z;
        //Debug.Log("SendPositionToServer = " +StructureToByte(sendPosition).Length);
        Action<byte[]> Send = (b) =>
        {
            socket.Send(BitConverter.GetBytes(b.Length), 4, SocketFlags.None);
            socket.Send(b, b.Length, SocketFlags.None);
        };
        Debug.Log("Send !!~~~~");
        Send(StructureToByte(sendPosition));
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Quit");
        socket.Disconnect(true);
        socket.Close();

    }

    /*  현규가 만든 거
    // 득점
    public void Scores(ref GameObject conceder)
    {
        PlayerController concederPlayer = conceder.GetComponent<PlayerController>();

        // 득점자는 +2점, 실점자는 -1점
        if(concederPlayer.Id != _id)
        {
            _score += 2;
            // 득점에 대한 메시지 
            Debug.Log("득점자: " + _id + "   실점자: " + concederPlayer.Id);
        }
        // 자책골, 자책골은 득점으로 인정하지 않음
        else
        {
            // 자책골에 대한 메시지
            Debug.Log(_id + "의 자책골");
        }
        concederPlayer.Score--;

        Debug.Log(_id + ": " + _score + "    " + concederPlayer.Id + ": " + concederPlayer.Score);
    }
    */

}
