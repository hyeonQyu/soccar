using UnityEngine;
using System.Collections;
using System;

namespace AnimFollow
{
	public class Limb_AF : MonoBehaviour
	{
		public readonly int version = 7; // The version of this script

		// This script is distributed (automatically by RagdollControl) to all rigidbodies and reports to the RagdollControl script if any limb is currently colliding.

		RagdollControl_AF ragdollControl;
		string[] ignoreCollidersWithTag;
        
		void OnEnable()
		{
			ragdollControl = transform.root.GetComponentInChildren<RagdollControl_AF>();
			ignoreCollidersWithTag  = ragdollControl.ignoreCollidersWithTag;
		}
		
		void OnCollisionEnter(Collision collision)
		{
			bool ignore = false;
			if (!(collision.transform.name == "Ground") && collision.transform.root != this.transform.root)
			{
				//foreach(string ignoreTag in ignoreCollidersWithTag)
				//{
				//	if(collision.transform.tag == ignoreTag)
				//	{
				//		ignore = true;
				//		break;
				//	}
				//}
				string collisionTag = collision.gameObject.tag;
				// Ignore RagDoll collision
				if(collision.transform.root.gameObject.tag.Equals("Player") || collisionTag.Equals("Ball") ||
						collisionTag.Equals("Fense") || collisionTag.Equals("Post") || collisionTag.Equals("Net"))
				{
					ignore = true;
				}

				
				if (!ignore)
				{
					ragdollControl.numberOfCollisions++;
					ragdollControl.collisionSpeed = collision.relativeVelocity.magnitude;
//					Debug.Log (collision.transform.name + "\nincreasing");
				}
			}

			try
			{
				if (transform.root.GetChild(0).gameObject.GetInstanceID() != PlayerController.Player.GetInstanceID())
				{
					return;
				}
			}
			catch(NullReferenceException e)
			{
				return;
			}
            if (transform.root.gameObject.GetInstanceID() == collision.transform.root.gameObject.GetInstanceID())
                return;
            if (collision.gameObject.tag.Equals("Ground") || collision.gameObject.tag.Equals("Ball") || collision.transform.root.gameObject.tag.Equals("Player"))
				return;
			Vector3 ragdollPosition = transform.root.GetChild(1).GetChild(0).GetChild(0).position;
			PlayerController.AlterEgo.transform.position = new Vector3(ragdollPosition.x, PlayerController.AlterEgo.transform.position.y, ragdollPosition.z);
		}
		
		void OnCollisionExit(Collision collision)
		{
			bool ignore = false;
			if (!(collision.transform.name == "Ground") && collision.transform.root != this.transform.root)
			{
				//foreach(string ignoreTag in ignoreCollidersWithTag)
				//{
				//	if(collision.transform.tag == ignoreTag)
				//	{
				//		ignore = true;
				//		break;
				//	}
				//}
				string collisionTag = collision.gameObject.tag;
				//Ignore RagDoll collision
				if(collision.transform.root.gameObject.tag.Equals("Player") || collisionTag.Equals("Ball") ||
						collisionTag.Equals("Fense") || collisionTag.Equals("Post") || collisionTag.Equals("Net"))
				{
					ignore = true;
				}
				if (!ignore)
				{
					ragdollControl.numberOfCollisions--;
	//				Debug.Log (collision.transform.name + "\ndecreasing");
				}
			}
		}
	}
}
