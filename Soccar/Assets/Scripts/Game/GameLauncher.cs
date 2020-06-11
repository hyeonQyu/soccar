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

    public static bool IsReadyToKickOff;
    private bool _isKickOff;    

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

        // 네트워크 설정
        _networkManager = _networkManagerObject.GetComponent<NetworkManager>();
        _networkManager.SetWebSocket(_sceneMedium, _scoreBoard);
        PlayerController.NetworkManager = _networkManager;

        Sound = new Sound(true);

        PlayerController.PlayerIndex = 99;

        // 공 객체를 찾음
        Balls = new GameObject[2];
        for(int i = 0; i < Balls.Length; i++)
        {
            Balls[i] = GameObject.Find("Ball" + i);
            Balls[i].GetComponent<BallController>().IsFeverBall = false;
            PlayerController.MiniMapManager.Balls[i] = PlayerController.MiniMapManager.MiniMap.transform.Find("Mini Map Ball" + i).gameObject;
        }
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

        // 경기 시작 휘슬이 울린 후 타이머 시작
        _time -= Time.deltaTime;
        _seconds = (int)Math.Round(_time);
        _txtTime.text = (_seconds / 60).ToString() + ":" + ToDoubleDigit((_seconds % 60).ToString());

        // 움직임 입력
        PlayerController.InputRelativePosition();
        PlayerController.InputAbsolutePostion();
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