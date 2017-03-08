using UnityEngine;
using System.Collections;

public class CharacterMovement : StateMachineBehaviour {
	public float speed=1f;
	private Transform trans;
	private Vector3 targetDirection;
	private float deltaTargetRotation;
	[Range(0f, 1f)]public float rotatingSpeed;
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (trans == null) {
			trans = animator.transform.parent;
		}
	}
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		targetDirection.x = animator.GetFloat ("h");
		targetDirection.y = 0;
		targetDirection.z = animator.GetFloat ("v");
		if (targetDirection != Vector3.zero) {
			deltaTargetRotation = Vector3.Angle (trans.forward, targetDirection);
		} else {
			deltaTargetRotation = 0;
		}
		if (deltaTargetRotation > 1f) {
			trans.rotation = Quaternion.Lerp (trans.rotation ,Quaternion.LookRotation(targetDirection), rotatingSpeed);
		}
		else if (targetDirection!=Vector3.zero){
			trans.rotation = Quaternion.LookRotation(targetDirection);
		}
		
	}
	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//rB.MovePosition (rB.transform.position+rB.transform.forward*Time.deltaTime*animator.GetFloat("Speed")*maxSpeed);
		//rB.velocity = rB.transform.forward*animator.GetFloat("Speed")*maxSpeed;
		if (targetDirection != Vector3.zero) {
			trans.Translate (Vector3.forward * speed * Time.deltaTime);
		}
		//rB.AddForce(rB.transform.forward*animator.GetFloat("Speed")*maxSpeed,ForceMode.VelocityChange);
	}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
