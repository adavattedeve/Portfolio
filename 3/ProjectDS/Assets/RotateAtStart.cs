using UnityEngine;
using System.Collections;

public class RotateAtStart : StateMachineBehaviour {
	[Range(0f,1f)]public float speed=0.08f;
	public float maxAngle=180f;
	public float rotationTime=0.15f;
	private float currentRotationTime;
	private float timer;
	private Transform transform;
	private Transform cameraTransform;
	private Quaternion targetRot;
	private Quaternion startRot;
	
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (transform == null) {
			transform = animator.transform;
		}
		if (cameraTransform == null) {
			cameraTransform = Camera.main.transform;
		}
		timer = 0;
		currentRotationTime = rotationTime;
//		Vector3 forw = cameraTransform.forward;
//		forw.y = 0;
//		Vector3 right = cameraTransform.right;
//		right.y = 0;
//		Vector3 targetDirection = right * animator.GetFloat ("H")  + forw * animator.GetFloat ("V");
		Vector3 targetDirection = new Vector3 (animator.GetFloat ("RotationDirectionX") ,0, animator.GetFloat ("RotationDirectionY"));
		if (targetDirection == Vector3.zero) {
			targetDirection = transform.forward;
		}

		float angle = Vector3.Angle (transform.forward, targetDirection);
		currentRotationTime = rotationTime * ( Mathf.Clamp(angle / maxAngle, 1f, 100f));
		startRot = transform.rotation;
		targetRot = Quaternion.LookRotation (targetDirection);
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (timer < rotationTime){
			transform.rotation =  Quaternion.Lerp(startRot, targetRot, timer/currentRotationTime);
		}
		timer += Time.deltaTime;
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
