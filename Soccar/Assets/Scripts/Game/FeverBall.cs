using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverBall : MonoBehaviour
{
    private GameObject _followYellowLight;
    private GameObject _star;
    private GameObject _explosion;
    // Start is called before the first frame update
    void Start()
    {
        _followYellowLight = transform.GetChild(0).gameObject;
        _star = transform.GetChild(1).gameObject;
        _explosion = transform.GetChild(2).gameObject;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag.Equals("Ground"))
        {
            _explosion.SetActive(true);
            _followYellowLight.SetActive(false);
            _star.SetActive(false);
        }
    }
}
