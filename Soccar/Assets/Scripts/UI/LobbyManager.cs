using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField]
    private Image _logo;
    [SerializeField]
    private Image _mainScreen;
    [SerializeField]
    private Image _pressAnyKey;

    public static bool IsLogoDestroyed { set; get; }
    private bool _isGameStart;

    // Start is called before the first frame update
    void Start()
    {
        Animator logoAnimator = _logo.GetComponent<Animator>();
        logoAnimator.SetBool("isDestroy", true);

        // Press Any Key 크기 조정
        _pressAnyKey.gameObject.transform.localPosition = new Vector3(0, Screen.height / -2.3f, 0);
        _pressAnyKey.gameObject.transform.localScale = new Vector3(Screen.height / 197.6f, Screen.height / 988f, 1);
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            // Press Any Key 크기 조정
            if(_mainScreen != null)
            {
                _pressAnyKey.gameObject.transform.localPosition = new Vector3(0, Screen.height / -2.3f, 0);
                _pressAnyKey.gameObject.transform.localScale = new Vector3(Screen.height / 197.6f, Screen.height / 988f, 1);
            }
        }
        catch(Exception e) { }

        // 아무 키나 눌르면 Main Screen이 사라지고 게임 시작
        if(IsLogoDestroyed && Input.anyKeyDown && !_isGameStart)
        {
            Destroy(_pressAnyKey.gameObject);
            Animator mainScreenAnimator = _mainScreen.GetComponent<Animator>();
            mainScreenAnimator.SetBool("isStart", true);
            _isGameStart = true;
        }

        /* 게임 시작 */
    }
}
