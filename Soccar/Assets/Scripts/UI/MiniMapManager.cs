using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapManager
{
    public GameObject[] Players { get; set; }
    public GameObject[] Balls { get; set; }
    // public GameObject[] GoalPosts { get; set; }

    public GameObject MiniMapGround { get; private set; }

    public MiniMapManager(int headcount)
    {
        Players = new GameObject[headcount];
        Balls = new GameObject[2];

        MiniMapGround = GameObject.Find("Mini Map Ground");
    }
}
