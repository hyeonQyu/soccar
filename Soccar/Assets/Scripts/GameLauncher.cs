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
        PlayerController.SetPlayers();
        NetworkManager.SetWebSocket();
    }

    // Update is called once per frame
    void Update()
    {
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

        PlayerController.KeyDowned();
    }
}
