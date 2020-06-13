using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMedium : MonoBehaviour
{
    public int Port { get; set; }
    public int Headcount { get; set; }
    public int PlayerIndex { get; set; }
    public string PlayerName { get; set; }
    public bool IsAfterGame { get; set; }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Port = 80;
    }
}
