using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour
{
    public string PlayerName { get; set; }
    public int Score { get; set; }

    private Vector3 _lastReceivedPosition;
    public Vector3 LastReceivedPosition
    {
        set
        {
            _lastReceivedPosition = value;
        }
    }

    // 득점
    public void Scores(ref GameObject conceder)
    {
        PlayerInformation concederPlayer = conceder.GetComponent<PlayerInformation>();

        // 득점자는 +2점, 실점자는 -1점
        if(!concederPlayer.PlayerName.Equals(PlayerName))
        {
            Score += 2;
            // 득점에 대한 메시지 
            Debug.Log("득점자: " + PlayerName + "   실점자: " + concederPlayer.PlayerName);
        }
        // 자책골, 자책골은 득점으로 인정하지 않음
        else
        {
            // 자책골에 대한 메시지
            Debug.Log(PlayerName + "의 자책골");
        }
        concederPlayer.Score--;

        Debug.Log(PlayerName + ": " + Score + "    " + concederPlayer.PlayerName + ": " + concederPlayer.PlayerName);
    }

    // 캐릭터에 충돌 발생 시 분신을 캐릭터 위치로 옮김
    private void OnCollisionEnter(Collision collision)
    {
        if(gameObject.GetInstanceID() != PlayerController.Player.GetInstanceID())
            return;

        GameObject other = collision.gameObject;

        if(other.tag.Equals("Ground") || other.tag.Equals("Ball"))
            return;

        PlayerController.AlterEgo.transform.position = gameObject.transform.position;
        Debug.Log("Collision " + other.tag);
    }
}
