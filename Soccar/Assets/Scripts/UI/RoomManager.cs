using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager
{
    public const int MaxRoomCount = 5;
    public List<Room> Rooms { get; private set; }

    // 로비 패널 UI에 관련된 요소
    private GameObject _roomList;
    private GameObject[] _rooms = new GameObject[MaxRoomCount];
    private Text[] _roomKeys = new Text[MaxRoomCount];
    private Text[] _roomNames = new Text[MaxRoomCount];
    private Text[] _roomHeadcounts = new Text[MaxRoomCount];
    private GameObject[] _flags = new GameObject[MaxRoomCount];

    public RoomManager(GameObject roomList)
    {
        _roomList = roomList;
        InitializeRoomsUiComponents();
    }

    // 로비 패널의 UI 요소 초기화
    private void InitializeRoomsUiComponents()
    {
        for(int i = 0; i < MaxRoomCount; i++)
        {
            _rooms[i] = _roomList.transform.Find("Room" + i).gameObject;

            _roomKeys[i] = _rooms[i].transform.Find("Room Key").gameObject.GetComponent<Text>();
            _roomNames[i] = _rooms[i].transform.Find("Room Name").gameObject.GetComponent<Text>();
            _roomHeadcounts[i] = _rooms[i].transform.Find("Room Headcount").gameObject.GetComponent<Text>();
            _flags[i] = _rooms[i].transform.Find("Flag").gameObject;
        }
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

    // 로비 패널의 UI에 업데이트
    public void ShowRoomList(bool isUpdate)
    {
        int i;
        // 현재 대기 방
        for(i = 0; i < Rooms.Count; i++)
        {
            _roomKeys[i].text = Rooms[i].RoomKey.ToString();
            _roomNames[i].text = Rooms[i].RoomName.ToString();
            _roomHeadcounts[i].text = Rooms[i].Headcount.ToString() + "/6";
            _flags[i].transform.localScale = new Vector3(1, 1, 1);
        }
        // 빈 셀
        for(; i < MaxRoomCount; i++)
        {
            _roomKeys[i].text = "";
            _roomNames[i].text = "";
            _roomHeadcounts[i].text = "";
            _flags[i].transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
