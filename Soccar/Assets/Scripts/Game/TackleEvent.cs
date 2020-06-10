using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TackleEvent : MonoBehaviour
{
    private Animator _animator;
    private float _tackleForce;
    // Start is called before the first frame update
    void Start()
    {
        _tackleForce = 5.0f;
        _animator = transform.root.GetChild(0).gameObject.GetComponent<Animator>();
    }
    private void OnCollisionEnter(Collision other)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer.Tackle"))
        {
            if (transform.root.gameObject.GetInstanceID() != other.transform.root.gameObject.GetInstanceID())
            {
                Debug.Log("충돌 당하는 놈 " + other.gameObject.name);
                if (LayerMask.LayerToName(other.gameObject.layer).Equals("RagDoll") && other.gameObject.name.Equals("RightLeg"))
                {
                    Debug.Log("태클 당하는 놈" + other.transform.root.gameObject.name);
                    Vector3 ss = new Vector3(0, 1, 0);
                    Debug.Log("속도no = " + ss * _tackleForce);
                    Debug.Log("속도 = " + ss * _tackleForce * Time.fixedDeltaTime);
                    other.transform.root.GetChild(1).GetChild(0).GetChild(0).position += Vector3.up * _tackleForce * Time.fixedDeltaTime;  // 수정해야함
                }
            }
        }
    }
}
