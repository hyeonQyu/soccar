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
    private Room _room;

    public static string PlayerId { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Animator logoAnimator = _logoPanel.GetComponent<Animator>();
        logoAnimator.SetBool("isDestroy", true);

        _roomManager = new RoomManager(_roomList);
        _room = new Room();
        _networkManager = new NetworkManager(false, _roomManager, _room);
    }

    // Update is called once per frame
    void Update()
    {
        // 아무 키나 누르면 Main Screen이 사라지고 로그인 화면으로 이동
        if(IsLogoDestroyed && Input.anyKeyDown && !_isOnLobby)
        {
            Destroy(_pressAnyKey.gameObject);
            Animator mainScreenAnimator = _mainScreenPanel.GetComponent<Animator>();
            mainScreenAnimator.SetBool("isDestroy", true);
            _isOnLobby = true;
        }

        /* 로비 */
    }
}
