using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapManager
{
    public GameObject[] Players { get; set; }
    public GameObject[] Balls { get; set; }
    public GameObject[] GoalPosts { get; set; }

    public GameObject MiniMap { get; private set; }

    public MiniMapManager(int headcount)
    {
        Players = new GameObject[headcount];
        Balls = new GameObject[BallController.TotalBallCount];
        GoalPosts = new GameObject[headcount];

        MiniMap = GameObject.Find("Mini Map");
        MiniMap.SetActive(true);
    }
}
