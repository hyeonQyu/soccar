using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalChecker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ball"))
        {
            GameObject ball = other.gameObject;
            // 해당 공이 득점 상태가 아닌 경우에만 득점 인정(공이 트리거를 한 번에 두 번 통과하는 경우 방지)
            if(!ball.GetComponent<BallController>().IsScored)
            {
                Debug.Log("공 ID: " + other.gameObject.GetInstanceID());
                ball.GetComponent<BallController>().IsScored = true;

                // 3초 대기
                StartCoroutine(MoveBall(ball));
            }
        }
    }

    private IEnumerator MoveBall(GameObject ball)
    {
        yield return new WaitForSeconds(3);
        // (-25, 30, -25) ~ (25, 30, 25)의 랜덤 위치로 공을 이동시킴
        ball.transform.position = new Vector3(Random.Range(-25f, 25f), 30, Random.Range(-25f, 2f));
        ball.GetComponent<BallController>().IsScored = false;
        // 탄성력 복구
        ball.GetComponent<Collider>().material.bounciness = 0.8f;
    }
}
