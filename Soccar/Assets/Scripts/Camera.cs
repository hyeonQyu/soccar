﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    private GameObject[] _gameObjectList;

    public static GameObject _gameObject;
    public static int _playerIndex; // p1=0, p2=1, p3=2, p4=3

    const int PLAYERCOUNTS = 4;

    // Start is called before the first frame update
    void Start()
    {
        _gameObjectList = new GameObject[4];
        _gameObjectList[0] = GameObject.Find("p1");
        _gameObjectList[1] = GameObject.Find("p2");
        _gameObjectList[2] = GameObject.Find("p3");
        _gameObjectList[3] = GameObject.Find("p4");
        _playerIndex = 0;
        _gameObject = _gameObjectList[_playerIndex];
        transform.eulerAngles = new Vector3(45f, _playerIndex * 90f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // 왼쪽 오른쪽 방향키가 눌렸을 경우를 위한 함수
        OnArrowTyped();

        // 카메라의 position을 실시간으로 업데이트함
        ChangeCameraView();

        Debug.Log(_playerIndex);
    }

    void OnArrowTyped()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow) == true)
        {
            ChangePlayer(false);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow) == true)
        {
            ChangePlayer(true);
        }
    }

    void ChangePlayer(bool isRight)
    {
        if (isRight)
        {
            // 다음플레이어로 변경
            _playerIndex = _playerIndex + 1;
            _playerIndex = _playerIndex % PLAYERCOUNTS;
            _gameObject = _gameObjectList[_playerIndex];
            
        }
        else
        {
            // 이전플레이어로 변경
            _playerIndex = _playerIndex - 1 + PLAYERCOUNTS;
            _playerIndex = _playerIndex % PLAYERCOUNTS;
            _gameObject = _gameObjectList[_playerIndex];
        }
    }

    void ChangeCameraView()
    {
        // 해당하는 플레이어에 따라 카메라의 위치가 달라진다.
        /* 기본적으로 카메라는 플레이어로 부터 7.0만큼 떨어져 있으며(플레이어마다 X or Z) 높이(Y)는 7.0이다.
            또한 45도 만큼 아래를 쳐다보고 있으며 해당 플레이어가 오른쪽으로 위치하도록 Rotation.Y 값을 조정한다. */
        switch (_playerIndex)
        {
            case 0:
                transform.transform.position = new Vector3(_gameObject.transform.position.x, 7f, _gameObject.transform.position.z - 7f);
                transform.eulerAngles = new Vector3(45f, _playerIndex * 90f, 0);
                break;
            case 1:
                transform.transform.position = new Vector3(_gameObject.transform.position.x -7f, 7f, _gameObject.transform.position.z);
                transform.eulerAngles = new Vector3(45f, _playerIndex * 90f, 0);
                break;
            case 2:
                transform.transform.position = new Vector3(_gameObject.transform.position.x, 7f, _gameObject.transform.position.z + 7f);
                transform.eulerAngles = new Vector3(45f, _playerIndex * 90f, 0);
                break;
            case 3:
                transform.transform.position = new Vector3(_gameObject.transform.position.x + 7f, 7f, _gameObject.transform.position.z);
                transform.eulerAngles = new Vector3(45f, _playerIndex * 90f, 0);
                break;
        }

    }
}