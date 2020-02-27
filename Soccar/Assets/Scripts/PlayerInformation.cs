using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour
{
    private string _id;
    public string ID
    {
        set
        {
            _id = value;
        }
        get
        {
            return _id;
        }
    }
    private int _score = 0;
    public int Score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
        }
    }

    // 득점
    public void Scores(ref GameObject conceder)
    {
        PlayerInformation concederPlayer = conceder.GetComponent<PlayerInformation>();

        // 득점자는 +2점, 실점자는 -1점
        if(concederPlayer.ID != _id)
        {
            _score += 2;
            // 득점에 대한 메시지 
            Debug.Log("득점자: " + _id + "   실점자: " + concederPlayer.ID);
        }
        // 자책골, 자책골은 득점으로 인정하지 않음
        else
        {
            // 자책골에 대한 메시지
            Debug.Log(_id + "의 자책골");
        }
        concederPlayer.Score--;

        Debug.Log(_id + ": " + _score + "    " + concederPlayer.ID + ": " + concederPlayer.Score);
    }
}
