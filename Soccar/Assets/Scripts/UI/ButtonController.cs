using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    // 로그인 관련
    private GameObject _loginPanel;
    private InputField _inputIdField;
    private Button _loginButton;

    // 방만들기 관련
    private GameObject _createRoomPanel;
    private InputField _inputRoomName;

    // 방
    private GameObject _roomPanel;

    public void Start()
    {
        _loginPanel = GameObject.Find("Login Panel");
        _inputIdField = GameObject.Find("Input ID Field").GetComponent<InputField>();
        _loginButton = GameObject.Find("Login Button").GetComponent<Button>();

        _createRoomPanel = GameObject.Find("Create Room Panel");
        _inputRoomName = _createRoomPanel.transform.Find("InputField").GetComponent<InputField>();

        _roomPanel = GameObject.Find("Room Panel");
    }

    public void OnClickLogin()
    {
        if(_inputIdField.text.Equals(""))
            return;
        LobbyManager.PlayerId = _inputIdField.text;

        // 로그인과 동시에 게임 방 리스트 요청
        try
        {
            NetworkManager.Send("room_list", LobbyManager.PlayerId);
        }
        catch(NullReferenceException e)
        {
            Debug.LogError("OnClickLogin: Send 실패");
        }

        // 로그인 패널 소멸
        _loginPanel.GetComponent<Animator>().SetBool("isDestroy", true);
        Destroy(_inputIdField.gameObject);
        Destroy(_loginButton.gameObject);
    }

    public void OnClickRefresh()
    {
        try
        {
            NetworkManager.Send("room_list", LobbyManager.PlayerId);
        }
        catch(NullReferenceException e)
        {
            Debug.LogError("OnClickRefresh: Send 실패");
        }
    }

    public void OnClickCreateRoom()
    {
        // 방만들기 패널 활성화(애니메이션)
        _inputRoomName.text = "";
        _createRoomPanel.GetComponent<Animator>().Play("Create Room Panel");
    }

    // 방만들기 패널 안의 OK 버튼
    public void OnClickCreateRoomOk()
    {
        string roomName = _inputRoomName.text;
        if(roomName.Equals(""))
            return;
        try
        {
            NetworkManager.Send("create_room", roomName);
        }
        catch(NullReferenceException e)
        {
            Debug.LogError("OnClickCreateRoomOk: Send 실패");
        }

        // 방 패널 켜기
        _roomPanel.GetComponent<Animator>().Play("Enter Room");
    }

    // 방만들기 패널 안의 Cancel 버튼
    public void OnClickCreateRoomCancel()
    {
        // 방만들기 패널 비활성화(애니메이션)
        _createRoomPanel.GetComponent<Animator>().Play("Destroy Room Panel");  
    }

    public void OnClickEnterRoom()
    {

    }

    public void OnClickExitRoom()
    {

    }
}
