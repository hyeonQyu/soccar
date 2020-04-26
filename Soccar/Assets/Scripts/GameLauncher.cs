using UnityEngine;

public class GameLauncher:MonoBehaviour
{
    private static bool _isClickedStart;
    public static bool IsClickedStart
    {
        set { _isClickedStart = value; }
    }

    [SerializeField]
    GameObject _waitingText;

    private int _frameCount;

    // Start is called before the first frame update
    void Start()
    {
        PlayerController.SetPlayers();
        NetworkManager.SetWebSocket();
        _frameCount = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(NetworkManager.GetTimestamp());

        if(_isClickedStart)
        {
            Debug.Log("==시작 버튼 눌림");

            // 게임 시작 버튼이 눌렸음을 서버에 전송
            NetworkManager.Send("start_button", "start");

            _isClickedStart = false;
            NetworkManager.RequestPlayerIndex = "req";
            return;
        }

        // 게임 시작 Ack를 맨 처음에 제대로 수신하면
        if(NetworkManager.GameStart == "start" && PlayerController.PlayerIndex == 99)
        {
            Debug.Log("==플레이어 인덱스 주세요");
            // Player Index 요청
            NetworkManager.Send("request_player_index", NetworkManager.RequestPlayerIndex);

            NetworkManager.RequestPlayerIndex = "";
            return;
        }

        // 인덱스를 받은 후
        if(PlayerController.PlayerIndex != 99 && !PlayerController.IsPlayerInitialized)
        {
            Debug.Log("==인덱스 받음, 초기화 할차례");

            // 플레이어 초기화
            PlayerController.InitializePlayer();
            return;
        }

        /* 게임을 시작하기 전에 선수들이 등장하는 동안 여러번 RTT를 계산하여 평균을 내고
         * 평균 RTT가 가장 짧은 클라이언트를 메인 시뮬레이터로 사용할 예정 */
        PlayerController.InputRelativePosition();
        //PlayerController.InputAbsolutePostion();

    }

    private void OnDestroy()
    {
        ButtonControl.Destroy();
        NetworkManager.Destroy();
        PlayerController.Destroy();
    }
}
