using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private bool _isScored;
    public bool IsScored
    {
        set
        {
            _isScored = value;
        }
        get
        {
            return _isScored;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("공 ID: " + gameObject.GetInstanceID());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
