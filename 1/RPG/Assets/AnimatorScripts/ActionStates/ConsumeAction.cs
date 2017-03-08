using UnityEngine;
using System.Collections;

public class ConsumeAction : StateMachineBehaviour {
	private AbilityManager aManager;
	private ActionBuffer buffer;
	private Rigidbody rB;
	private Movement movement;
	public float rotationSpeed;
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (aManager == null) {
			aManager = animator.GetComponent<AbilityManager>();
		}
		if (buffer == null) {
			buffer = animator.GetComponent<ActionBuffer> ();
		}
		if (movement == null) {
			movement = animator.GetComponent<Movement> ();
		}
		if (rB == null) {
			rB = animator.GetComponent<Rigidbody> ();
		}
		aManager.SetCurrentlyCasting (buffer.Action);
		movement.StartCoroutine(movement.MoveRotation (buffer.RotationBuffer, rotationSpeed));
		buffer.ConsumeAction();
		animator.SetInteger ("Action", 0);
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
//	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
//		if (buffer.Action != 0) {
//			animator.SetInteger ("Action", buffer.Action);
//		}
//	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
//	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
//	
//	}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
