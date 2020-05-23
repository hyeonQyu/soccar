using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room
{
    public string Name { get; private set; }
    public int Headcount { get; private set; }
    public string[] PlayerNames { get; private set; }
    public GameObject RoomPanel { get; private set; }

    public Room(GameObject roomPanel)
    {
        RoomPanel = roomPanel;
    }

    public Room(string name, int headcount)
    {
        Name = name;
        Headcount = headcount;
    }

    // 현재 방 정보 업데이트
    public void SetRoomInfo(Packet.ReceivingRoomInfo receivingRoomInfo)
    {
        Name = receivingRoomInfo.RoomName;
        PlayerNames = receivingRoomInfo.PlayerNames;

        ShowRoom();
    }

    private void ShowRoom()
    {
        RoomPanel.transform.Find("Room Name").GetComponent<Text>().text = Name;
        // 방에 접속한 플레이어 목록 업데이트
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
            Rooms.Add(new Room(receivingRoomList.RoomNames[i], receivingRoomList.Headcounts[i]));
        }

        ShowRoomList(true);
    }

    // UI에 업데이트
    public void ShowRoomList(bool isUpdate)
    {
        for(int i = 0; i < MaxRoomCount; i++)
        {
            GameObject room = _roomList.transform.Find("Room" + i).gameObject;

            Text index = room.transform.Find("Room Index").gameObject.GetComponent<Text>();
            Text name = room.transform.Find("Room Name").gameObject.GetComponent<Text>();
            Text headcount = room.transform.Find("Room Headcount").gameObject.GetComponent<Text>();

            if(i >= Rooms.Count)
            {
                index.text = "";
                name.text = "";
                headcount.text = "";

                continue;
            }

            index.text = (i + 1).ToString();
            name.text = Rooms[i].Name;
            headcount.text = Rooms[i].Headcount.ToString() + "/6";
        }
    }
}
