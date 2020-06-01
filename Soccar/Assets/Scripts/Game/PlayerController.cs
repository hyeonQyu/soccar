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

    public static float theta { get; private set; }

    public static Vector3 rightVector { get; private set; }
    public static Vector3 leftVector { get; private set; }
    public static Vector3 forwardVector { get; private set; }
    public static Vector3 backwardVector { get; private set; }

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

        Players = new GameObject[GameLauncher.Headcount];
        for(int i =0; i < 6; i++)
        {
            if (i < GameLauncher.Headcount)
            {
                Players[i] = GameObject.Find("Player" + i.ToString());
            }
            else
            {
                GameObject.Find("Player" + i.ToString()).SetActive(false);
            }
        }

        AlterEgo = GameObject.Find("Alter Ego");
    }

    public static void InitializePlayer(string playerName)
    {
        // set direction vector
        theta = (360f / GameLauncher.Headcount) * PlayerIndex;
        
        rightVector = new Vector3(Mathf.Cos(theta), 0, Mathf.Sign(theta));
        leftVector = new Vector3(Mathf.Cos(theta+180), 0, Mathf.Sign(theta+180));
        forwardVector = new Vector3(Mathf.Cos(90+theta), 0, Mathf.Sign(90+theta));
        backwardVector = new Vector3(Mathf.Cos(270+theta), 0, Mathf.Sign(270+theta));

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
            myPosition += (leftVector * _playerSpeed * Time.deltaTime);

            _isMoved = true;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            myPosition += (rightVector * _playerSpeed * Time.deltaTime);

            _isMoved = true;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            myPosition += (forwardVector * _playerSpeed * Time.deltaTime);

            _isMoved = true;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            myPosition += (backwardVector * _playerSpeed * Time.deltaTime);

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
            for(int i = 0; i < GameLauncher.Headcount; i++)
            {
                if(i == PlayerIndex)
                    continue;
                Vector3 movingPosition = playersPositionsFromServer[i];
                Players[i].transform.Translate(movingPosition);
            }
        }
        else if(type == Absolute)
        {
            for(int i = 0; i < GameLauncher.Headcount; i++)
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