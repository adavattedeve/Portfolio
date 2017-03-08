using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
	private Animator animator;
	private Rigidbody rB;
	private float timer;
	private float stoppingTimer;

	private float deltaTargetRotation;
	private Vector3 targetDirection;

	private bool moving;
	public bool Moving{set{moving =value;}}


	public float maxSpeed;
	public float stoppingTime;
	public float accerelationTime;

	[Range(0f, 1f)]public float rotatingSpeed;
	void Awake(){
		animator = GetComponent<Animator> ();
		rB = GetComponent<Rigidbody> ();
		moving = false;
	}

	void FixedUpdate(){

		if (moving) {
			targetDirection.x = animator.GetFloat ("h");
			targetDirection.y = 0;
			targetDirection.z = animator.GetFloat ("v");
			if (targetDirection != Vector3.zero) {
				deltaTargetRotation = Vector3.Angle (transform.forward, targetDirection);
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
			rB.AddForce(rB.transform.forward*animator.GetFloat("Speed")*maxSpeed,ForceMode.VelocityChange);
		}
	}

	public IEnumerator MoveRotation(Quaternion targetRotation, float speed){
		float rotTimer=0;
		while (true) {
			rB.MoveRotation(Quaternion.Lerp (rB.rotation ,targetRotation, rotTimer/0.3f));
			if (rotTimer >=1){
				rB.MoveRotation(targetRotation);
				break;}
			yield return new WaitForFixedUpdate ();
			rotTimer+=Time.deltaTime*speed;
		}
	}
}
