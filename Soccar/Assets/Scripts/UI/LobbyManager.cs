using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public const byte OnLoginPanel = 1;
    public const byte OnLobbyPanel = 2;
    public const byte OnRoomPanel = 3;

    [SerializeField]
    private GameObject _roomList;
    [SerializeField]
    private GameObject _roomPanel;
    [SerializeField]
    private GameObject _alertPanel;
    [SerializeField]
    private GameObject _buttonControllerObject;
    [SerializeField]
    private GameObject _networkManagerObject;
    //[SerializeField]
    //private GameObject _sceneMediumObject;

    private SceneMedium _sceneMedium;

    private InputField _chatMessage;

    // 통신을 위한 요소
    private LobbyNetworkLinker _lobbyNetworkLinker;
    private NetworkManager _networkManager;

    // 사운드
    private Sound _sound;

    // 방 관리
    private RoomManager _roomManager;
    private Room _room;

    private Coroutine[] _showSpeechBubbleCoroutines = new Coroutine[Room.MaxPlayerPerRoom];

    private ButtonController _buttonController;

    public static string SocketId { get; set; }
    public static string PlayerName { get; set; }
    public static byte CurrentPanel { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        _sound = new Sound(false);

        _roomManager = new RoomManager(_roomList);
        _room = new Room(_roomPanel);

        _lobbyNetworkLinker = new LobbyNetworkLinker(_roomManager, _room, _roomPanel, _alertPanel);
        _sceneMedium = GameObject.Find("Scene Medium").GetComponent<SceneMedium>();
        _networkManager = _networkManagerObject.GetComponent<NetworkManager>();
        _networkManager.SetWebSocket(_sceneMedium, _lobbyNetworkLinker);

        _chatMessage = _roomPanel.transform.Find("Message").GetComponent<InputField>();
        _buttonController = _buttonControllerObject.GetComponent<ButtonController>();

        // 게임 첫 실행
        if(!_sceneMedium.IsAfterGame)
        {
            CurrentPanel = OnLoginPanel;
        }
        // 게임 플레이 후 로비 씬 전환
        else
        {
            PlayerName = _sceneMedium.PlayerName;
            CurrentPanel = 0;
            Destroy(GameObject.Find("Login Panel"));
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 게임 플레이 후 로비
        if(CurrentPanel == 0)
        {
            if(_networkManager.Socket.IsConnected)
            {
                CurrentPanel = OnLobbyPanel;
                _networkManager.Send("login", PlayerName);
            }
        }

        // Input Field가 있는 패널에서 엔터키 입력
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(CurrentPanel == OnLoginPanel)
            {
                _buttonController.OnClickLogin();
            }
            else if(CurrentPanel == OnRoomPanel)
            {
                _chatMessage.ActivateInputField();
                _buttonController.OnClickSendMessage();
            }
        }

        // 채팅 말풍선
        try
        {
            for(int i = 0; i < _room.PlayerKeys.Length; i++)
            {
                if(_room.IsSpeeching[i])
                {
                    _room.IsSpeeching[i] = false;

                    if(_showSpeechBubbleCoroutines[i] != null)
                        StopCoroutine(_showSpeechBubbleCoroutines[i]);
                    _showSpeechBubbleCoroutines[i] = StartCoroutine(ShowSpeechBubbleOnRoomPanel(i));
                }
            }
        }
        catch(NullReferenceException e) { }
    }

    private IEnumerator ShowSpeechBubbleOnRoomPanel(int i)
    {
        // 말풍선을 4초 후 사라지게 만듦
        yield return new WaitForSeconds(4);
        _room.SpeechBubble[i].transform.localScale = new Vector3(0, 0, 0);
    }

    private void OnApplicationQuit()
    {
        _networkManager.Send("disconnection", "");
    }
}
