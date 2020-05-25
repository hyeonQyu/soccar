using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyNetworkLinker
{
    public RoomManager RoomManager { get; private set; }
    public Room Room { get; private set; }
    public GameObject RoomPanel { get; private set; }
    public GameObject AlertPanel { get; private set; }

    public LobbyNetworkLinker(RoomManager roomManager, Room room, GameObject roomPanel, GameObject alertPanel)
    {
        RoomManager = roomManager;
        Room = room;
        RoomPanel = roomPanel;
        AlertPanel = alertPanel;
    }
}
