using System;
using UnityEngine;
using UnityEngine.UI;

public class GameLauncher : MonoBehaviour
{
    [SerializeField]
    private GameObject _loadingGameScenePanel;

    // 네트워크
    [SerializeField]
    private GameObject _networkManagerObject;
    private NetworkManager _networkManager;

    // 사운드 관리
    private Sound _sound;

    // 씬 매개체
    private SceneMedium _sceneMedium;

    // 시간 표시(초 단위, 5분)
    [SerializeField]
    private Text _txtTime;
    private float _time = 300;
    private int _seconds = 300;

    private bool _isKickOff;

    // 플레이어 움직임 보간에 사용
    public static RoutineScheduler RoutineScheduler { get; private set; }

    public static GameObject[] Balls { get; private set; }

    public static int Headcount { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        _sceneMedium = GameObject.Find("Scene Medium").GetComponent<SceneMedium>();
        Headcount = _sceneMedium.Headcount;

        RoutineScheduler = GetComponent<RoutineScheduler>();
        PlayerController.SetPlayers();

        _networkManager = _networkManagerObject.GetComponent<NetworkManager>();
        _networkManager.SetWebSocket(true, _sceneMedium);
        PlayerController.NetworkManager = _networkManager;

        _sound = new Sound(true);

        PlayerController.PlayerIndex = 99;

        Balls = new GameObject[2];
        Balls[0] = GameObject.Find("Ball0");
        Balls[1] = GameObject.Find("Ball1");
        PlayerController.MiniMapManager.Balls[0] = PlayerController.MiniMapManager.MiniMapGround.transform.Find("Mini Map Ball0").gameObject;
        PlayerController.MiniMapManager.Balls[1] = PlayerController.MiniMapManager.MiniMapGround.transform.Find("Mini Map Ball1").gameObject;
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
        }
        catch(Exception e) { }

        // 경기 시작 휘슬을 울림
        if(!_isKickOff)
        {
            _sound.Whistle.Play();
            _isKickOff = true;
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
}