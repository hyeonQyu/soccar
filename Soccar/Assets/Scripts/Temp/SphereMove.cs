using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMove : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private float _speed;
    Vector3 _moveVector3;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _speed = 30f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _moveVector3 = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _rigidbody.AddForce(_moveVector3 * _speed * Time.fixedDeltaTime);
    }
}
