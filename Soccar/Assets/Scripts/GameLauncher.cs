using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLauncher : MonoBehaviour
{
    private static bool _isClickedStart;
    public static bool IsClickedStart
    {
        set { _isClickedStart = value; }
    }

    [SerializeField]
    GameObject _waitingText;

    // Start is called before the first frame update
    void Start()
    {
        PlayerController.InitializePlayers();
        NetworkManager.SetWebSocket();
    }

    // Update is called once per frame
    void Update()
    {
        if(_isClickedStart)
        {
            Debug.Log("Start button clicked");

            // 게임 시작 버튼이 눌렸음을 서버에 전송
            NetworkManager.Send("start_button", "start");

            _isClickedStart = false;
            return;
        }

        // 게임 시작 Ack를 맨 처음에 제대로 수신하면
        if(NetworkManager.GameStart == "start" && PlayerController.PlayerIndex == 99)
        {
            // Player Index 요청
            NetworkManager.Send("request_player_index", "req");

            while(PlayerController.PlayerIndex == 99)
            {
                Debug.Log("==수신 대기중..."); // Player Index를 서버로부터 수신할 때까지 대기
            }
            Debug.Log("==인덱스 받음, 초기화 할차례");

            // 플레이어 초기화
            PlayerController.SetPlayer();
            return;
        }

        PlayerController.KeyDowned();
    }
}
