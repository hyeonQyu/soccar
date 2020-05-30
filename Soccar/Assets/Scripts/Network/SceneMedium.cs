using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMedium : MonoBehaviour
{
    public int Port { get; set; }
    public int Headcount { get; set; }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
