using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Collider _ball;
    public bool IsFeverBall { get; set; }
    private GameObject _lastPlayer;
    public GameObject LastPlayer
    {
        get
        {
            return _lastPlayer;
        }
    }
    private bool _isOnNet;
    private bool _isScored;
    public bool IsScored
    {
        set
        {
            _isScored = value;
        }
        get
        {
            return _isScored;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _ball = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Net"))
        {
            _ball.material.bounciness *= 0.01f;
        }
    }

    // 실제 축구공처럼 튀기게 하기 위함
    private void OnCollisionEnter(Collision collision)
    {
        // 땅에 부딪히면 탄성 감소
        if(collision.gameObject.CompareTag("Ground"))
        {
            _ball.material.bounciness *= 0.8f;
            return;
        }
        // 그물은 공을 튀기지 않기 때문에 그물 위에 올라가면 공을 내려줘야 함
        else if(collision.gameObject.CompareTag("Net"))
        {
            _ball.material.bounciness *= 0.01f;
            _isOnNet = true;
            StartCoroutine(MoveBallOnNet());
            return;
        }

        // 어떤 플레이어의 득점인지 판단하기 위해 가장 마지막에 접촉한 플레이어를 저장해야 함
        if(collision.gameObject.CompareTag("Player"))
        {
            _lastPlayer = collision.gameObject;
        }
        // 다른 물체에 부딪히면 탄성을 원래대로
        _ball.material.bounciness = 0.8f;
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Net"))
        {
            _isOnNet = false;
        }
    }

    private IEnumerator MoveBallOnNet()
    {
        int loop = 0;
        // 4초간 대기하며 공이 그물 위에 있는지 검사
        while(_isOnNet && loop < 8)
        {
            yield return new WaitForSeconds(0.5f);
            loop++;
        }

        // 공이 4초 후에도 그물 위에 있으면 탄성 복구 후 랜덤 위치로 공 이동
        if(_isOnNet)
        {
            _ball.material.bounciness = 0.8f;
            transform.position = new Vector3(Random.Range(-25f, 25f), 30, Random.Range(-25f, 2f));
        }
    }
}
