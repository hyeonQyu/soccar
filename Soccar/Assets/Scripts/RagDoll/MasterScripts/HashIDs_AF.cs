using UnityEngine;
using System.Collections;

namespace AnimFollow
{
    public class HashIDs_AF : MonoBehaviour
    {
        // Add this script to the master

        public readonly int version = 7; // The version of this script

        // Here we store the hash tags for various strings used in our animators.
        // public int dyingState;
        // public int locomotionState;
        // public int deadBool;
        public int SpeedFloat;
        // public int sneakingBool;
        public int FrontTrigger;
        public int BackTrigger;
        public int FrontMirrorTrigger;
        public int BackMirrorTrigger;

        public int Idle;
        public int Walk;
        public int Run;
        public int Jump;
        public int Tackle;
        public int Shoot;
        public int JumpTrigger;
        public int TackleTrigger;
        public int ShootTrigger;
        public int EndTrigger;

		public int GetupFront;
        public int GetupBack;
        public int GetupFrontMirror;
        public int GetupBackMirror;

        public int AnyStateToGetupFront;
        public int AnyStateToGetupBack;
        public int AnyStateToGetupFrontMirror;
        public int AnyStateToGetupBackMirror;

        void Awake()
        {
            // dyingState = Animator.StringToHash("Base Layer.Dying");
            // locomotionState = Animator.StringToHash("Base Layer.Locomotion");
            // deadBool = Animator.StringToHash("Dead");
            // sneakingBool = Animator.StringToHash("Sneaking");


            // These are used by the RagdollControll script and must exist exactly as below
            SpeedFloat = Animator.StringToHash("SpeedFloat");

            Idle = Animator.StringToHash("Base Layer.Move.Idle");
            Walk = Animator.StringToHash("Base Layer.Move.Walk");
            Run = Animator.StringToHash("Base Layer.Move.Run");

            JumpTrigger = Animator.StringToHash("JumpTrigger");
            TackleTrigger = Animator.StringToHash("TackleTrigger");
            ShootTrigger = Animator.StringToHash("ShootTrigger");
			EndTrigger = Animator.StringToHash("EndTrigger");

            Jump = Animator.StringToHash("Base Layer.Jump");
            Tackle = Animator.StringToHash("Base Layer.Tackle");
            Shoot = Animator.StringToHash("Base Layer.Shoot");

            FrontTrigger = Animator.StringToHash("FrontTrigger");
            BackTrigger = Animator.StringToHash("BackTrigger");
            FrontMirrorTrigger = Animator.StringToHash("FrontMirrorTrigger");
            BackMirrorTrigger = Animator.StringToHash("BackMirrorTrigger");

            GetupFront = Animator.StringToHash("Base Layer.GetupFront");
            GetupBack = Animator.StringToHash("Base Layer.GetupBack");
            GetupFrontMirror = Animator.StringToHash("Base Layer.GetupFronMirror");
            GetupBackMirror = Animator.StringToHash("Base Layer.GetupBackMirror");

            // Use this in Unity older than unity 5
            //			anyStateToGetupFront = Animator.StringToHash("AnyState -> Base Layer.GetupFront"); 
            //			anyStateToGetupBack = Animator.StringToHash("AnyState -> Base Layer.GetupBack"); 
            //			anyStateToGetupFrontMirror = Animator.StringToHash("AnyState -> Base Layer.GetupFrontMirror"); 
            //			anyStateToGetupBackMirror = Animator.StringToHash("AnyState -> Base Layer.GetupBackMirror");

            // Use this in Unity 5
            AnyStateToGetupFront = Animator.StringToHash("Entry -> Base Layer.GetupFront");
            AnyStateToGetupBack = Animator.StringToHash("Entry -> Base Layer.GetupBack");
            AnyStateToGetupFrontMirror = Animator.StringToHash("Entry -> Base Layer.GetupFrontMirror");
            AnyStateToGetupBackMirror = Animator.StringToHash("Entry -> Base Layer.GetupBackMirror");
        }
    }
}
