using UnityEngine;
using System.Collections;

public class CharacterRotation : StateMachineBehaviour {
	private Rigidbody rB;
	private float deltaTargetRotation;
	private Vector3 targetDirection;
	private Transform goTransform;
	[Range(0f, 1f)]public float rotatingSpeed;
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (rB == null) {
			rB = animator.GetComponent<Rigidbody> ();
		}
		if (goTransform == null) {
			goTransform = animator.transform;
		}
	}
	
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		targetDirection.x = animator.GetFloat ("h");
		targetDirection.y = 0;
		targetDirection.z = animator.GetFloat ("v");
		if (targetDirection != Vector3.zero) {
			deltaTargetRotation = Vector3.Angle (goTransform.forward, targetDirection);
		} else {
			deltaTargetRotation = 0;
		}
	if (deltaTargetRotation > 1f) {
			rB.MoveRotation(Quaternion.Lerp (rB.rotation ,Quaternion.LookRotation(targetDirection), rotatingSpeed));
			//if (Vector3.Cross (goTransform.forward, targetDirection).y > 0) {
				//rB.MoveRotation(Quaternion.Lerp (Quaternion.LookRotation(targetDirection) ,rB.rotation, deltaTargetRotation/181));
				//goTransform.Rotate(Vector3.up * rotatingForce*(deltaTargetRotation/150)*Time.deltaTime);
				//rB.AddTorque (Vector3.up * rotatingForce*(deltaTargetRotation/150)*Time.deltaTime, ForceMode.VelocityChange);
			}
//			else{
//				goTransform.Rotate(Vector3.down * rotatingForce*(deltaTargetRotation/150)*Time.deltaTime);
//				//rB.AddTorque (Vector3.down * rotatingForce*(deltaTargetRotation/150)*Time.deltaTime, ForceMode.VelocityChange);
//			}
//		}
		else if (targetDirection!=Vector3.zero){
			rB.MoveRotation(Quaternion.LookRotation(targetDirection));
		}

	}
	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	
	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
//	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

//	}
	
	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
