using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Animator _collisionAnimator;
    private Collider _ball;
    private Rigidbody _rigidBody;
    private float _shootSpeed;
    private bool _isShoot;
    private bool _isDribble;
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
    public bool IsScored { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        _shootSpeed = 20.0f;
        _ball = GetComponent<Collider>();
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Net"))
        {
            _ball.material.bounciness *= 0.01f;
        }
    }

    // 실제 축구공처럼 튀기게 하기 위함
    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;

        // 땅에 부딪히면 탄성 감소
        if (other.CompareTag("Ground"))
        {
            _ball.material.bounciness *= 0.8f;
            return;
        }
        // 그물은 공을 튀기지 않기 때문에 그물 위에 올라가면 공을 내려줘야 함
        else if (other.CompareTag("Net"))
        {
            _ball.material.bounciness *= 0.01f;
            _isOnNet = true;
            StartCoroutine(MoveBallOnNet());
            return;
        }
        else if (other.CompareTag("Fense"))
        {
            _ball.material.bounciness = 1;
        }

        // 다른 물체에 부딪히면 탄성을 원래대로
        _ball.material.bounciness = 0.8f;

        _isShoot = false;

        if (LayerMask.LayerToName(other.layer).Equals("RagDoll"))
        {
            // 마지막 사용자 기록
            _lastPlayer = collision.transform.root.gameObject;
            // 공을 멈추기 위해서
            _rigidBody.velocity = Vector3.zero;
            _collisionAnimator = _lastPlayer.transform.GetChild(0).gameObject.GetComponent<Animator>();

            // 슈팅
            if (_collisionAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == PlayerController.Hash.Shoot && collision.gameObject.name.Equals("RightLeg"))
            {
                _isShoot = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer).Equals("RagDoll"))
        {
            // 드리블
            _rigidBody.velocity = collision.transform.forward * _collisionAnimator.GetFloat("SpeedFloat") * 5.0f;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Net"))
        {
            _isOnNet = false;
        }

        if (LayerMask.LayerToName(collision.gameObject.layer).Equals("RagDoll") && _isShoot)
        {
            // 방향은 Avatar가 바라보는 방향
            Vector3 direction = collision.transform.root.GetChild(0).forward;
            // Debug.Log("Shoot!!");
            // 한 번만 실행하도록 위치 바꿔줌
            transform.position += direction * 0.5f;

            // 살짝 위로 올라가도록
            _rigidBody.velocity = direction * Input.GetAxis("Sensitivity") * _shootSpeed + new Vector3(0, 3.0f, 0);
        }

    }

    private IEnumerator MoveBallOnNet()
    {
        int loop = 0;
        // 4초간 대기하며 공이 그물 위에 있는지 검사
        while (_isOnNet && loop < 8)
        {
            yield return new WaitForSeconds(0.5f);
            loop++;
        }

        // 공이 4초 후에도 그물 위에 있으면 탄성 복구 후 중앙으로 공 이동
        if (_isOnNet)
        {
            _ball.material.bounciness = 0.8f;
            transform.position = new Vector3(0, 10, 0);
        }
    }
}
