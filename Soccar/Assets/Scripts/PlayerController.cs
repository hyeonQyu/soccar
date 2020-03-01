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
    NetworkThread _networkThread;
    private Packet.PlayerMotionSend _playerMotionSend = new Packet.PlayerMotionSend();

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

    // Use this for initialization
    void Start()
    {
        _runSpeed = _walkSpeed * 2;
        _networkThread = new NetworkThread();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isClickedStart)
        {
            Debug.Log("Start button clicked");

            // 게임 시작 버튼이 눌렸음을 서버에 전송
            Packet.StartButtonSend startButtonSend = new Packet.StartButtonSend();
            startButtonSend.CheckNum = NetworkThread.GameStartPacket;
            _networkThread.Send(Packet.TypeStartButtonSend, startButtonSend);

            // 서버로부터 Ack를 받음
            _networkThread.Receive();

            //byte[] startPacket = BitConverter.GetBytes(NetworkThread.GameStartPacket);
            //_networkThread.Socket.Send(BitConverter.GetBytes(startPacket.Length), 4, SocketFlags.None);
            //_networkThread.Socket.Send(startPacket, startPacket.Length, SocketFlags.None);

            //byte[] startPacketAck = new byte[4];
            //_networkThread.Socket.Receive(startPacketAck, 4, SocketFlags.None);

            if (NetworkThread.GameStartPacket == _networkThread.StartButtonAck.CheckNum)
            {
                // 게임시작 버튼 클릭 정상적으로 완료
                Debug.Log("GameStart. Received right ACK from server");

                // 플레이어 인덱스 요청
                Packet.RequestPlayerIndexSend requestPlayerIndexSend = new Packet.RequestPlayerIndexSend();
                requestPlayerIndexSend.CheckNum = NetworkThread.RequestPlayerIndex;
                _networkThread.Send(Packet.TypeRequestPlayerIndexSend, requestPlayerIndexSend);

                _isClickedStart = false;
                _isFull = true;
                _waitingText.SetActive(true);

                //byte[] indexPacket = BitConverter.GetBytes(NetworkThread.RequestPlayerIndex);
                //_networkThread.Socket.Send(BitConverter.GetBytes(indexPacket.Length), 4, SocketFlags.None);
                //_networkThread.Socket.Send(indexPacket, indexPacket.Length, SocketFlags.None);
                //_isClickedStart = false;
                //_isFull = true;
                //_waitingText.SetActive(true);
            }
        }

        // 게임 인원이 충족되면
        if (_isFull && _networkThread.Socket.Poll(0, SelectMode.SelectRead)) 
        {
            // 플레이어 인덱스를 받음
            _networkThread.Receive();
            _myPlayerIndex = _networkThread.RequestplayerIndexAck.PlayerIndex;

            // 플레이어들끼리 연결됨
            _waitingText.SetActive(false);
            _isConnected = true;
            _isFull = false;
            Debug.Log("Receive Index From Server = " + _myPlayerIndex);

            _networkThread.StartThread();

            //byte[] receivedIndex = new byte[4];
            //_networkThread.Socket.Receive(receivedIndex, 4, SocketFlags.None);
            //_myPlayerIndex = BitConverter.ToInt32(receivedIndex, 0);

            //// In Camera.cs for checking received clientIndex from server
            //_waitingText.SetActive(false);
            //_isConnected = true;
            //_isFull = false;
            //Debug.Log("Receive Index From Server = " + _myPlayerIndex);

            //_networkThread.StartThread();
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
        //Vector3 myPosition = Camera.PlayerList[_myPlayerIndex].transform.position;
        if(Input.GetKey(KeyCode.LeftShift))
        {
            _playerSpeed = _runSpeed;
        }
        else
        {
            _playerSpeed = _walkSpeed;
        }

        if(Input.GetKey(KeyCode.LeftArrow))
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
        if(Input.GetKey(KeyCode.RightArrow))
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
        if(Input.GetKey(KeyCode.UpArrow))
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
        if(Input.GetKey(KeyCode.DownArrow))
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
            _playerMotionSend.PlayerIndex = _myPlayerIndex;
            _playerMotionSend.X = myPosition.x;
            _playerMotionSend.Y = myPosition.y;
            _playerMotionSend.Z = myPosition.z;

            // 움직임 전송
            _networkThread.Send(Packet.TypePlayerMotionSend, _playerMotionSend);
            _isMoved = false;
        }
    }

    private void Move()
    {
        Vector3 vector3 = new Vector3(_networkThread.PlayerMotionAck.X, _networkThread.PlayerMotionAck.Y, _networkThread.PlayerMotionAck.Z);
        Camera.PlayerList[_networkThread.PlayerMotionAck.PlayerIndex].transform.Translate(vector3);
        //Camera.PlayerList[_networkThread.ReceivePosition.PlayerIndex].transform.position = vector3;     
    }      

    private void OnApplicationQuit()
    {
        Debug.Log("Quit");
        _networkThread.Socket.Disconnect(true);
        _networkThread.Socket.Close();
        _networkThread.IsThreadRun = false;
    }

}
