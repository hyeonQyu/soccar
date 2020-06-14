using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float _distance = 5.3f;
    [SerializeField]
    private float _angle = 25f;
    [SerializeField]
    private float _height = 3.7f;
    [SerializeField]
    private float _weightBackward = 0.6f;

    private bool _isFirstRun = true;
    private bool _isEndGame;
    private int _winner;

    [SerializeField]
    private Camera _miniMapCam;

    // Update is called once per frame
    void LateUpdate()
    {
        if(PlayerController.IsPlayersInitialized && _isFirstRun)
        {
            _miniMapCam.transform.eulerAngles = new Vector3(90, -PlayerController.Theta, 0);
            transform.eulerAngles = new Vector3(_angle, -PlayerController.Theta, 0);
            _isFirstRun = false;
            Debug.Log("==카메라 세팅됨");
        }
        if (!_isFirstRun && !_isEndGame)
        {
            // 카메라의 position을 실시간으로 업데이트함
            ChangeCameraView();
        }
        else if(_isEndGame)
        {
            MoveToWinner(_winner);
        }
    }

    void ChangeCameraView()
    {
        // 해당하는 플레이어에 따라 카메라의 위치가 달라진다.
        /* 카메라는 플레이어로 부터 _distance만큼 떨어져 있으며(플레이어마다 X or Z) 높이(Y)는 _distance이다.
            또한 _angle 만큼 아래를 쳐다보고 있으며 해당 플레이어가 오른쪽으로 위치하도록 Rotation.Y 값을 조정한다. */
        Vector3 playerPosition = PlayerController.Player.transform.position;
        Vector3 goalPostPosition = PlayerController.GoalPosts[PlayerController.PlayerIndex].transform.position;

        // 골대로부터 특정 거리
        float distanceBackward = Vector3.Distance(playerPosition, goalPostPosition + 2.35f * PlayerController.BackwardVector);
        float distanceRight = Vector3.Distance(playerPosition, goalPostPosition + 7 * PlayerController.RightVector);
        float distanceLeft = Vector3.Distance(playerPosition, goalPostPosition + 7 * PlayerController.LeftVector);

        // 골대에 가까워지면 시야가 방해되므로 카메라의 회전 및 높이를 변경
        float height = _height;
        float angle = _angle;
        float distance = _distance;
        if(distanceBackward < 5 || distanceLeft < 5 || distanceRight < 5)
        {
            float tmpDistanceBackward = distanceBackward - _weightBackward;
            float distanceMin = Mathf.Min(tmpDistanceBackward, distanceLeft, distanceRight);
            if(distanceMin == tmpDistanceBackward)
                distanceMin = distanceBackward;

            height = -distanceMin + 8.7f;
            angle = -11f * distanceMin + 85;
            distance = 0.4f * distanceMin + 3.3f;
        }

        float lerpSpeed = Time.deltaTime * 4f;

        Vector3 targetPosition = new Vector3(PlayerController.Player.transform.position.x, height, PlayerController.Player.transform.position.z)
                                                            + (distance * PlayerController.BackwardVector);
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed);

        Vector3 rotation = new Vector3(angle, transform.eulerAngles.y, 0);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, rotation, lerpSpeed / 2);
    }

    public void MoveToWinner(int winner)
    {
        _isEndGame = true;
        _winner = winner;
        Transform winnerTransform = PlayerController.Players[winner].transform;

        // 위치 이동
        Vector3 forwardPosition = winnerTransform.position + winnerTransform.forward * 2.0f + new Vector3(0, 1.0f, 0);
        transform.position = Vector3.Lerp(transform.position, forwardPosition, Time.deltaTime * 2f);

        // 승자 플레이어를 비춤
        Vector3 direction = winnerTransform.position + new Vector3(0, 0.5f, 0) - transform.position;
        transform.rotation = Quaternion.LookRotation(direction.normalized);
    }
}
