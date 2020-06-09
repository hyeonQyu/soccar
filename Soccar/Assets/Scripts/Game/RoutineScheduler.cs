using System;
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
            try
            {
                currentPlayerPositions[i] = PlayerController.Players[i].transform.position;
            }
            catch(Exception e) { }
        }
        Vector3[] currentBallPositions = new Vector3[2];
        for(int i = 0; i < 2; i++)
        {
            currentBallPositions[i] = GameLauncher.Balls[i].transform.position;
        }

        // 플레이어와 공 움직임
        _movePlayersCoroutine = StartCoroutine(MovePlayers(currentPlayerPositions, receivingPositions.PlayerPositions));
        _moveBallsCoroutine = StartCoroutine(MoveBalls(currentBallPositions, receivingPositions.BallPositions));
    }

    private IEnumerator MovePlayers(Vector3[] prePositions, Vector3[] destPositions)
    {
        float t = 0;
        Vector3 directionVector;
        for (int k = 0; k < destPositions.Length; k++)
        {
            if(k != PlayerController.PlayerIndex)
            {
                directionVector = destPositions[k] - prePositions[k];
                directionVector.y = 0;
            
                if (directionVector != Vector3.zero)
                {
                    try
                    {
                        PlayerController.Players[k].transform.rotation = Quaternion.LookRotation(directionVector.normalized);
                    }
                    catch(MissingReferenceException e) { }
                }
            }
        }

        for(int i = 0; i < 10; i++)
        {
            t += 0.1f;

            for(int j = 0; j < destPositions.Length; j++)
            {
                try
                {
                    //if(j == PlayerController.PlayerIndex)
                    //    continue;
                    if (PlayerController.Players[j].transform.root.GetChild(1).gameObject.GetComponent<AnimFollow.RagdollControl_AF>().falling)
                    {
                        PlayerController.Players[j].transform.position = PlayerController.Players[j].transform.root.GetChild(1).gameObject.GetComponent<AnimFollow.RagdollControl_AF>().ragdollRootBone.position;
                    }
                    else
                    {

                        PlayerController.Players[j].transform.position = Vector3.Lerp(prePositions[j], destPositions[j], t);
                        Vector3 newVector = new Vector3(PlayerController.Players[j].transform.position.x, 1, PlayerController.Players[j].transform.position.z);
                        PlayerController.MiniMapManager.Players[j].transform.localPosition = newVector;
                    }
                }
                catch (Exception e) { }
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
                if(PlayerController.PlayerIndex != PlayerController.SuperClientIndex)
                    GameLauncher.Balls[j].transform.position = Vector3.Lerp(prePositions[j], destPositions[j], t);
                Vector3 newVector = new Vector3(GameLauncher.Balls[j].transform.position.x, 2, GameLauncher.Balls[j].transform.position.z);
                PlayerController.MiniMapManager.Balls[j].transform.localPosition = newVector;
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
