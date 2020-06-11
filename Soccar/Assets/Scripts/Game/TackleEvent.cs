using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TackleEvent : MonoBehaviour
{
    private Animator _animator;
    // Start is called before the first frame update
    private GameObject otherRagDoll;
    void Start()
    {
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
                    if (other.gameObject.tag.Equals("Leg") && !otherRagDoll.GetComponent<AnimFollow.RagdollControl_AF>().falling &&
                    !otherRagDoll.GetComponent<AnimFollow.RagdollControl_AF>().gettingUp && !other.gameObject.GetComponent<AnimFollow.Limb_AF>().IsTackled)
                    {
                        Debug.Log("한 번 태클");
                        // Set avatar's animation to idle
                        other.transform.root.GetChild(0).gameObject.GetComponent<Animator>().SetFloat(PlayerController.Hash.SpeedFloat, 0f);
                        // otherRagDoll.GetComponent<AnimFollow.RagdollControl_AF>().falling = true;
                        // 충돌된 오브젝트의 물리를 받도록 조정
                        other.gameObject.GetComponent<AnimFollow.Limb_AF>().IsTackled = true;
                    }
                }
            }
        }
        catch (Exception) { return; }
    }
}
