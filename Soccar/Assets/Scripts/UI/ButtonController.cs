using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public void OnClickLogin()
    {
        GameObject loginPanel = GameObject.Find("Login Panel");
        InputField inputIdField = GameObject.Find("Input ID Field").GetComponent<InputField>();
        Button loginButton = GameObject.Find("Login Button").GetComponent<Button>();

        if(inputIdField.text.Equals(""))
            return;
        LobbyManager.PlayerId = inputIdField.text;

        // 로그인과 동시에 게임 방 리스트 요청
        try
        {
            NetworkManager.Send("room_list", LobbyManager.PlayerId);
        }
        catch(NullReferenceException e)
        {
            Debug.LogError("OnClickLogin: Send 실패");
        }

        // 로그인 패널 소멸
        loginPanel.GetComponent<Animator>().SetBool("isDestroy", true);
        Destroy(inputIdField.gameObject);
        Destroy(loginButton.gameObject);
    }
}
