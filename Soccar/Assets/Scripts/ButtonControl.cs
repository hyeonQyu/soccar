using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour
{
    private GameObject _inputField;
    private static InputField _inputID;
    public static InputField InputID
    {
        get
        {
            return _inputID;
        }
    }

    private void Start()
    {
        // 사용자 ID 입력창
        _inputField = GameObject.Find("Input ID");
        _inputID = _inputField.GetComponent<InputField>();
    }

    public void OnClickedStartButton()
    {
        GameLauncher.IsClickedStart = true;
        GameObject btnStart = GameObject.Find("StartButton");

        btnStart.SetActive(false);
        _inputField.SetActive(false);
    }

    public static void Destroy()
    {
        _inputID = null;
    }
}
