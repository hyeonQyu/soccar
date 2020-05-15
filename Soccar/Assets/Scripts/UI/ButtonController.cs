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

    public void OnClickRefresh()
    {
        try
        {
            NetworkManager.Send("room_list", LobbyManager.PlayerId);
        }
        catch(NullReferenceException e)
        {
            Debug.LogError("OnClickRefresh: Send 실패");
        }
    }

    public void OnClickCreateRoom()
    {
        GameObject createRoomPanel = GameObject.Find("Create Room Panel");
        InputField inputRoomName = createRoomPanel.transform.Find("InputField").GetComponent<InputField>();

        // 방만들기 패널 활성화(애니메이션)
        inputRoomName.text = "";
        createRoomPanel.GetComponent<Animator>().Play("Create Room Panel");
    }

    // 방만들기 패널 안의 Cancel 버튼
    public void OnClickCreateRoomCancel()
    {
        GameObject createRoomPanel = GameObject.Find("Create Room Panel");     

        // 방만들기 패널 비활성화(애니메이션)
        createRoomPanel.GetComponent<Animator>().Play("Destroy Room Panel");  
    }

    public void OnClickEnterRoom()
    {

    }
}
