using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalController : MonoBehaviour
{
    Animator _animator;
    public float MoveSpeed = 5f;
    public float RotationSpeed = 360f;
    CharacterController _characterCon;
    // Start is called before the first frame update
    void Start()
    {    
        _animator = GetComponent<Animator>();
        _characterCon = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Smooth Move
        if(direction.sqrMagnitude > 0.01f)
        {
            Vector3 forward = Vector3.Slerp(
                transform.forward,
                direction,
                RotationSpeed * Time.deltaTime/Vector3.Angle(transform.forward, direction)
            );
            transform.LookAt(transform.position + forward);
        }
        _characterCon.Move(direction * MoveSpeed * Time.deltaTime);
        _animator.SetFloat("speed", _characterCon.velocity.magnitude);
        
        if(Input.GetKey (KeyCode.Space))
        {
            _animator.SetTrigger("isJump");
        }
        if(Input.GetKey(KeyCode.D))
        {
            _animator.SetTrigger("isShoot");
        }
        if(Input.GetKey(KeyCode.A))
        {
            _animator.SetTrigger("isTackle");
        }  
    }
}
