using UnityEngine;
using System.Collections;

public class CharacterMovement : StateMachineBehaviour {
	private float timer;
	private float stoppingTimer;
	private Rigidbody rB;

	public float accerelationTime;
	public float stoppingTime;
	public float maxSpeed;
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (rB == null) {
			rB = animator.GetComponent<Rigidbody> ();
			timer=0;
		}
		stoppingTimer=0;
	}
	
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (animator.GetFloat ("h") == 0 && animator.GetFloat ("v") == 0) {
			stoppingTimer+=Time.deltaTime;
			animator.SetFloat ("Speed", Mathf.Lerp(animator.GetFloat("Speed"), 0, stoppingTimer/stoppingTime));
			if ( animator.GetFloat("Speed")==0){
				timer=0;
			}
		}
		else {
			stoppingTimer=0;
			timer+=Time.deltaTime;
			animator.SetFloat ("Speed",Mathf.Lerp(0, 1f, timer/accerelationTime));
		}
	}
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//rB.MovePosition (rB.transform.position+rB.transform.forward*Time.deltaTime*animator.GetFloat("Speed")*maxSpeed);
		//rB.velocity = rB.transform.forward*animator.GetFloat("Speed")*maxSpeed;
		rB.AddForce(rB.transform.forward*animator.GetFloat("Speed")*maxSpeed,ForceMode.VelocityChange);
	}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
