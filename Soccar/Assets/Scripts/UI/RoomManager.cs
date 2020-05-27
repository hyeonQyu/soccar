using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room
{
    private const int MaxPlayerPerRoom = 6;

    // 실제 정보
    public int Key { get; private set; }
    public string Name { get; private set; }
    public int Headcount { get; private set; }
    public string[] PlayerNames { get; private set; }

    // UI에 관련된 요소
    public GameObject RoomPanel { get; private set; }
    private GameObject _playerList;
    private GameObject[] _players = new GameObject[MaxPlayerPerRoom];
    private Text[] _playerNames = new Text[MaxPlayerPerRoom];
    private RawImage[] _playerCharacters = new RawImage[MaxPlayerPerRoom];

    // 방 패널 출력을 위한 객체 생성자
    public Room(GameObject roomPanel)
    {
        RoomPanel = roomPanel;
        InitializePlayersUiComponents();
    }

    // 로비 패널 출력을 위한 객체 생성자
    public Room(int key, string name, int headcount)
    {
        Key = key;
        Name = name;
        Headcount = headcount;
    }

    // 방 패널의 UI 요소 초기화
    private void InitializePlayersUiComponents()
    {
        _playerList = RoomPanel.transform.Find("Player List").gameObject;
        for(int i = 0; i < MaxPlayerPerRoom; i++)
        {
            _players[i] = _playerList.transform.Find("Player" + i).gameObject;
            _playerNames[i] = _players[i].transform.Find("Name Background").Find("Name").GetComponent<Text>();
            _playerCharacters[i] = _players[i].transform.Find("Character").GetComponent<RawImage>();
        }
    }

    // 현재 방 정보 업데이트
    public void SetRoomInfo(Packet.ReceivingRoomInfo receivingRoomInfo)
    {
        Key = receivingRoomInfo.RoomKey;
        Name = receivingRoomInfo.RoomName;
        PlayerNames = receivingRoomInfo.PlayerNames;

        ShowRoom();
    }

    private void ShowRoom()
    {
        RoomPanel.transform.Find("Room Key").GetComponent<Text>().text = Key.ToString();
        RoomPanel.transform.Find("Room Name").GetComponent<Text>().text = Name;

        // 방에 접속한 플레이어 목록 업데이트
        int i;
        // 현재 방에 있는 플레이어
        for(i = 0; i < PlayerNames.Length; i++)
        {
            _playerNames[i].text = PlayerNames[i];
            _playerCharacters[i].texture = Resources.Load<Texture>("Waiting Players/Waiting Player" + i);
        }
        // 빈 셀
        for(; i < MaxPlayerPerRoom; i++)
        {
            _playerNames[i].text = "";
            _playerCharacters[i].texture = null;
        }
    }
}

public class RoomManager
{
    public List<Room> Rooms { get; private set; }
    private GameObject _roomList;

    public const int MaxRoomCount = 5;

    public RoomManager(GameObject roomList)
    {
        _roomList = roomList;
    }

    // 대기방 정보 업데이트
    public void SetRoomList(Packet.ReceivingRoomList receivingRoomList)
    {
        Rooms = new List<Room>();

        int roomCount = receivingRoomList.RoomNames.Length;
        for(int i = 0; i < roomCount; i++)
        {
            Rooms.Add(new Room(receivingRoomList.RoomKeys[i], receivingRoomList.RoomNames[i], receivingRoomList.Headcounts[i]));
        }

        ShowRoomList(true);
    }

    // UI에 업데이트
    public void ShowRoomList(bool isUpdate)
    {
        for(int i = 0; i < MaxRoomCount; i++)
        {
            GameObject room = _roomList.transform.Find("Room" + i).gameObject;

            Text key = room.transform.Find("Room Key").gameObject.GetComponent<Text>();
            Text name = room.transform.Find("Room Name").gameObject.GetComponent<Text>();
            Text headcount = room.transform.Find("Room Headcount").gameObject.GetComponent<Text>();

            if(i >= Rooms.Count)
            {
                key.text = "";
                name.text = "";
                headcount.text = "";

                continue;
            }

            key.text = Rooms[i].Key.ToString();
            name.text = Rooms[i].Name;
            headcount.text = Rooms[i].Headcount.ToString() + "/6";
        }
    }
}
