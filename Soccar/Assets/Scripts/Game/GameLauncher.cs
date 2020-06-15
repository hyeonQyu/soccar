using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameLauncher : MonoBehaviour
{
    [SerializeField]
    private GameObject _loadingGameScenePanel;
    [SerializeField]
    private GameObject[] _scoreBoard;
    [SerializeField]
    private GameObject _moveLobbyButton;
    [SerializeField]
    private GameObject _gameResult;

    // 네트워크
    [SerializeField]
    private GameObject _networkManagerObject;
    private NetworkManager _networkManager;

    // 사운드 관리
    public static Sound Sound { get; set; }
    public static bool IsCrowdScream { get; set; }
    private Coroutine _crowdScreamCoroutine;

    // 씬 매개체
    private SceneMedium _sceneMedium;

    // 시간 표시(초 단위, 5분)
    [SerializeField]
    private Text _txtTime;
    private float _time = 300;
    private int _seconds = 300;

    public static bool IsReadyToKickOff { get; set; }
    private bool _isKickOff;
    public static bool IsEndGame { get; set; }
    private bool _isEndGameComponentsActivated;
    public static bool IsWinner { get; set; }
    public static bool IsFeverTime { get; set; }

    // 플레이어 움직임 보간에 사용
    public static RoutineScheduler RoutineScheduler { get; private set; }

    public static GameObject[] Balls { get; private set; }

    public static int Headcount { get; set; }
    private const int MaxPlayer = 6;

    // Start is called before the first frame update
    void Start()
    {
        _sceneMedium = GameObject.Find("Scene Medium").GetComponent<SceneMedium>();
        Headcount = _sceneMedium.Headcount;
        BallController.ShootPowers = new float[Headcount];

        RoutineScheduler = GetComponent<RoutineScheduler>();
        PlayerController.SetPlayers();       

        // 스코어보드 배치
        int n = Headcount - 1;
        for(int i = 0; i < Headcount; i++)
        {
            Debug.Log("스코어보드 배치: " + n + " " + i);
            _scoreBoard[i].transform.localPosition = new Vector3((n - i) * -100 + 300, 0, 0);
        }
        for(int i = Headcount; i < MaxPlayer; i++)
        {
            Destroy(_scoreBoard[i]);
        }

        _moveLobbyButton.SetActive(false);
        _gameResult.SetActive(false);

        // 네트워크 설정
        _networkManager = _networkManagerObject.GetComponent<NetworkManager>();
        _networkManager.SetWebSocket(_sceneMedium, _scoreBoard);
        PlayerController.NetworkManager = _networkManager;

        Sound = new Sound(true);

        PlayerController.PlayerIndex = 99;

        // 공 객체를 찾음
        Balls = new GameObject[BallController.TotalBallCount];
        BallController.CurrentBallCount = BallController.TotalBallCount - 1;
        for(int i = 0; i < BallController.TotalBallCount; i++)
        {
            Balls[i] = GameObject.Find("Ball" + i);

            BallController ballController = Balls[i].GetComponent<BallController>();
            ballController.IsFeverBall = false;
            ballController.BallIndex = i;

            PlayerController.MiniMapManager.Balls[i] = PlayerController.MiniMapManager.MiniMap.transform.Find("Mini Map Ball" + i).gameObject;
        }
        Balls[BallController.CurrentBallCount].GetComponent<BallController>().IsFeverBall = true;
        Balls[BallController.CurrentBallCount].SetActive(false);
        PlayerController.MiniMapManager.Balls[BallController.CurrentBallCount].SetActive(false);
    }

    void FixedUpdate()
    {
        //Debug.Log(NetworkManager.GetTimestamp());
        // 소켓 연결시까지 대기
        if(!_networkManager.Socket.IsConnected)
            return;

        if(!PlayerController.IsPlayersInitialized)
        {
            // Player Index 세팅
            if(PlayerController.PlayerIndex == 99)
            {
                _networkManager.Send("player_index", _sceneMedium.PlayerIndex.ToString());
                return;
            }

            // Player Index를 받은 후 플레이어 초기화
            PlayerController.InitializePlayer(_sceneMedium.PlayerName);
            PlayerController.InitializeGoalPost();
            return;
        }

        // 로딩 씬이 사라질 때까지 대기
        try
        {
            if(_loadingGameScenePanel.transform.localScale.x != 0)
                return;
            Destroy(_loadingGameScenePanel);

            // 로딩이 완료되었음을 서버에 알림
            Packet.SendingCompleteLoading sendingCompleteLoading = new Packet.SendingCompleteLoading(PlayerController.PlayerIndex, _sceneMedium.PlayerName);
            _networkManager.Send<Packet.SendingCompleteLoading>("complete_loading", sendingCompleteLoading);
        }
        catch(Exception e) { }

        if(!IsReadyToKickOff)
            return;

        // 경기 시작 휘슬을 울림
        if(!_isKickOff)
        {
            Sound.Whistle.Play();
            _isKickOff = true;
            Sound.CrowdDefault.Play();
            return;
        }

        if(IsEndGame)
        {
            if(!_isEndGameComponentsActivated)
                Invoke("ActiveEndGameComponents", 2);
            return;
        }

        // 경기 시작 휘슬이 울린 후 타이머 시작
        if(_seconds >= 0)
        {
            _time -= Time.fixedDeltaTime;
            _seconds = (int)Math.Round(_time);
            _txtTime.text = (_seconds / 60).ToString() + ":" + ToDoubleDigit((_seconds % 60).ToString());
            if(IsFeverTime)
            {
                _txtTime.color = new Color(1, 0.25f, 0.1f);
                IsFeverTime = false;
            }
        }

        // 움직임 입력
        PlayerController.InputRelativePosition();
        PlayerController.InputAbsolutePostion();
    }

    // 게임 결과 및 로비로 이동 버튼 표시
    private void ActiveEndGameComponents()
    {
        if(!_moveLobbyButton.activeSelf)
        {
            PlayerController.MiniMapManager.MiniMap.SetActive(false);

            Image gameResultImage = _gameResult.GetComponent<Image>();
            if(IsWinner)
            {
                gameResultImage.sprite = Resources.Load<Sprite>("UI/Victory");
                Sound.Victory.Play();
            }
            else
            {
                gameResultImage.sprite = Resources.Load<Sprite>("UI/Defeat");
                Sound.Defeat.Play();
            }

            _gameResult.SetActive(true);
            _moveLobbyButton.SetActive(true);
            _isEndGameComponentsActivated = true;

            _isEndGameComponentsActivated = true;
        }
    }

    public static void InitializeStatic()
    {
        Headcount = 0;
        IsReadyToKickOff = false;
        IsWinner = false;
        IsEndGame = false;
        IsCrowdScream = false;
        IsFeverTime = false;
    }

    private string ToDoubleDigit(string str)
    {
        return str.Length == 2 ? str : "0" + str;
    }

    private void OnApplicationQuit()
    {
        _networkManager.Send("disconnection", "");
    }
}