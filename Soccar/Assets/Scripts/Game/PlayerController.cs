using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerController
{
    public const int Relative = 0;
    public const int Absolute = 1;

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

    public static void InitializePlayer()
    {
        Player = Players[_playerIndex];
        Player.GetComponent<PlayerInformation>().ID = ButtonControl.InputID.text;

        _isPlayersInitialized = true;
    }

    public static void InputRelativePosition()
    {
        // 상대 좌표
        Vector3 myPosition = new Vector3(0, 0, 0);
        // 절대 좌표
        //Vector3 myPosition = Player.transform.position;
        //NetworkManager.MyPosition.Position = myPosition;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _playerSpeed = _runSpeed;
        }
        else
        {
            _playerSpeed = _walkSpeed;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            switch (_playerIndex)
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
        if (Input.GetKey(KeyCode.RightArrow))
        {
            switch (_playerIndex)
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
        if (Input.GetKey(KeyCode.UpArrow))
        {
            switch (_playerIndex)
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
        if (Input.GetKey(KeyCode.DownArrow))
        {
            switch (_playerIndex)
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
            // 움직임 변경
            NetworkManager.MyPosition.Position = myPosition;
            NetworkManager.Send("relative_position", NetworkManager.MyPosition);

            // 자신의 캐릭터를 움직임
            //Move(myPosition);

            _isMoved = false;
        }

        //NetworkManager.Send("relative_position", NetworkManager.MyPosition);
    }

    public static void InputAbsolutePostion()
    {
        Packet.PersonalPosition myPosition = new Packet.PersonalPosition(PlayerIndex);
        myPosition.Position = Players[PlayerIndex].transform.position;
        NetworkManager.Send("absolute_position", myPosition);
    }

    // 자신의 캐릭터를 움직임
    private static void Move(Vector3 movingPosition)
    {
        Players[_playerIndex].transform.Translate(movingPosition);
    }

    // 서버로부터 타 플레이어의 캐릭터 움직임을 전달받아 움직임
    public static void Move(Packet.PersonalPosition playerMotionFromServer)
    {
        Vector3 movingPosition = playerMotionFromServer.Position;
        //Vector3 movingPosition = new Vector3(playerMotionFromServer.X, playerMotionFromServer.Y, playerMotionFromServer.Z);
        //상대
        Players[playerMotionFromServer.PlayerIndex].transform.Translate(movingPosition);
        //절대
        //Players[playerMotionFromServer.PlayerIndex].transform.position = movingPosition;
        //Lerp보간
        //Vector3 currentPosition = Players[playerMotionFromServer.PlayerIndex].transform.position;
        //Players[playerMotionFromServer.PlayerIndex].transform.position = Vector3.Lerp(currentPosition, movingPosition, Time.deltaTime * 30f);
    }

    // 서버로부터 모든 플레이어의 위치를 받아 한꺼번에 움직임
    public static void Move(Packet.PlayersPosition playersPositionFromServer, int type)
    {
        //Vector3 myMovingPosition = playersPositionFromServer.Positions[_playerIndex];
        //Players[_playerIndex].transform.position = myMovingPosition;

        if (type == Relative)
        {
            // 원래는 모두를 움직여주어야 함
            for (int i = 0; i < 4; i++)
            {
                Vector3 movingPosition = playersPositionFromServer.Positions[i];
                Players[i].transform.Translate(movingPosition);
            }
        }
        else if (type == Absolute)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector3 movingPosition = playersPositionFromServer.Positions[i];
                Players[i].transform.position = movingPosition;
            }
        }
    }

    public static void Destroy()
    {
        Players = null;
        Player = null;
    }

}