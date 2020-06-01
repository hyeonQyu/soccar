using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutineScheduler : MonoBehaviour
{
    private Coroutine _movePlayersCoroutine = null;
    private Coroutine _moveBallsCoroutine = null;

    public void StartMoving(Packet.ReceivingPositions receivingPositions)
    {
        // 현재 공, 플레이어 위치 저장
        Vector3[] currentPlayerPositions = new Vector3[GameLauncher.Headcount];
        for(int i = 0; i < GameLauncher.Headcount; i++)
        {
            currentPlayerPositions[i] = PlayerController.Players[i].transform.position;
        }
        Vector3[] currentBallPositions = new Vector3[2];
        for(int i = 0; i < 2; i++)
        {
            currentBallPositions[i] = GameLauncher.Balls[i].transform.position;
        }

        // 플레이어와 공 움직임
        _movePlayersCoroutine = StartCoroutine(MovePlayers(currentPlayerPositions, receivingPositions.PlayerPositions));
        if(PlayerController.PlayerIndex != 0)
            _moveBallsCoroutine = StartCoroutine(MoveBalls(currentBallPositions, receivingPositions.BallPositions));
    }

    private IEnumerator MovePlayers(Vector3[] prePositions, Vector3[] destPositions)
    {
        float t = 0;

        for(int i = 0; i < 10; i++)
        {
            t += 0.1f;

            for(int j = 0; j < destPositions.Length; j++)
            {
                //if(j == PlayerController.PlayerIndex)
                //    continue;

                PlayerController.Players[j].transform.position = Vector3.Lerp(prePositions[j], destPositions[j], t);
            }

            yield return new WaitForSeconds(0.002f);
        }
    }

    private IEnumerator MoveBalls(Vector3[] prePositions, Vector3[] destPositions)
    {
        float t = 0;

        for(int i = 0; i < 10; i++)
        {
            t += 0.1f;

            for(int j = 0; j < destPositions.Length; j++)
            {
                GameLauncher.Balls[j].transform.position = Vector3.Lerp(prePositions[j], destPositions[j], t);
            }

            yield return new WaitForSeconds(0.002f);
        }
    }

    public void StopMoving()
    {
        if(_movePlayersCoroutine != null)
        {
            StopCoroutine(_movePlayersCoroutine);
        }

        if(_moveBallsCoroutine != null)
        {

        }
    }
}
