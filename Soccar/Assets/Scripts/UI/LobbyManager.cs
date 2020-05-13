using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField]
    private Image _logoPanel;
    [SerializeField]
    private Image _mainScreenPanel;
    [SerializeField]
    private Image _pressAnyKey;
    [SerializeField]
    private GameObject _roomList;

    public static bool IsLogoDestroyed { set; get; }
    private bool _isOnLobby;

    private NetworkManager _networkManager;
    private RoomManager _roomManager;

    public static string PlayerId { get; set; }

    private float _preScreenHeight;

    // Start is called before the first frame update
    void Start()
    {
        Animator logoAnimator = _logoPanel.GetComponent<Animator>();
        logoAnimator.SetBool("isDestroy", true);

        // Press Any Key 크기 및 위치 조정
        _pressAnyKey.gameObject.transform.localPosition = new Vector3(0, Screen.height / -2.3f, 0);
        _pressAnyKey.gameObject.transform.localScale = new Vector3(Screen.height / 197.6f, Screen.height / 988f, 1);
        Debug.Log("위치 재종");

        _roomManager = new RoomManager(_roomList);
        _networkManager = new NetworkManager(false, _roomManager);

        // 방 리스트 위치 및 크기 조정
        _roomManager.ShowRoomList(false);

        _preScreenHeight = Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            // Press Any Key 크기 조정
            if(_mainScreenPanel != null)
            {
                _pressAnyKey.gameObject.transform.localPosition = new Vector3(0, Screen.height / -2.3f, 0);
                _pressAnyKey.gameObject.transform.localScale = new Vector3(Screen.height / 197.6f, Screen.height / 988f, 1);
            }
        }
        catch(Exception e) { }

        // 아무 키나 누르면 Main Screen이 사라지고 로그인 화면으로 이동
        if(IsLogoDestroyed && Input.anyKeyDown && !_isOnLobby)
        {
            Destroy(_pressAnyKey.gameObject);
            Animator mainScreenAnimator = _mainScreenPanel.GetComponent<Animator>();
            mainScreenAnimator.SetBool("isDestroy", true);
            _isOnLobby = true;
        }

        /* 로비 */
        // 방 리스트 위치 및 크기 조정
        //if(_preScreenHeight != Screen.height)
        _roomManager.ShowRoomList(false);
    }
}
