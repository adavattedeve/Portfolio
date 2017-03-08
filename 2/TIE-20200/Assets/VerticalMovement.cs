using UnityEngine;
using System.Collections;

public class VerticalMovement : StateMachineBehaviour {
	public float speed=1f;
	public bool clipToFloor=false;
	Transform trans;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (trans == null) {
			trans = animator.transform.parent;
		}
	}
	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (clipToFloor) {
			Vector3 temp = trans.position;
			temp.y=0;
			trans.position = temp;
		}
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//rB.MovePosition (rB.transform.position+rB.transform.forward*Time.deltaTime*animator.GetFloat("Speed")*maxSpeed);
		//rB.velocity = rB.transform.forward*animator.GetFloat("Speed")*maxSpeed;
		trans.Translate (Vector3.up * speed * animator.GetFloat("Speed") * Time.deltaTime);
		//rB.AddForce(rB.transform.forward*animator.GetFloat("Speed")*maxSpeed,ForceMode.VelocityChange);
	}
}
