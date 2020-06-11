using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutineScheduler : MonoBehaviour
{
    private Coroutine _movePlayersCoroutine = null;
    private Coroutine _moveBallsCoroutine = null;

    public void StartMoving(Packet.ReceivingTransform receivingTransform)
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
        _movePlayersCoroutine = StartCoroutine(MovePlayers(currentPlayerPositions, receivingTransform));
        _moveBallsCoroutine = StartCoroutine(MoveBalls(currentBallPositions, receivingTransform.BallPositions));
    }

    private IEnumerator MovePlayers(Vector3[] prePositions, Packet.ReceivingTransform receivingTransform)
    {
        Vector3[] destPositions = receivingTransform.PlayerPositions;
        float t = 0;
        AnimFollow.HashIDs_AF hash = PlayerController.Hash;

        for (int k = 0; k < destPositions.Length; k++)
        {
            // 태클 중인 경우 플레이어를 회전시키지 않음
            try
            {
                if(PlayerController.PlayerAnimators[k].GetCurrentAnimatorStateInfo(0).fullPathHash == hash.Tackle
                        || receivingTransform.AnimHashCodes[k] == hash.TackleTrigger)
                    continue;

                PlayerController.Players[k].transform.eulerAngles = receivingTransform.PlayerRotations[k];
            }
            catch(MissingReferenceException e) { }
        }

        for(int i = 0; i < 10; i++)
        {
            t += 0.1f;

            for(int j = 0; j < destPositions.Length; j++)
            {
                try
                {
                    if(PlayerController.Players[j].transform.root.GetChild(1).gameObject.GetComponent<AnimFollow.RagdollControl_AF>().falling ||
                        PlayerController.Players[j].transform.root.GetChild(1).gameObject.GetComponent<AnimFollow.RagdollControl_AF>().gettingUp)
                        continue;

                    // 플레이어 이동
                    PlayerController.Players[j].transform.position = Vector3.Lerp(prePositions[j], destPositions[j], t);
                    Vector3 newVector = new Vector3(PlayerController.Players[j].transform.position.x, 0.1f, PlayerController.Players[j].transform.position.z);
                    PlayerController.MiniMapManager.Players[j].transform.localPosition = newVector;
                    PlayerController.PlayerAnimators[j].SetFloat(hash.SpeedFloat, receivingTransform.PlayerSpeeds[j] / 5, 0.1f, Time.fixedDeltaTime);

                    // 애니메이션을 실행시키지 않는 조건
                    if(receivingTransform.AnimHashCodes[j] == 0 || PlayerController.PlayerAnimators[j].IsInTransition(0))
                        continue;
                    int curAnimHashCode = PlayerController.PlayerAnimators[j].GetCurrentAnimatorStateInfo(0).fullPathHash;
                    if(curAnimHashCode == hash.Tackle || curAnimHashCode == hash.Shoot || curAnimHashCode == hash.Jump)
                        continue;

                    // 애니메이션 실행
                    PlayerController.PlayerAnimators[j].SetTrigger(receivingTransform.AnimHashCodes[j]);
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
                Vector3 newVector = new Vector3(GameLauncher.Balls[j].transform.position.x, 0.2f, GameLauncher.Balls[j].transform.position.z);
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
