using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private Text _roomKey;

    // 메시지
    private InputField _message;

    // 사운드
    private AudioSource _buttonClickSound;

    private NetworkManager _networkManager;

    public void Start()
    {
        _networkManager = GameObject.Find("Network Manager").GetComponent<NetworkManager>();


        try
        {
            _buttonClickSound = GameObject.Find("Button Click Sound").GetComponent<AudioSource>();

            _alertPanel = GameObject.Find("Alert Panel");
            _alertMessage = _alertPanel.transform.Find("Message").gameObject.GetComponent<Text>();
            _createRoomPanel = GameObject.Find("Create Room Panel");
            _inputRoomName = _createRoomPanel.transform.Find("InputField").GetComponent<InputField>();
            _roomPanel = GameObject.Find("Room Panel");
            _roomKey = _roomPanel.transform.Find("Room Key").GetComponent<Text>();
            _message = _roomPanel.transform.Find("Message").GetComponent<InputField>();            
        }
        catch(NullReferenceException) { }     
    }

    public void OnClickLogin()
    {
        _buttonClickSound.Play();

        GameObject loginPanel = GameObject.Find("Login Panel");
        InputField inputIdField = GameObject.Find("Input ID Field").GetComponent<InputField>();
        Button loginButton = GameObject.Find("Login Button").GetComponent<Button>();

        if(inputIdField.text.Equals(""))
            return;
        LobbyManager.PlayerName = inputIdField.text;

        // 닉네임은 알파벳과 숫자만 가능
        if(!IsAlphabetNumeric(LobbyManager.PlayerName, true))
        {
            _alertMessage.text = "You can use only alphabetic and numeric nicknames without space.";
            _alertPanel.GetComponent<Animator>().Play("Open Alert");
            return;
        }

        // 로그인과 동시에 게임 방 리스트 요청
        _networkManager.Send("login", LobbyManager.PlayerName);

        // 로그인 패널 소멸
        loginPanel.GetComponent<Animator>().SetBool("isDestroy", true);
        Destroy(inputIdField.gameObject);
        Destroy(loginButton.gameObject);
    }

    public void OnClickRefresh()
    {
        GetComponent<Animator>().SetBool("IsClicked", true);
        _buttonClickSound.Play();
        _networkManager.Send("refresh", LobbyManager.PlayerName);
        GetComponent<Animator>().SetBool("IsClicked", false);
    }

    public void OnClickCreateRoom()
    {
        _buttonClickSound.Play();

        // 방만들기 패널 활성화(애니메이션)
        _inputRoomName.text = "";
        _createRoomPanel.GetComponent<Animator>().Play("Create Room Panel");
    }

    // 방만들기 패널 안의 OK 버튼
    public void OnClickCreateRoomOk()
    {
        _buttonClickSound.Play();

        if(LobbyManager.CurrentPanel == LobbyManager.OnRoomPanel)
            return;

        string roomName = _inputRoomName.text;
        if(roomName.Equals(""))
            return;

        // 방 이름은 알파벳과 숫자만 가능
        if(!IsAlphabetNumeric(roomName, false))
        {
            _alertMessage.text = "You can use only alphabetic and numeric room names.";
            _alertPanel.GetComponent<Animator>().Play("Open Alert");
            return;
        }

        Packet.SendingCreateRoom sendingCreateRoom = new Packet.SendingCreateRoom(roomName, LobbyManager.PlayerName);
        _networkManager.Send<Packet.SendingCreateRoom>("create_room", sendingCreateRoom);

        // 방 패널 켜기
        _roomPanel.GetComponent<Animator>().Play("Enter Room");
        _createRoomPanel.GetComponent<Animator>().Play("Destroy Room Panel");
    }

    // 방만들기 패널 안의 Cancel 버튼
    public void OnClickCreateRoomCancel()
    {
        _buttonClickSound.Play();

        // 방만들기 패널 비활성화(애니메이션)
        _createRoomPanel.GetComponent<Animator>().Play("Destroy Room Panel");  
    }

    public void OnClickEnterRoom()
    {
        _buttonClickSound.Play();

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
        _networkManager.Send<Packet.SendingEnterRoom>("enter_room", sendingEnterRoom);

        // 방 패널 켜기
        _roomPanel.GetComponent<Animator>().Play("Enter Room");
    }

    public void OnClickExitRoom()
    {
        _buttonClickSound.Play();

        if(LobbyManager.CurrentPanel == LobbyManager.OnLobbyPanel)
            return;

        int roomKey = int.Parse(_roomPanel.transform.Find("Room Key").GetComponent<Text>().text);

        Packet.SendingExitRoom sendingExitRoom = new Packet.SendingExitRoom(roomKey, LobbyManager.PlayerName);
        _networkManager.Send<Packet.SendingExitRoom>("exit_room", sendingExitRoom);

        _roomPanel.GetComponent<Animator>().Play("Exit Room");
    }

    public void OnClickAlertOk()
    {
        _buttonClickSound.Play();

        _alertPanel.GetComponent<Animator>().Play("Close Alert");
    }

    public void OnClickSendMessage()
    {
        string roomKey = _roomPanel.transform.Find("Room Key").GetComponent<Text>().text;

        if(_message.text.Equals(""))
            return;

        Packet.SendingChat sendingChat = new Packet.SendingChat(roomKey, _message.text);
        _networkManager.Send<Packet.SendingChat>("chat", sendingChat);

        _message.text = "";
    }

    public void OnClickGameStart()
    {
        _buttonClickSound.Play();
        _networkManager.Send("start_game", _roomKey.text);
    }

    public void OnClickMoveLobby()
    {
        SceneMedium sceneMedium = GameObject.Find("Scene Medium").GetComponent<SceneMedium>();
        sceneMedium.IsAfterGame = true;
        sceneMedium.Port = 80;

        // 게임 서버와 연결 해제
        _networkManager = GameObject.Find("Network Manager").GetComponent<NetworkManager>();
        _networkManager.Send("disconnection", "");

        // 웹소켓 관련 요소 삭제
        Destroy(GameObject.Find("(singleton) socket.io.SocketManager"));
        Destroy(GameObject.Find("MainThreadDispatcher"));

        // static 요소 초기화
        GameLauncher.InitializeStatic();
        PlayerController.InitializeStatic();

        SceneManager.LoadScene("Lobby Scene");
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
