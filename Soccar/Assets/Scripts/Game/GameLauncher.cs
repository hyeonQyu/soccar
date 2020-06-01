using UnityEngine;

public class GameLauncher : MonoBehaviour
{
    // 네트워크
    [SerializeField]
    private GameObject _networkManagerObject;
    private NetworkManager _networkManager;

    // 씬 매개체
    private SceneMedium _sceneMedium;

    // 플레이어 움직임 보간에 사용
    public static RoutineScheduler RoutineScheduler { get; private set; }

    public static GameObject[] Balls = new GameObject[2];

    // Start is called before the first frame update
    void Start()
    {
        RoutineScheduler = GetComponent<RoutineScheduler>();
        PlayerController.SetPlayers();

        _networkManager = _networkManagerObject.GetComponent<NetworkManager>();
        _networkManager.SetWebSocket(true, GameObject.Find("Scene Medium").GetComponent<SceneMedium>());
        PlayerController.NetworkManager = _networkManager;

        PlayerController.PlayerIndex = 99;

        Balls[0] = GameObject.Find("Ball1");
        Balls[1] = GameObject.Find("Ball2");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(NetworkManager.GetTimestamp());

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