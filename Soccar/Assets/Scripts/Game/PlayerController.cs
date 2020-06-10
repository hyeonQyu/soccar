using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerController
{
    public const int Jump = 0;
    public const int Tackle = 1;
    public const int Shoot = 2;

    // 전체 플레이어
    public static GameObject[] Players { get; set; }
    public static PlayerInformation[] PlayerInformations { get; set; }
    // 현재 컨트롤하는 플레이어
    public static GameObject Player { get; private set; }
    public static GameObject AlterEgo { get; private set; }
    public static MiniMapManager MiniMapManager { get; set; }
    public static GameObject[] GoalPosts { get; set; }

    // 현재 접속중인 플레이어
    public static bool[] IsConnectPlayers { get; set; }
    private static bool _isDash;

    // 속도
    private static float _speed;
    private static float _playerSpeed;
    public static float Theta { get; private set; }

    public static Vector3 RightVector { get; private set; }
    public static Vector3 LeftVector { get; private set; }
    public static Vector3 ForwardVector { get; private set; }
    public static Vector3 BackwardVector { get; private set; }

    public static NetworkManager NetworkManager { private get; set; }

    // 99 = 서버로 부터 값을 받지 않음.
    public static int PlayerIndex { get; set; }
    public static int SuperClientIndex { get; set; }

    // 움직임 발생시 true로 변환하여 서버로 패킷전송
    private static bool _isMoved = false;
    public static bool IsPlayersInitialized { get; private set; }

    // 플레이어 컴포넌트
    [HideInInspector] public static bool InhibitRun { get; set; }    // Set from RagdollControl
    public static bool InhibitMove { get; set; }     // Set from RagdollControl
    public static float AnimatorSpeed = 1.3f;   // Read by RagdollControl
	public static Animator PlayerAnimator { get; private set; }			// Reference to the animator component.
	public static AnimFollow.HashIDs_AF Hash;			// Reference to the HashIDs.
    public static readonly int version = 7; // The version of this script
    public static void SetPlayers()
    {
        _speed = 2.5f;
        _playerSpeed = 0;
        //_rotationSpeed = 2.5f;

        Players = new GameObject[GameLauncher.Headcount];
        PlayerInformations = new PlayerInformation[GameLauncher.Headcount];
        MiniMapManager = new MiniMapManager(GameLauncher.Headcount);
        IsConnectPlayers = new bool[GameLauncher.Headcount];
        for(int i = 0; i < 6; i++)
        {
            string suffix = i.ToString();

            // 접속한 플레이어 수 만큼만 게임오브젝트와 연결
            if (i < GameLauncher.Headcount)
            {
                Players[i] = GameObject.Find("Player" + suffix);
                PlayerInformations[i] = Players[i].GetComponent<PlayerInformation>();
                PlayerInformations[i].SetPlayerInformation(i);
                MiniMapManager.Players[i] = MiniMapManager.MiniMap.transform.Find("Mini Map Player" + suffix).gameObject;
                IsConnectPlayers[i] = true;

                // Avatar를 가리킴
                Players[i] = Players[i].transform.GetChild(0).gameObject;
            }
            else
            {
                GameObject.Find("Player" + suffix).SetActive(false);
                UnityEngine.Object.Destroy(MiniMapManager.MiniMap.transform.Find("Mini Map Player" + suffix).gameObject);
                try
                {
                    IsConnectPlayers[i] = false;
                }
                catch(IndexOutOfRangeException e) { }
            }
        }

        AlterEgo = GameObject.Find("Alter Ego");

        // 슈퍼 클라이언트의 인덱스를 찾음
        for(int i = 0; i < GameLauncher.Headcount; i++)
        {
            if(!IsConnectPlayers[i])
                continue;
            
            SuperClientIndex = i;
            break;
        }
    }

    public static void InitializePlayer(string playerName)
    {
        // set direction vector
        Theta = (360f / GameLauncher.Headcount) * PlayerIndex;
        Debug.Log("theta = " + Theta);

        float piTheta = Theta * Mathf.PI / 180;

        RightVector = new Vector3(Mathf.Cos(piTheta), 0, Mathf.Sin(piTheta));   // (1,0,0)vector 회전
        LeftVector = new Vector3(-Mathf.Cos(piTheta), 0, -Mathf.Sin(piTheta));  // (-1,0,0)vector 회전
        ForwardVector = new Vector3(-Mathf.Sin(piTheta), 0, Mathf.Cos(piTheta));  // (0,0,1)vector 회전
        BackwardVector = new Vector3(Mathf.Sin(piTheta), 0, -Mathf.Cos(piTheta));  // (0,0,-1)vector 회전

        Debug.Log("r=" + RightVector + " l=" + LeftVector + " f=" + ForwardVector + " b=" + BackwardVector);

        Player = Players[PlayerIndex];
        PlayerInformations[PlayerIndex].PlayerName = playerName;
        AlterEgo.transform.position = Player.transform.position;
        
        // Set Animation
        PlayerAnimator = Player.GetComponent<Animator>();
        Hash = Player.GetComponent<AnimFollow.HashIDs_AF>();

        IsPlayersInitialized = true;
    }

    public static void InitializeGoalPost()
    {
        GoalPosts = new GameObject[GameLauncher.Headcount];
        float theta = 360 / GameLauncher.Headcount;
        float piTheta;
        for (int i = 0; i < 6; i++)
        {
            string suffix = i.ToString();

            if (i < GameLauncher.Headcount)
            {
                GoalPosts[i] = GameObject.Find("Goal Post" + suffix);
                piTheta = theta * Mathf.PI / 180 * i;
                Vector3 backwardVector = new Vector3(Mathf.Sin(piTheta), 0, -Mathf.Cos(piTheta)); // 현재 골대의 back_ward 방향벡터 구하기
                GoalPosts[i].transform.position = backwardVector * 8.7f;
                GoalPosts[i].transform.eulerAngles = new Vector3(0, -theta * i, 0);

                // Set Player Position & Rotation (골대 위치 초기화하는 김에 플레이어도 같이 함)
                Players[i].transform.parent.transform.position = backwardVector * 6 + new Vector3(0, 0, 0);
                Players[i].transform.eulerAngles = new Vector3(0, -theta * i, 0);
            }
            else
            {
                UnityEngine.Object.Destroy(GameObject.Find("Goal Post" + suffix));
            }
        }
        AlterEgo.transform.position = Player.transform.position;
        AlterEgo.transform.eulerAngles = Player.transform.eulerAngles;
    }

    public static void InputRelativePosition()
    {
        if(InhibitMove)
            return;

        // 현재 태클 상태이면 분신을 움직이지 못하도록 함
        int animHash = PlayerAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        if(animHash == Hash.Tackle)
        {
            return;
        }
        
        // 상대 좌표
        Vector3 myPosition = Vector3.zero;
        Vector3 direction = Vector3.zero;

        _isDash = false;
        if (Input.GetKey(KeyCode.E))
        {
            _isDash = true;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            myPosition += RightVector;

            _isMoved = true;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            myPosition += LeftVector;

            _isMoved = true;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            myPosition += ForwardVector;

            _isMoved = true;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            myPosition += BackwardVector;

            _isMoved = true;
        }

        direction = myPosition;
        myPosition = myPosition.normalized;

        if (_isMoved)
        {
            // 자신의 분신을 움직임
            if (_isDash)
            {
                _playerSpeed = 5.0f;
            }
            else
            {
                _playerSpeed = 2.5f;
            }
            myPosition = myPosition * _playerSpeed * Time.fixedDeltaTime;
            Move(myPosition);

            _isMoved = false;
        }
        else
        {
            _playerSpeed = 0;
        }

        // Player 방향
        if (direction != Vector3.zero)
        {
            Player.transform.rotation = Quaternion.LookRotation(direction.normalized);
        }

        PlayerAnimator.SetFloat(Hash.SpeedFloat, (_playerSpeed / 5f), 0.1f, Time.fixedDeltaTime);

        // 현재 state에 있음
        if(!PlayerAnimator.IsInTransition(0))
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                PlayerAnimator.SetTrigger(Hash.JumpTrigger);  
            }

            else if (Input.GetKeyDown(KeyCode.A))
            {
                PlayerAnimator.SetTrigger(Hash.TackleTrigger);
            }

            else if (Input.GetKeyDown(KeyCode.D))
            {
                PlayerAnimator.SetTrigger(Hash.ShootTrigger);
            }
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
        // 경기장 밖을 벗어나면 움직이지 않음
        if(IsOutOfStadium(AlterEgo.transform.position, movingPosition))
            return;
        
        AlterEgo.transform.position += movingPosition;
        
        //Players[_playerIndex].transform.Translate(movingPosition);
    }

    public static bool IsOutOfStadium(Vector3 curPosition, Vector3 movingPosition)
    {
        Vector2 alterEgoVector2 = new Vector2(curPosition.x, curPosition.z);
        Vector2 movingVector2 = new Vector2(movingPosition.x, movingPosition.z);

        double radius = Vector2.Distance(alterEgoVector2 + movingVector2, new Vector2(0, 0));
        if(radius > 11.1f)
            return true;

        return false;
    }

    public static void Destroy()
    {
        Players = null;
        Player = null;
    }
}