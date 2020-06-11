using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TackleEvent : MonoBehaviour
{
    private Animator _animator;
    private float _tackleForce;
    // Start is called before the first frame update
    private GameObject otherRagDoll;
    void Start()
    {
        _tackleForce = 3.0f;
        _animator = transform.root.GetChild(0).gameObject.GetComponent<Animator>();
    }
    private void OnCollisionEnter(Collision other)
    {
        try
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).fullPathHash == PlayerController.Hash.Tackle)
            {
                if (LayerMask.LayerToName(other.gameObject.layer).Equals("RagDoll") &&
                transform.root.gameObject.GetInstanceID() != other.transform.root.gameObject.GetInstanceID())
                {
                    Debug.Log("태클 당하는 놈" + other.transform.root.gameObject.name);
                    otherRagDoll = other.transform.root.GetChild(1).gameObject;
                    if (!otherRagDoll.GetComponent<AnimFollow.RagdollControl_AF>().falling)
                    {
                        otherRagDoll.GetComponent<AnimFollow.RagdollControl_AF>().falling = true;
                        other.transform.root.GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * _tackleForce);
                    }
                }
            }
        }
        catch (Exception) { return; }
    }
}
