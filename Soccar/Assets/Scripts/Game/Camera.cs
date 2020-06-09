using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField]
    private float _distance = 5.3f;
    [SerializeField]
    private float _angle = 25f;
    [SerializeField]
    private float _height = 3.7f;
    private bool _isFirstRun = true;

    [SerializeField]
    private UnityEngine.Camera _miniMapCam;

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
        if (!_isFirstRun)
        {
            // 카메라의 position을 실시간으로 업데이트함
            ChangeCameraView();
        }
    }

    void ChangeCameraView()
    {
        // 해당하는 플레이어에 따라 카메라의 위치가 달라진다.
        /* 카메라는 플레이어로 부터 _distance만큼 떨어져 있으며(플레이어마다 X or Z) 높이(Y)는 _distance이다.
            또한 _angle 만큼 아래를 쳐다보고 있으며 해당 플레이어가 오른쪽으로 위치하도록 Rotation.Y 값을 조정한다. */
        transform.position = new Vector3(PlayerController.Player.transform.position.x, _height, PlayerController.Player.transform.position.z)
                                                            + (_distance * PlayerController.BackwardVector);
        transform.eulerAngles = new Vector3(_angle, -PlayerController.Theta, 0);
    }
}
