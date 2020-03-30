using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class PlayerController
{
    // 전체 플레이어
    public static GameObject[] Players { get; set; }
    // 현재 컨트롤하는 플레이어
    public static GameObject Player { get; set; }

    // 속도
    private static float _walkSpeed;
    private static float _runSpeed;
    private static float _playerSpeed;

    // 99 = 서버로 부터 값을 받지 않음.
    private static int _playerIndex = 99;
    public static int PlayerIndex
    {
        get
        {
            return _playerIndex;
        }
        set
        {
            _playerIndex = value;
        }
    }

    // 움직임 발생시 true로 변환하여 서버로 패킷전송
    private static bool _isMoved = false;
    private static bool _isPlayersInitialized = false;
    public static bool IsPlayerInitialized
    {
        get { return _isPlayersInitialized; }
    }


    //private static bool _isConnected = false;
    //public static bool IsConnected
    //{
    //    get
    //    {
    //        return _isConnected;
    //    }
    //    set
    //    {
    //        _isConnected = value;
    //    }
    //}
    //private bool _isFull = false;
    //public static bool IsClickedStart { get => _isClickedStart; set => _isClickedStart = value; }
    //private static bool _isClickedStart = false;

    public static void SetPlayers()
    {
        _walkSpeed = 10;
        _runSpeed = _walkSpeed * 2;

        Players = new GameObject[4];
        Players[0] = GameObject.Find("Player1");
        Players[1] = GameObject.Find("Player2");
        Players[2] = GameObject.Find("Player3");
        Players[3] = GameObject.Find("Player4");
    }

    //// Use this for initialization
    //void Start()
    //{
    //    //_runSpeed = _walkSpeed * 2;
    //    //NetworkManager.SetWebSocket();

    //    //Players = new GameObject[4];
    //    //Players[0] = GameObject.Find("Player1");

    //    //Players[1] = GameObject.Find("Player2");
    //    //Players[2] = GameObject.Find("Player3");
    //    //Players[3] = GameObject.Find("Player4");
    //    //_networkThread = new NetworkThread();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (_isClickedStart)
    //    {
    //        Debug.Log("Start button clicked");

    //        // 게임 시작 버튼이 눌렸음을 서버에 전송
    //        NetworkManager.Send("start_button", "start");

    //        _isClickedStart = false;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
    //        return;

    //        //Packet.StartButtonSend startButtonSend = new Packet.StartButtonSend();
    //        //startButtonSend.CheckNum = NetworkThread.GameStartPacket;
    //        //_networkThread.SendPacket(Packet.TypeStartButtonSend, startButtonSend);

    //        //// 서버로부터 Ack를 받음
    //        //_networkThread.ReceivePacket();

    //        //byte[] startPacket = BitConverter.GetBytes(NetworkThread.GameStartPacket);
    //        //_networkThread.Socket.Send(BitConverter.GetBytes(startPacket.Length), 4, SocketFlags.None);
    //        //_networkThread.Socket.Send(startPacket, startPacket.Length, SocketFlags.None);

    //        //byte[] startPacketAck = new byte[4];
    //        //_networkThread.Socket.Receive(startPacketAck, 4, SocketFlags.None);
                
            

    //        //if (NetworkThread.GameStartPacket == _networkThread.StartButtonAck.CheckNum)
    //        //{
    //        //    // 게임시작 버튼 클릭 정상적으로 완료
    //        //    Debug.Log("GameStart. Received right ACK from server");

    //        //    // 플레이어 인덱스 요청
    //        //    Packet.RequestPlayerIndexSend requestPlayerIndexSend = new Packet.RequestPlayerIndexSend();
    //        //    requestPlayerIndexSend.CheckNum = NetworkThread.RequestPlayerIndex;
    //        //    _networkThread.SendPacket(Packet.TypeRequestPlayerIndexSend, requestPlayerIndexSend);

    //        //    _isClickedStart = false;
    //        //    _isFull = true;
    //        //    _waitingText.SetActive(true);

    //        //    //byte[] indexPacket = BitConverter.GetBytes(NetworkThread.RequestPlayerIndex);
    //        //    //_networkThread.Socket.Send(BitConverter.GetBytes(indexPacket.Length), 4, SocketFlags.None);
    //        //    //_networkThread.Socket.Send(indexPacket, indexPacket.Length, SocketFlags.None);
    //        //    //_isClickedStart = false;
    //        //    //_isFull = true;
    //        //    //_waitingText.SetActive(true);
    //        //}
    //    }

    //    // 게임 시작 Ack를 제대로 수신하면
    //    if(NetworkManager.GameStart == "start" && _playerIndex == 99)
    //    {
    //        // Player Index 요청
    //        NetworkManager.Send("request_player_index", "req");

    //        while(_playerIndex == 99)
    //        {
    //            Debug.Log("==수신 대기중..."); // Player Index를 서버로부터 수신할 때까지 대기
    //        }
    //        Debug.Log("==인덱스 받음, 초기화 할차례");

    //        // 플레이어 초기화
    //        Player = Players[_playerIndex];
    //        Player.GetComponent<PlayerInformation>().ID = ButtonControl.InputID.text;
    //        return;
    //    }

    //    KeyDowned();

    //    //// 게임 인원이 충족되면
    //    //if (_isFull && _networkThread.Socket.Poll(0, SelectMode.SelectRead)) 
    //    //{
    //    //    // 플레이어 인덱스를 받음
    //    //    _networkThread.ReceivePacket();
    //    //    _playerIndex = _networkThread.RequestplayerIndexAck.PlayerIndex;

    //    //    // 플레이어들끼리 연결됨
    //    //    _waitingText.SetActive(false);
    //    //    _isConnected = true;
    //    //    _isFull = false;
    //    //    Debug.Log("Receive Index From Server = " + _playerIndex);

    //    //    _networkThread.StartThread();

    //    //    //byte[] receivedIndex = new byte[4];
    //    //    //_networkThread.Socket.Receive(receivedIndex, 4, SocketFlags.None);
    //    //    //_myPlayerIndex = BitConverter.ToInt32(receivedIndex, 0);

    //    //    //// In Camera.cs for checking received clientIndex from server
    //    //    //_waitingText.SetActive(false);
    //    //    //_isConnected = true;
    //    //    //_isFull = false;
    //    //    //Debug.Log("Receive Index From Server = " + _myPlayerIndex);

    //    //    //_networkThread.StartThread();
    //    //}

    //    //if (_isConnected)
    //    //{
    //    //    KeyDowned();
    //    //    Move();
    //    //}
    //}

    public static void InitializePlayer()
    {
        Player = Players[_playerIndex];
        Player.GetComponent<PlayerInformation>().ID = ButtonControl.InputID.text;

        _isPlayersInitialized = true;
    }

    public static void KeyDowned()
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

        if(Input.GetKey(KeyCode.LeftArrow))
        {
            switch(_playerIndex)
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
            switch(_playerIndex)
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
            switch(_playerIndex)
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
            switch(_playerIndex)
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
            // 움직임 전송
            NetworkManager.MyPlayerMotion.SetLocation(myPosition);
            NetworkManager.Send("player_motion", NetworkManager.MyPlayerMotion);

            // 자신의 캐릭터를 움직임
            Move(myPosition);

            _isMoved = false;
        }
    }

    // 자신의 캐릭터를 움직임
    private static void Move(Vector3 movingPosition)
    {
        Players[_playerIndex].transform.Translate(movingPosition);
    }

    // 서버로부터 타 플레이어의 캐릭터 움직임을 전달받아 움직임
    public static void Move(Packet.PlayerMotion playerMotionFromServer)
    {
        Vector3 movingPosition = new Vector3(playerMotionFromServer.X, playerMotionFromServer.Y, playerMotionFromServer.Z);
        Players[playerMotionFromServer.PlayerIndex].transform.Translate(movingPosition);
    }      

}
