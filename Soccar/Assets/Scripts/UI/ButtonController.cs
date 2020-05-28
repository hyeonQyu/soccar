using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    // 경고 창
    private GameObject _alertPanel;
    private Text _alertMessage;

    // 방만들기 관련
    private GameObject _createRoomPanel;
    private InputField _inputRoomName;

    // 방
    private GameObject _roomPanel;

    public void Start()
    {
        _alertPanel = GameObject.Find("Alert Panel");
        _alertMessage = _alertPanel.transform.Find("Message").gameObject.GetComponent<Text>();

        _createRoomPanel = GameObject.Find("Create Room Panel");
        _inputRoomName = _createRoomPanel.transform.Find("InputField").GetComponent<InputField>();

        _roomPanel = GameObject.Find("Room Panel");
    }

    public void OnClickLogin()
    {
        GameObject loginPanel = GameObject.Find("Login Panel");
        InputField inputIdField = GameObject.Find("Input ID Field").GetComponent<InputField>();
        Button loginButton = GameObject.Find("Login Button").GetComponent<Button>();

        if(inputIdField.text.Equals(""))
            return;
        LobbyManager.PlayerName = inputIdField.text;

        // 닉네임은 알파벳과 숫자만 가능
        if(!IsAlphabetNumeric(LobbyManager.PlayerName, true))
        {
            _alertMessage.text = "You can use only alphabetic and numeric nicknames without space";
            _alertPanel.GetComponent<Animator>().Play("Open Alert");
            return;
        }

        // 로그인과 동시에 게임 방 리스트 요청
        try
        {
            NetworkManager.Send("room_list", LobbyManager.PlayerName);
        }
        catch(NullReferenceException e)
        {
            Debug.LogError("OnClickLogin: Send 실패");
        }

        // 로그인 패널 소멸
        loginPanel.GetComponent<Animator>().SetBool("isDestroy", true);
        Destroy(inputIdField.gameObject);
        Destroy(loginButton.gameObject);
    }

    public void OnClickRefresh()
    {
        try
        {
            NetworkManager.Send("room_list", LobbyManager.PlayerName);
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
        if(LobbyManager.CurrentPanel == LobbyManager.OnRoomPanel)
            return;

        string roomName = _inputRoomName.text;
        if(roomName.Equals(""))
            return;

        // 방 이름은 알파벳과 숫자만 가능
        if(!IsAlphabetNumeric(roomName, false))
        {
            _alertMessage.text = "You can use only alphabetic and numeric room names";
            _alertPanel.GetComponent<Animator>().Play("Open Alert");
            return;
        }

        Packet.SendingCreateRoom sendingCreateRoom = new Packet.SendingCreateRoom(roomName, LobbyManager.PlayerName);        
        try
        {
            NetworkManager.Send<Packet.SendingCreateRoom>("create_room", sendingCreateRoom);
        }
        catch(NullReferenceException e)
        {
            Debug.LogError("OnClickCreateRoomOk: Send 실패");
        }

        // 방 패널 켜기
        _roomPanel.GetComponent<Animator>().Play("Enter Room");
        _createRoomPanel.GetComponent<Animator>().Play("Destroy Room Panel");
    }

    // 방만들기 패널 안의 Cancel 버튼
    public void OnClickCreateRoomCancel()
    {
        // 방만들기 패널 비활성화(애니메이션)
        _createRoomPanel.GetComponent<Animator>().Play("Destroy Room Panel");  
    }

    public void OnClickEnterRoom()
    {
        if(LobbyManager.CurrentPanel == LobbyManager.OnRoomPanel)
            return;

        int roomKey = -1;
        try
        {
            roomKey = int.Parse(transform.Find("Room Key").GetComponent<Text>().text);
        }
        catch(FormatException e)
        {
            return;
        }

        Packet.SendingEnterRoom sendingEnterRoom = new Packet.SendingEnterRoom(roomKey, LobbyManager.PlayerName);
        try
        {
            NetworkManager.Send<Packet.SendingEnterRoom>("enter_room", sendingEnterRoom);
        }
        catch(NullReferenceException e)
        {
            Debug.LogError("OnClickEnterRoom: Send 실패");
        }

        // 방 패널 켜기
        _roomPanel.GetComponent<Animator>().Play("Enter Room");
    }

    public void OnClickExitRoom()
    {
        if(LobbyManager.CurrentPanel == LobbyManager.OnLobbyPanel)
            return;

        int roomKey = int.Parse(_roomPanel.transform.Find("Room Key").GetComponent<Text>().text);

        Packet.SendingExitRoom sendingExitRoom = new Packet.SendingExitRoom(roomKey, LobbyManager.PlayerName);
        try
        {
            NetworkManager.Send<Packet.SendingExitRoom>("exit_room", sendingExitRoom);
        }
        catch(NullReferenceException e)
        {
            Debug.LogError("OnClickExitRoom: Send 실패");
        }

        _roomPanel.GetComponent<Animator>().Play("Exit Room");
    }

    public void OnClickAlertOk()
    {
        _alertPanel.GetComponent<Animator>().Play("Close Alert");
    }

    public void OnClickSendMessage()
    {
        InputField message = GameObject.Find("Message").GetComponent<InputField>();
        string roomKey = _roomPanel.transform.Find("Room Key").GetComponent<Text>().text;

        if(message.text.Equals(""))
            return;

        Packet.SendingChat sendingChat = new Packet.SendingChat(roomKey, message.text);
        try
        {
            NetworkManager.Send<Packet.SendingChat>("chat", sendingChat);
        }
        catch(NullReferenceException e)
        {
            Debug.LogError("OnClickSendMesage: Send 실패");
        }

        message.text = "";
    }

    private bool IsAlphabetNumeric(string str, bool isNickname)
    {
        bool isAvaliableStr = true;
        // 닉네임은 영어와 숫자만 허용
        foreach(char c in str)
        {
            if(('A' <= c && c <= 'Z') || ('a' <= c && c <= 'z') || (0 <= c - '0' && c - '0' <= 9))
                continue;

            // 방 이름은 공백 허용, 닉네임은 공백을 허용하지 않음
            if(!isNickname && c == ' ')
                continue;

            isAvaliableStr = false;
            break;
        }

        return isAvaliableStr;
    }
}
