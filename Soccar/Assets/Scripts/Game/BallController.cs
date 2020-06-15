using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public const int TotalBallCount = 3;
    public static int CurrentBallCount { get; set; }

    public int BallIndex { private get; set; }
    public static float[] ShootPowers { get; set; }
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
        float force = Mathf.Abs(_rigidBody.velocity.x) + Mathf.Abs(_rigidBody.velocity.z);
        float bouncingForce = Mathf.Abs(_rigidBody.velocity.x) + Mathf.Abs(_rigidBody.velocity.y * 2f) + Mathf.Abs(_rigidBody.velocity.z);

        // 땅에 부딪히면 탄성 감소
        if (other.CompareTag("Ground"))
        {
            _ball.material.bounciness *= 0.8f;

            GameLauncher.Sound.BounceBall.volume = bouncingForce / 20f;
            GameLauncher.Sound.BounceBall.Play();

            return;
        }
        // 그물은 공을 튀기지 않기 때문에 그물 위에 올라가면 공을 내려줘야 함
        else if (other.CompareTag("Net"))
        {
            _ball.material.bounciness *= 0.01f;

            if(other.name.Equals("Back"))
            {
                GameLauncher.Sound.GoalNet.volume = force / 10f;
                GameLauncher.Sound.GoalNet.Play();
            }

            _isOnNet = true;
            StartCoroutine(MoveBallOnNet());
            return;
        }
        else if (other.CompareTag("Fense"))
        {
            _ball.material.bounciness = 1;

            GameLauncher.Sound.HitFense.volume = force / 30f;
            GameLauncher.Sound.HitFense.Play();

            return;
        }
        else if(other.CompareTag("Post"))
        {
            _ball.material.bounciness = 1;

            GameLauncher.Sound.HitPost.volume = force / 20f;
            GameLauncher.Sound.HitPost.Play();

            return;
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

            string collisionName = collision.gameObject.name;
            // 슈팅
            if (_collisionAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == PlayerController.Hash.Shoot && 
                    (collisionName.Equals("RightLeg") || collisionName.Equals("RightUpLeg") || collision.Equals("LeftLeg")))
            {
                // 방향은 Avatar가 바라보는 방향
                Vector3 direction = collision.transform.root.GetChild(0).forward;
                Vector3 movingPosition = direction * 0.2f;
                // 경기장 밖으로 나가지 않도록 조절
                if(PlayerController.IsOutOfStadium(transform.position, movingPosition))
                    return;
                // 한 번만 실행하도록 위치 바꿔줌
                transform.position += movingPosition;

                _isShoot = true;
            }
            else if(_collisionAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == PlayerController.Hash.Shoot)
                Debug.Log("shoot " + collisionName);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer).Equals("RagDoll"))
        {
            string collisionName = collision.gameObject.name;
            if(collisionName.Equals("RightLeg") || collisionName.Equals("LeftLeg") || collisionName.Equals("RightUpLeg")
                || collisionName.Equals("LeftUpLeg") || collisionName.Equals("Hips"))
            {
                try
                {
                    // 드리블
                    _rigidBody.velocity = collision.transform.forward * _collisionAnimator.GetFloat("SpeedFloat") * 5.0f;
                }
                catch(MissingReferenceException) { }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Net"))
        {
            _isOnNet = false;
        }

        if(LayerMask.LayerToName(collision.gameObject.layer).Equals("RagDoll"))
        {
            //Debug.Log("fever_time 드리블 종료");
        }

        if (LayerMask.LayerToName(collision.gameObject.layer).Equals("RagDoll") && _isShoot)
        {
            // 방향은 Avatar가 바라보는 방향
            Vector3 direction = collision.transform.root.GetChild(0).forward;

            

            // 키 입력 강도에 따른 파워 조절
            float power = ShootPowers[collision.transform.root.GetComponent<PlayerInformation>().PlayerIndex];
            if(power < 0.5f)
                power = 0.5f;
            // 살짝 위로 올라가도록
            _rigidBody.velocity = direction * power * _shootSpeed + new Vector3(Random.Range(-2.5f, 2.5f), Random.Range(5f, 10f) * power, Random.Range(-2.5f, 2.5f));
            if(!GameLauncher.Sound.KickBall[BallIndex].isPlaying)
            {
                GameLauncher.Sound.KickBall[BallIndex].volume = power;
                GameLauncher.Sound.KickBall[BallIndex].Play();
            }
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
