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
    private Image _logoPanel;
    [SerializeField]
    private Image _mainScreenPanel;
    [SerializeField]
    private Image _pressAnyKey;
    [SerializeField]
    private GameObject _roomList;
    [SerializeField]
    private GameObject _roomPanel;
    [SerializeField]
    private GameObject _alertPanel;
    [SerializeField]
    private GameObject _buttonControllerObject;
    private InputField _chatMessage;

    public static bool IsLogoDestroyed { set; get; }
    private bool _isOnLobby;

    // 통신을 위한 요소
    private LobbyNetworkLinker _lobbyNetworkLinker;
    private NetworkManager _networkManager;

    // 방 관리
    private RoomManager _roomManager;
    private Room _room;

    private Coroutine[] _showSpeechBubbleCoroutines = new Coroutine[Room.MaxPlayerPerRoom];

    private ButtonController _buttonController;

    public static string PlayerName { get; set; }
    public static byte CurrentPanel = 0;

    // Start is called before the first frame update
    void Start()
    {
        Animator logoAnimator = _logoPanel.GetComponent<Animator>();
        logoAnimator.SetBool("isDestroy", true);

        _roomManager = new RoomManager(_roomList);
        _room = new Room(_roomPanel);

        _lobbyNetworkLinker = new LobbyNetworkLinker(_roomManager, _room, _roomPanel, _alertPanel);
        _networkManager = new NetworkManager(false, _lobbyNetworkLinker);

        _chatMessage = _roomPanel.transform.Find("Chat").Find("Message").GetComponent<InputField>();
        _buttonController = _buttonControllerObject.GetComponent<ButtonController>();
    }

    // Update is called once per frame
    void Update()
    {
        // 아무 키나 누르면 Main Screen이 사라지고 로그인 화면으로 이동
        if(IsLogoDestroyed && Input.anyKeyDown && !_isOnLobby)
        {
            Destroy(_pressAnyKey.gameObject);
            Animator mainScreenAnimator = _mainScreenPanel.GetComponent<Animator>();
            mainScreenAnimator.SetBool("isDestroy", true);
            _isOnLobby = true;

            CurrentPanel = OnLoginPanel;
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
                _roomPanel.transform.Find("Chat").Find("Message").GetComponent<InputField>().ActivateInputField();
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
}
