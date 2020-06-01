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
    public static GameObject Player { get; private set; }
    public static GameObject AlterEgo { get; private set; }

    // 속도
    private static float _walkSpeed;
    private static float _runSpeed;
    private static float _playerSpeed;

    public static NetworkManager NetworkManager { private get; set; }

    // 99 = 서버로 부터 값을 받지 않음.
    public static int PlayerIndex { get; set; }

    // 움직임 발생시 true로 변환하여 서버로 패킷전송
    private static bool _isMoved = false;
    public static bool IsPlayersInitialized { get; private set; }

    public static void SetPlayers()
    {
        _walkSpeed = 10;
        _runSpeed = _walkSpeed * 2;

        Players = new GameObject[4];
        Players[0] = GameObject.Find("Player1");
        Players[1] = GameObject.Find("Player2");
        Players[2] = GameObject.Find("Player3");
        Players[3] = GameObject.Find("Player4");

        AlterEgo = GameObject.Find("Alter Ego");
    }

    public static void InitializePlayer(string playerName)
    {
        Player = Players[PlayerIndex];
        AlterEgo.transform.position = Player.transform.position;
        Player.GetComponent<PlayerInformation>().PlayerName = playerName;

        IsPlayersInitialized = true;
    }

    public static void InputRelativePosition()
    {
        // 상대 좌표
        Vector3 myPosition = new Vector3(0, 0, 0);

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
            switch (PlayerIndex)
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
            switch (PlayerIndex)
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
            switch (PlayerIndex)
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
            switch (PlayerIndex)
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
            // 자신의 분신을 움직임
            Move(myPosition);

            _isMoved = false;
        }
    }

    // 공 + 절대좌표 전송
    public static void InputAbsolutePostion()
    {
        Packet.SendingAbsolutePositions sendingAbsolutePositions = new Packet.SendingAbsolutePositions(PlayerIndex);
        sendingAbsolutePositions.PlayerPosition = AlterEgo.transform.position;
        //sendingAbsolutePositions.PlayerPosition = Players[PlayerIndex].transform.position;
        sendingAbsolutePositions.BallPositions[0] = GameLauncher.Balls[0].transform.position;
        sendingAbsolutePositions.BallPositions[1] = GameLauncher.Balls[1].transform.position;

        NetworkManager.Send<Packet.SendingAbsolutePositions>("absolute_position", sendingAbsolutePositions);
    }

    // 자신의 분신을 움직임
    private static void Move(Vector3 movingPosition)
    {
        AlterEgo.transform.Translate(movingPosition);
        //Players[_playerIndex].transform.Translate(movingPosition);
    }

    // 서버로부터 모든 플레이어의 위치를 받아 한꺼번에 움직임
    public static void Move(Vector3[] playersPositionsFromServer, int type)
    {
        //Vector3 myMovingPosition = playersPositionFromServer.Positions[_playerIndex];
        //Players[_playerIndex].transform.position = myMovingPosition;

        if(type == Relative)
        {
            // 원래는 모두를 움직여주어야 함
            for(int i = 0; i < 4; i++)
            {
                if(i == PlayerIndex)
                    continue;
                Vector3 movingPosition = playersPositionsFromServer[i];
                Players[i].transform.Translate(movingPosition);
            }
        }
        else if(type == Absolute)
        {
            for(int i = 0; i < 4; i++)
            {
                Vector3 movingPosition = playersPositionsFromServer[i];
                Vector3.Lerp(Players[i].transform.position, movingPosition, 1);
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