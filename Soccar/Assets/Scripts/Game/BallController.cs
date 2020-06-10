using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Animator _collisionAnimator;
    private Collider _ball;
    private Rigidbody _rigidBody;
    private float _shootSpeed;
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
        _shootSpeed = 500.0f;
        _ball = GetComponent<Collider>();
        _rigidBody = GetComponent<Rigidbody>();
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
        GameObject other = collision.gameObject;

        // 땅에 부딪히면 탄성 감소
        if(other.CompareTag("Ground"))
        {
            _ball.material.bounciness *= 0.8f;
            return;
        }
        // 그물은 공을 튀기지 않기 때문에 그물 위에 올라가면 공을 내려줘야 함
        else if(other.CompareTag("Net"))
        {
            _ball.material.bounciness *= 0.01f;
            _isOnNet = true;
            StartCoroutine(MoveBallOnNet());
            return;
        }
        else if(other.CompareTag("Fense"))
        {
            _ball.material.bounciness = 1;
        }

        // 다른 물체에 부딪히면 탄성을 원래대로
        _ball.material.bounciness = 0.8f;

        if(LayerMask.LayerToName(other.layer).Equals("RagDoll"))
        {
            _rigidBody.velocity = Vector3.zero;
        }
    }

    private void OnCollisionStay(Collision collision) 
    {
        GameObject collisionObject = collision.gameObject;

        if(collisionObject.layer.Equals(LayerMask.NameToLayer("RagDoll")))
        {
            _collisionAnimator = collisionObject.transform.root.GetChild(0).gameObject.GetComponent<Animator>();

            Debug.Log("D 눌렀을 때 민감도 = " + Input.GetAxis("Sensitivity"));

            // 드리블
           _rigidBody.velocity = collision.transform.forward * _collisionAnimator.GetFloat("SpeedFloat") * 5.0f;

            // 슛
            if(_collisionAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == PlayerController.Hash.Shoot && collision.gameObject.name.Equals("RightLeg"))
            {
                Debug.Log("(Shoot)충돌한 오브젝트 = " + collision.gameObject.name);
                // 살짝 위로 올라가도록
                _rigidBody.AddForce(collision.transform.root.GetChild(0).forward * _shootSpeed + new Vector3(0, 3, 0));
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Net"))
        {
            _isOnNet = false;
        }

        // 어떤 플레이어의 득점인지 판단하기 위해 가장 마지막에 접촉한 플레이어를 저장해야 함(레그돌과 부딪힘으로 바꿔야댐)
        if(LayerMask.LayerToName(collision.gameObject.layer).Equals("RagDoll"))
        {
            _lastPlayer = collision.transform.root.gameObject;
            Debug.Log(transform.gameObject.name + "의 마지막 플레이어 = " + collision.transform.root.gameObject.name);
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

        // 공이 4초 후에도 그물 위에 있으면 탄성 복구 후 중앙으로 공 이동
        if(_isOnNet)
        {
            _ball.material.bounciness = 0.8f;
            transform.position = new Vector3(0, 10, 0);
        }
    }
}
