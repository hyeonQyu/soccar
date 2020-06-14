using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound
{
    private GameObject _sounds;

    // 게임
    public AudioSource CrowdDefault { get; private set; }
    public AudioSource CrowdGoal { get; private set; }
    public AudioSource Whistle { get; private set; }
    public AudioSource[] KickBall { get; private set; }
    public AudioSource HitPost { get; private set; }
    public AudioSource HitFense { get; private set; }
    public AudioSource BounceBall { get; private set; }
    public AudioSource GoalNet { get; private set; }
    public AudioSource EndWhistle { get; private set; }
    public AudioSource SlidingTackle { get; private set; }
    public AudioSource Fall { get; private set; }
    public AudioSource Jump { get; private set; }
    public AudioSource FeverTimeStart { get; private set; }
    public AudioSource Victory { get; private set; }
    public AudioSource Defeat { get; private set; }

    // 로비


    public Sound(bool isGameScene)
    {
        _sounds = GameObject.Find("Sounds");

        // 게임
        if(isGameScene)
        {
            KickBall = new AudioSource[BallController.TotalBallCount];
            for(int i = 0; i < BallController.TotalBallCount; i++)
                KickBall[i] = _sounds.transform.Find("Kick Ball" + i.ToString()).GetComponent<AudioSource>();
            CrowdDefault = _sounds.transform.Find("Crowd Default").GetComponent<AudioSource>();
            CrowdGoal = _sounds.transform.Find("Crowd Goal").GetComponent<AudioSource>();
            Whistle = _sounds.transform.Find("Whistle").GetComponent<AudioSource>();
            HitPost = _sounds.transform.Find("Hit Post").GetComponent<AudioSource>();
            HitFense = _sounds.transform.Find("Hit Fense").GetComponent<AudioSource>();
            BounceBall = _sounds.transform.Find("Bounce Ball").GetComponent<AudioSource>();
            GoalNet = _sounds.transform.Find("Goal Net").GetComponent<AudioSource>();
            EndWhistle = _sounds.transform.Find("End Whistle").GetComponent<AudioSource>();
            SlidingTackle = _sounds.transform.Find("Sliding Tackle").GetComponent<AudioSource>();
            Fall = _sounds.transform.Find("Fall").GetComponent<AudioSource>();
            Jump = _sounds.transform.Find("Jump").GetComponent<AudioSource>();
            FeverTimeStart = _sounds.transform.Find("Fever Time Start").GetComponent<AudioSource>();
            Victory = _sounds.transform.Find("Victory").GetComponent<AudioSource>();
            Defeat = _sounds.transform.Find("Defeat").GetComponent<AudioSource>();
        }
        // 로비
        else
        {

        }
    }
}
