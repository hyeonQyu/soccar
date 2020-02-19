using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Collider _collider;
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
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Net"))
        {
            Debug.Log("bounce " + _collider.material.bounciness);
            _collider.material.bounciness *= 0.01f;
        }
    }

    // 실제 축구공처럼 튀기게 하기 위함
    private void OnCollisionEnter(Collision collision)
    {
        // 땅에 부딪히면 탄성 감소
        if(collision.gameObject.CompareTag("Ground"))
        {
            _collider.material.bounciness *= 0.8f;
            return;
        }
        // 그물은 공을 튀기지 않기 때문에 그물 위에 올라가면 공을 내려줘야 함
        else if(collision.gameObject.CompareTag("Net"))
        {
            _collider.material.bounciness *= 0.01f;
            _isOnNet = true;
            StartCoroutine(MoveBallOnNet());
            return;
        }
        // 다른 물체에 부딪히면 탄성을 원래대로
        _collider.material.bounciness = 0.8f;
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
            _collider.material.bounciness = 0.8f;
            transform.position = new Vector3(Random.Range(-25f, 25f), 30, Random.Range(-25f, 2f));
        }
    }
}
