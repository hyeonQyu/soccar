using UnityEngine;
using System.Collections;

namespace AnimFollow
{
	public partial class SimpleFootIK_AF : MonoBehaviour
	{
		[HideInInspector] public static Vector3 GlideFree = Vector3.zero;
		private Vector3 _glideFree2 = Vector3.zero;

		void Awake()
		{
			Awake2();
		}

		void FixedUpdate()
		{
			deltaTime = Time.fixedDeltaTime;
			DoSimpleFootIK();
		}

		void DoSimpleFootIK()
		{	
			if (userNeedsToFixStuff)
			{
				animFollow.DoAnimFollow(); // Only here to make the dead on headshot feature work properly
				return;
			}

			ShootIKRays();

			PositionFeet();

			animFollow.DoAnimFollow();
		}

		// public void OnAnimatorMove()
		// {
		// 	_glideFree2 = Vector3.Lerp(_glideFree2, GlideFree, .05f);
		// 	PlayerController.AlterEgo.transform.position += PlayerController.PlayerAnimators[PlayerController.PlayerIndex].deltaPosition + _glideFree2;
		// 	transform.position += PlayerController.PlayerAnimators[PlayerController.PlayerIndex].deltaPosition + _glideFree2;
		// }
	}
}