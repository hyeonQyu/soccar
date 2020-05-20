using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollContorller : MonoBehaviour
{
    Rigidbody _rigidBody;

    private float _vertical;
    private float _horizontal;
    private float _verticalRaw;
    private float _horizontalRaw;
    Vector3 targetRotation;
    public float RotationSpeed = 10;
    public float Speed = 100;
    Animator _animator;
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Direction Axis
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        _horizontalRaw = Input.GetAxisRaw("Horizontal");
        _verticalRaw = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(_horizontal, 0, _vertical);
        Vector3 inputRaw = new Vector3(_horizontalRaw, 0, _verticalRaw);

        if(input.sqrMagnitude>1f)
        {
            input.Normalize();
        }
        
        if(inputRaw!=Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(input).eulerAngles;
        }
        _rigidBody.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(targetRotation.x, Mathf.Round(targetRotation.y/45)*45, targetRotation.z),
            Time.deltaTime*RotationSpeed
            );
        
        if(inputRaw.sqrMagnitude!=0)
        {
            _animator.enabled = true;
        }
        else if(inputRaw.sqrMagnitude == 0)
        {
            _animator.enabled = false;
        }
    }
}
