using UnityEngine;

public class GameLauncher : MonoBehaviour
{
    [SerializeField]
    private GameObject _loadingGameScenePanel;

    // 네트워크
    [SerializeField]
    private GameObject _networkManagerObject;
    private NetworkManager _networkManager;

    // 씬 매개체
    private SceneMedium _sceneMedium;

    // 플레이어 움직임 보간에 사용
    public static RoutineScheduler RoutineScheduler { get; private set; }

    public static GameObject[] Balls = new GameObject[2];

    public static int Headcount;

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

        PlayerController.PlayerIndex = 99;

        Balls[0] = GameObject.Find("Ball1");
        Balls[1] = GameObject.Find("Ball2");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(NetworkManager.GetTimestamp());
        // 로딩 씬이 사라질 때까지 대기
        if(_loadingGameScenePanel.transform.localScale.x != 0)
            return;
        Destroy(_loadingGameScenePanel);

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
            return;
        }

        // 움직임 입력
        PlayerController.InputRelativePosition();
        PlayerController.InputAbsolutePostion();
    }
}