using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootGaugeManager : MonoBehaviour
{
    private float _height;
    private Image _power;
    private Camera _camera;

    void Start()
    {
        _height = 1.57f;
        _power = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Update()
    {
        if(!PlayerController.IsPlayersInitialized)
            return;

        transform.position = _camera.WorldToScreenPoint(PlayerController.Player.transform.position + new Vector3(0, _height, 0));
        
        if(PlayerController.IsShooting)
        {
            transform.localScale = new Vector3(1, 1, 1);
            float shootPower = Input.GetAxis("Sensitivity") * 100;
            _power.rectTransform.sizeDelta = new Vector2(shootPower, _power.rectTransform.sizeDelta.y);
        }
        else
        {
            //gameObject.SetActive(false);
            transform.localScale = new Vector3(0, 0, 0);
            _power.rectTransform.sizeDelta = new Vector2(0, _power.rectTransform.sizeDelta.y);
        }
    }
}
