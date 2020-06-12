using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TackleEvent : MonoBehaviour
{
    private Animator _animator;
    // Start is called before the first frame update
    private GameObject otherRagDoll;
    private NetworkManager _networkManager;

    void Start()
    {
        _animator = transform.root.GetChild(0).gameObject.GetComponent<Animator>();
        _networkManager = GameObject.Find("Network Manager").GetComponent<NetworkManager>();
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
                    !otherRagDoll.GetComponent<AnimFollow.RagdollControl_AF>().gettingUp)
                    {
                        if(other.transform.root.gameObject.GetComponent<PlayerInformation>().PlayerIndex == PlayerController.PlayerIndex)
                        {
                            PlayerController.InhibitMove = true;
                            // Set avatar's animation to idle
                            other.transform.root.GetChild(0).gameObject.GetComponent<Animator>().SetFloat(PlayerController.Hash.SpeedFloat, 0f);

                            Packet.SendingTackleEvent sendingTackleEvent = new Packet.SendingTackleEvent(PlayerController.PlayerIndex, PlayerController.AlterEgo.transform.position);
                            _networkManager.Send<Packet.SendingTackleEvent>("tackle_event", sendingTackleEvent);
                        }
                    }
                }
            }
        }
        catch (Exception) { return; }
    }
}
