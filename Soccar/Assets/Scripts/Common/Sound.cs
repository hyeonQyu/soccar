using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound
{
    private GameObject _sounds;

    // 게임
    public AudioSource Whistle { get; private set; }

    // 로비
    public AudioSource BackgroundMusic { get; private set; }

    public Sound(bool isGameScene)
    {
        _sounds = GameObject.Find("Sounds");

        // 게임
        if(isGameScene)
        {    
            Whistle = _sounds.transform.Find("Whistle").GetComponent<AudioSource>();
        }
        // 로비
        else
        {
            BackgroundMusic = _sounds.transform.Find("Background Music").GetComponent<AudioSource>();
        }
    }
}
