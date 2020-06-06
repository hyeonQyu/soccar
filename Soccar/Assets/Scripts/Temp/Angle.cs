using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angle : MonoBehaviour
{
    [SerializeField] private GameObject _target;

    private Vector3 direction;
    private float _rotationSpeed;


    // Start is called before the first frame update
    void Start()
    {
        _rotationSpeed = 2f;
        direction = Vector3.zero;
        direction = (_target.transform.position - this.transform.position).normalized;  //방향벡터
        Debug.Log("Direction" + direction);
        
    }

    void FixedUpdate()
    {
        Debug.Log("Before" + transform.rotation);
        
        //this.transform.rotation = Quaternion.LookRotation(direction);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), _rotationSpeed * Time.fixedDeltaTime);
    
        Debug.Log("After" + transform.rotation);
    }
}
