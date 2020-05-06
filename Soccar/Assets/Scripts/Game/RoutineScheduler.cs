using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutineScheduler : MonoBehaviour
{
    private Coroutine _movePlayersCoroutine = null;

    public void StartPlayerMoving(Packet.ReceivingPositions receivingPositions)
    {
        Vector3[] currentPositions = new Vector3[4];
        for(int i = 0; i < 4; i++)
        {
            currentPositions[i] = PlayerController.Players[i].transform.position;
        }

        _movePlayersCoroutine = StartCoroutine(MovePlayers(receivingPositions.PlayerPositions, currentPositions));
    }

    // 10ms 단위로 10번 움직임
    private IEnumerator MovePlayers(Vector3[] prePositions, Vector3[] destPositions)
    {
        float t = 0;

        for(int i = 0; i < 10; i++)
        {
            t += 0.1f;

            for(int j = 0; j < destPositions.Length; j++)
            {
                PlayerController.Players[j].transform.position = Vector3.Lerp(prePositions[j], destPositions[i], t);
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    public void StopPlayerMoving()
    {
        if(_movePlayersCoroutine != null)
        {
            StopCoroutine(_movePlayersCoroutine);
        }
    }
}
