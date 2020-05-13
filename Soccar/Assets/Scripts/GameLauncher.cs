using UnityEngine;

public class GameLauncher : MonoBehaviour
{
    private NetworkManager _networkManager;
    public static RoutineScheduler RoutineScheduler { get; private set; }

    private static bool _isClickedStart;
    public static bool IsClickedStart
    {
        set { _isClickedStart = value; }
    }

    [SerializeField]
    private GameObject _waitingText;

    private int _frameCount;

    public static GameObject[] Balls = new GameObject[2];

    public static int Frame = 0;

    // Start is called before the first frame update
    void Start()
    {
        RoutineScheduler = GetComponent<RoutineScheduler>();
        PlayerController.SetPlayers();
        _networkManager = new NetworkManager(true);
        _frameCount = 0;

        Balls[0] = GameObject.Find("Ball1");
        Balls[1] = GameObject.Find("Ball2");
    }

    // Update is called once per frame
    void Update()
    {
        Frame++;
        //Debug.Log(NetworkManager.GetTimestamp());

        if (_isClickedStart)
        {
            Debug.Log("==시작 버튼 눌림");

            // 게임 시작 버튼이 눌렸음을 서버에 전송
            NetworkManager.Send("start_button", "start");

            _isClickedStart = false;
            NetworkManager.RequestPlayerIndex = "req";
            return;
        }

        // 게임 시작 Ack를 맨 처음에 제대로 수신하면
        if (NetworkManager.GameStart == "start" && PlayerController.PlayerIndex == 99)
        {
            Debug.Log("==플레이어 인덱스 주세요");
            // Player Index 요청
            NetworkManager.Send("request_player_index", NetworkManager.RequestPlayerIndex);

            NetworkManager.RequestPlayerIndex = "";
            return;
        }

        // 인덱스를 받은 후
        if (PlayerController.PlayerIndex != 99 && !PlayerController.IsPlayerInitialized)
        {
            Debug.Log("==인덱스 받음, 초기화 할차례");

            // 플레이어 초기화
            PlayerController.InitializePlayer();
            return;
        }

        if (PlayerController.IsPlayerInitialized)
        {
            /* 게임을 시작하기 전에 선수들이 등장하는 동안 여러번 RTT를 계산하여 평균을 내고
         * 평균 RTT가 가장 짧은 클라이언트를 메인 시뮬레이터로 사용할 예정 */
            PlayerController.InputRelativePosition();
            PlayerController.InputAbsolutePostion();

            //// 호스트만 공의 위치 전송
            //if(PlayerController.PlayerIndex == 0)
            //{
            //    for(int i = 0; i < Balls.Length; i++)
            //    {
            //        NetworkManager.BallsPosition.Positions[i] = Balls[i].transform.position;
            //    }
            //    Debug.Log("공 전송");
            //    NetworkManager.Send<Packet.BallsPosition>("ball_position", NetworkManager.BallsPosition);
            //}
        }
    }

    private void OnDestroy()
    {
        ButtonControl.Destroy();
        NetworkManager.Destroy();
        PlayerController.Destroy();
    }
}