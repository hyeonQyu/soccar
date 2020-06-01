using UnityEngine;
using UnityEngine.UI;

public class Room
{
    public const int MaxPlayerPerRoom = 6;

    // 실제 정보
    public int RoomKey { get; private set; }
    public string RoomName { get; private set; }
    public int Headcount { get; private set; }
    public string[] PlayerKeys { get; private set; }
    public string[] PlayerNames { get; private set; }

    // 방 패널 UI에 관련된 요소
    public GameObject RoomPanel { get; private set; }
    private GameObject _playerList;
    private GameObject[] _players = new GameObject[MaxPlayerPerRoom];
    private Text[] _playerKeys = new Text[MaxPlayerPerRoom];
    private Text[] _playerNames = new Text[MaxPlayerPerRoom];
    private RawImage[] _playerCharacters = new RawImage[MaxPlayerPerRoom];
    private Button _gameStartButton;

    // 채팅 요소
    public Image[] SpeechBubble { get; set; }
    public Text[] SpeechMessage { get; set; }
    public bool[] IsSpeeching { get; set; }

    // 방 패널 출력을 위한 객체 생성자
    public Room(GameObject roomPanel)
    {
        RoomPanel = roomPanel;

        SpeechBubble = new Image[MaxPlayerPerRoom];
        SpeechMessage = new Text[MaxPlayerPerRoom];
        IsSpeeching = new bool[MaxPlayerPerRoom];

        InitializePlayersUiComponents();
    }

    // 로비 패널 출력을 위한 객체 생성자
    public Room(int roomKey, string roomName, int headcount)
    {
        RoomKey = roomKey;
        RoomName = roomName;
        Headcount = headcount;
    }

    // 방 패널의 UI 요소 초기화
    private void InitializePlayersUiComponents()
    {
        _playerList = RoomPanel.transform.Find("Player List").gameObject;
        for(int i = 0; i < MaxPlayerPerRoom; i++)
        {
            _players[i] = _playerList.transform.Find("Player" + i).gameObject;
            _playerKeys[i] = _players[i].transform.Find("Key").GetComponent<Text>();
            _playerNames[i] = _players[i].transform.Find("Name Background").Find("Name").GetComponent<Text>();
            _playerCharacters[i] = _players[i].transform.Find("Character").GetComponent<RawImage>();

            SpeechBubble[i] = _players[i].transform.Find("Speech Bubble").GetComponent<Image>();
            SpeechMessage[i] = SpeechBubble[i].transform.Find("Speech Message").GetComponent<Text>();
        }

        _gameStartButton = RoomPanel.transform.Find("Game Start Button").GetComponent<Button>();
    }

    // 현재 방 정보 업데이트
    public void SetRoomInfo(Packet.ReceivingRoomInfo receivingRoomInfo)
    {
        RoomKey = receivingRoomInfo.RoomKey;
        RoomName = receivingRoomInfo.RoomName;
        PlayerKeys = receivingRoomInfo.PlayerKeys;
        PlayerNames = receivingRoomInfo.PlayerNames;

        ShowRoomInfo();
    }

    // 방 패널 UI 업데이트
    private void ShowRoomInfo()
    {
        RoomPanel.transform.Find("Room Key").GetComponent<Text>().text = RoomKey.ToString();
        RoomPanel.transform.Find("Room Name").GetComponent<Text>().text = RoomName;

        // 방에 접속한 플레이어 목록 업데이트
        int i;
        // 현재 방에 있는 플레이어
        for(i = 0; i < PlayerNames.Length; i++)
        {
            _playerKeys[i].text = PlayerKeys[i];
            _playerNames[i].text = PlayerNames[i];
            _playerCharacters[i].texture = Resources.Load<Texture>("Waiting Players/Waiting Player" + i);
        }
        // 빈 셀
        for(; i < MaxPlayerPerRoom; i++)
        {
            _playerKeys[i].text = "";
            _playerNames[i].text = "";
            _playerCharacters[i].texture = null;
        }

        if(LobbyManager.SocketId.Equals(PlayerKeys[0]))
            _gameStartButton.interactable = true;
        else
            _gameStartButton.interactable = false;
    }

    public void ShowSpeechBubble(Packet.ReceivingChat receivingChat)
    {
        string speecher = receivingChat.PlayerKey;

        for(int i = 0; i < PlayerKeys.Length; i++)
        {
            if(PlayerKeys[i].Equals(speecher))
            {
                SpeechBubble[i].transform.localScale = new Vector3(1, 1, 1);
                SpeechMessage[i].text = receivingChat.Message;
                IsSpeeching[i] = true;
            }
        }
    }

    // 방 안에서 현재 플레이어의 인덱스를 얻음
    public int GetPlayerIndex()
    {
        int index = -1;

        for(int i = 0; i < PlayerKeys.Length; i++)
        {
            if(LobbyManager.SocketId.Equals(PlayerKeys[i]))
            {
                index = i;
                break;
            }
        }

        return index;
    }
}
