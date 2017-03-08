using UnityEngine;
using System.Collections;

public class PlayerControl : CharacterControl {

	//REFS
	public GameObject mainCamera;
	private CameraRotationHorizontal cameraRotationH;
	private CameraRotationVertical cameraRotationV;
	private CameraLockOnRotation cameraLockOnRotation;
	private CameraFollowRotation cameraFollowRotation;

	protected override void Awake () {
		base.Awake();
		cameraRotationH = mainCamera.GetComponent<CameraRotationHorizontal> ();
		cameraRotationV = mainCamera.GetComponent<CameraRotationVertical> ();
		cameraLockOnRotation = mainCamera.GetComponent<CameraLockOnRotation> ();
		cameraFollowRotation = mainCamera.GetComponent<CameraFollowRotation> ();
	}
	public override void SetDirection (Vector3 dir)
	{
		base.SetDirection (dir);
		if (lockOn) {
			direction = dir;
		}
		else {
			direction = dir.x * mainCamera.transform.right + dir.z*mainCamera.transform.forward;
			direction.y=0;

		}
	}

	//ACTIONS
	public override void Attack(){
		base.Attack ();
	}
	public override void JumpAttack(){
		base.JumpAttack ();
	}
	public override void Roll(){
		if (lockOn) {
			Vector3 tempDir = direction.x * mainCamera.transform.right + direction.z*mainCamera.transform.forward;
			tempDir.y=0;
			animator.SetFloat ("RotationDirectionX", tempDir.x);
			animator.SetFloat ("RotationDirectionY", tempDir.z);
		} else {
			animator.SetFloat ("RotationDirectionX", direction.x);
			animator.SetFloat ("RotationDirectionY", direction.z);
		}
		animator.SetTrigger("Roll");
	}
	//STATE TRANSITIONS
	protected override void StateToDefault(){
		base.StateToDefault ();
		cameraRotationH.enabled = true;
		cameraRotationV.enabled = true;
		cameraLockOnRotation.enabled=false;
		cameraFollowRotation.enabled=true;
	}
	protected override void StateToLockOn(){
		base.StateToLockOn ();
		cameraRotationH.enabled = false;
		cameraRotationV.enabled = true;
		cameraLockOnRotation.enabled=true;
		cameraFollowRotation.enabled=false;
	}
	protected override void StateToAttack(){
		base.StateToAttack ();
		cameraFollowRotation.enabled=false;
	}
	protected override void StateToAction(){
		base.StateToAction ();
	}
}
