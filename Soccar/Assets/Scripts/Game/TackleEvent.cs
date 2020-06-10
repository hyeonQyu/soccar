using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TackleEvent : MonoBehaviour
{
    private Animator _animator;
    private float _shootSpeed;
    // Start is called before the first frame update
    void Start()
    {
        _animator = transform.root.GetChild(0).gameObject.GetComponent<Animator>();
        _shootSpeed = 5.0f;
    }
    private void OnCollisionEnter(Collision other)
    {
        if(transform.root.gameObject.GetInstanceID() != other.transform.root.gameObject.GetInstanceID() )
        {
            if(_animator.GetCurrentAnimatorStateInfo(0).fullPathHash == PlayerController.Hash.Tackle && LayerMask.LayerToName(other.gameObject.layer).Equals("RagDoll"))
            {
                Debug.Log("태클 당하는 놈" + other.transform.root.gameObject.name);
                other.rigidbody.AddForce(new Vector3(0, 2000.0f, 0), ForceMode.Impulse);    // 수정해야함
            }
        }
    }
}
