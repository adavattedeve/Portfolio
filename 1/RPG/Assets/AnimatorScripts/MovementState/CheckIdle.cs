using UnityEngine;
using System.Collections;

public class CheckIdle : StateMachineBehaviour {

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (animator.GetInteger ("Action") == 0 && 
			animator.GetFloat ("h") == 0 && 
			animator.GetFloat ("v") == 0){

			animator.SetBool ("Idle", true);
		} else {
			animator.SetBool ("Idle", false);
		}
	}

}
