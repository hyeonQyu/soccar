using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour
{
    [SerializeField]
    private GameObject _networkManagerObject;
    private NetworkManager _networkManager;
    public string PlayerName { get; set; }
    public int Score { get; set; }
    public int PlayerIndex { get; private set; }

    private Vector3 _lastReceivedPosition;
    public Vector3 LastReceivedPosition
    {
        set
        {
            _lastReceivedPosition = value;
        }
    }

    public void SetPlayerInformation(int playerIndex)
    {
        PlayerIndex = playerIndex;
        _networkManager = _networkManagerObject.GetComponent<NetworkManager>();
    }

    // 득점
    public void Scores(ref GameObject conceder, bool isFeverBall)
    {
        PlayerInformation concederPlayer = conceder.GetComponent<PlayerInformation>();

        // 슈퍼 클라이언트만 득점 정보 서버에 전송
        if(PlayerController.PlayerIndex == PlayerController.SuperClientIndex)
        {
            Packet.SendingScore sendingScore = new Packet.SendingScore(PlayerIndex, concederPlayer.PlayerIndex, isFeverBall);
            _networkManager.Send<Packet.SendingScore>("score", sendingScore);
        }

        //// 득점자는 +2점, 실점자는 -1점
        //if(!concederPlayer.PlayerName.Equals(PlayerName))
        //{
        //    Score += 2;
        //    // 득점에 대한 메시지 
        //    Debug.Log("득점자: " + PlayerName + "   실점자: " + concederPlayer.PlayerName);
        //}
        //// 자책골, 자책골은 득점으로 인정하지 않음
        //else
        //{
        //    // 자책골에 대한 메시지
        //    Debug.Log(PlayerName + "의 자책골");
        //}
        //concederPlayer.Score--;

        //Debug.Log(PlayerName + ": " + Score + "    " + concederPlayer.PlayerName + ": " + concederPlayer.PlayerName);
    }

    // 캐릭터에 충돌 발생 시 분신을 캐릭터 위치로 옮김
    private void OnCollisionEnter(Collision collision)
    {
        try
        {
            if(gameObject.GetInstanceID() != PlayerController.Player.GetInstanceID())
                return;
        }
        catch(NullReferenceException e)
        {
            return;
        }

        GameObject other = collision.gameObject;

        if(other.tag.Equals("Ground") || other.tag.Equals("Ball"))
            return;

        PlayerController.AlterEgo.transform.position = gameObject.transform.position;
        Debug.Log("Collision " + other.tag);
    }
}
