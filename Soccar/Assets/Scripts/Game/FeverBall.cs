using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverBall : MonoBehaviour
{
    private GameObject _followYellowLight;
    private GameObject _star;
    [SerializeField]
    private GameObject _explosion;
    private bool _isFinish;
    // Start is called before the first frame update
    void Start()
    {
        _isFinish = false;
        _followYellowLight = transform.GetChild(0).gameObject;
        _star = transform.GetChild(1).gameObject;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!_isFinish)
        {
            _explosion.transform.position = transform.position;
            _explosion.SetActive(true);
            _followYellowLight.SetActive(false);
            _star.SetActive(false);
            _isFinish = true;
        }
    }
    public void TurnOnEffect()
    {
        _star.SetActive(true);
        _followYellowLight.SetActive(true);
        _explosion.SetActive(false);
        _isFinish = false;
    }
}
