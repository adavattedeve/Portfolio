using UnityEngine;
using System.Collections;
public enum CharacterState{DEFAULT, LOCKON ,ATTACKING, ACTION}
public class CharacterControl : MonoBehaviour {
	private CharacterState state;
	public CharacterState State{set{
			switch (value){
			case CharacterState.ATTACKING:
				StateToAttack();
				break;
			case CharacterState.DEFAULT:
				StateToDefault();
				break;
			case CharacterState.LOCKON:
				StateToLockOn();
				break;
			case CharacterState.ACTION:
				StateToAction();
				break;
			}
			
			state=value;
		}
		get{return state;}}

	//params
	public float blockingSpeedMPL=0.5f;
	public float sprintSpeedMPL=1.25f;

	//REFS
	public Weapon currentWeapon;
	protected ICharacterInput characterInput;
	protected LockOnMovement lockOnMovement;
	protected Movement movement;
	protected Animator animator;
	protected Rigidbody rB;
	protected Targets targetControl;
	protected DealDamage damageDealing;
	protected Health health;
	protected CapsuleCollider characterCollider;
	protected GameObject target;
	public GameObject LockOnTarget{get{return target;}}

	protected Vector3 direction;
	protected bool lockOn;
	protected bool blocking;
	public bool Blocking{get{return blocking;}}
	protected float speed;

	protected virtual void Awake () {
		lockOnMovement = GetComponent<LockOnMovement> ();
		movement = GetComponent<Movement> ();
		animator = GetComponent<Animator> ();
		lockOn = false;
		blocking = false;
		Cursor.visible = false;
		state = CharacterState.DEFAULT;
		rB = GetComponent<Rigidbody> ();
		characterInput = (ICharacterInput)GetComponent (typeof(ICharacterInput));
		direction = Vector3.zero;
		targetControl = GetComponent<Targets> ();
		damageDealing = GetComponent<DealDamage> ();
		health = GetComponent<Health> ();
		characterCollider = GetComponent<CapsuleCollider> ();
	}
	
	// Update is called once per frame
	void Update () {
		characterInput.RefreshInput ();
	}
	void FixedUpdate (){

		switch (state){
		case CharacterState.ATTACKING:
			break;
		case CharacterState.DEFAULT:
			Vector3 currentDir=transform.forward;
			if (Vector3.Angle(currentDir, direction)>120){
				SetTrigger("Turn");
				return;
			}

			movement.Move(direction, speed);
			animator.SetFloat("Speed", speed);
			break;
		case CharacterState.LOCKON:
			lockOnMovement.Move (direction, speed, target);
			animator.SetFloat ("H", direction.x);
			animator.SetFloat ("V", direction.z);
			break;
		}
	}
	public virtual void SetDirection (Vector3 dir){
		speed = Mathf.Abs(dir.z);
		if (Mathf.Abs(dir.x)>speed){
			speed = Mathf.Abs(dir.x);
		}
	}
	public virtual void Attack(){
		if (lockOn) {
			animator.SetFloat("H", direction.x*10);
			animator.SetFloat("V", direction.z*10);
			direction *=10;
			Vector3 tempDir = target.transform.position-transform.position;
			tempDir.y=0;
			animator.SetFloat ("RotationDirectionX", tempDir.x);
			animator.SetFloat ("RotationDirectionY", tempDir.z);
		} else {
			animator.SetFloat ("RotationDirectionX", direction.x);
			animator.SetFloat ("RotationDirectionY", direction.z);
		}
		SetTrigger ("Attack");
	}
	public virtual void Roll(){
		animator.SetFloat ("RotationDirectionX", direction.x);
		animator.SetFloat ("RotationDirectionY", direction.z);;
		SetTrigger("Roll");
	}
	public virtual void Block(bool block){
		if (state == CharacterState.DEFAULT || state == CharacterState.LOCKON) {
			blocking = block;
			if (blocking){
				speed *=blockingSpeedMPL;
				direction *= blockingSpeedMPL;
			}
		}
		else {
			blocking = false;
		}
		animator.SetBool ("Blocking", blocking);
	}
	public virtual void Sprint (bool sprint){
		if (sprint && !blocking && !lockOn) {
			speed *= sprintSpeedMPL;
		}
	}
	public virtual void JumpAttack(){
		if (lockOn) {
			Vector3 tempDir = target.transform.position - transform.position;
			tempDir.y = 0;
			animator.SetFloat ("RotationDirectionX", tempDir.x);
			animator.SetFloat ("RotationDirectionY", tempDir.z);
		} else {
			animator.SetFloat ("RotationDirectionX", direction.x);
			animator.SetFloat ("RotationDirectionY", direction.z);
		}
		SetTrigger ("JumpAttack");
	}
	public virtual void ToLockOn(){
		if (!lockOn) {
			target = targetControl.GetTarget ();
			if (target != null) {
				lockOn = true;
			}else{
				lockOn=false;
			}
		} else {
			lockOn = false;
		}
		animator.SetBool ("LockOn", lockOn);
	}
	public virtual void ChangeTarget(int direction){

		if (lockOn) {
			GameObject newTarget = targetControl.ChangeTarget (direction);
			if (newTarget!=null){
				target = newTarget;
			}
		}
	}
	public void WeaponDamageOn(){
		damageDealing.DealDamageOn (currentWeapon);
	}
	public void DamageOff(){
		damageDealing.DealDamageOff ();
	}
	public void InvulnerableOn(){
		health.invulnerable = true;
	}
	public void InvulnerableOff(){
		health.invulnerable = false;
	}
	public virtual void Blocked(){
		SetTrigger("Blocked");
	}
	// when angle = 0 --> damage from straight forward
	public virtual void GetHit(float angle){
		animator.SetFloat ("GotHitAngle", angle);
		SetTrigger("GotHit");
		Debug.Log ("Got hit at angle: " + angle);
		damageDealing.DealDamageOff ();
	}
	// when angle = 0 --> damage from straight forward
	public virtual void Die(float angle){
		damageDealing.DealDamageOff ();
		SetTrigger("Dead");
		rB.isKinematic = true;
		characterCollider.enabled = false;

		enabled = false;
	}
	protected virtual void StateToDefault(){
		//ResetTriggers ();
		direction = Vector3.zero;
		lockOn = false;
		animator.applyRootMotion=false;
		animator.SetBool ("Blocking", blocking);
	}
	protected virtual void StateToLockOn(){
		//ResetTriggers ();
		direction = Vector3.zero;
		lockOn = true;
		animator.applyRootMotion=false;
		animator.SetBool ("Blocking", blocking);
	}
	protected virtual void StateToAttack(){
		ResetTriggers ();
		animator.applyRootMotion=true;
		animator.SetBool ("Blocking", false);
	}
	protected virtual void StateToAction(){
		ResetTriggers ();
		damageDealing.DealDamageOff ();
		animator.applyRootMotion=true;
		animator.SetBool ("Blocking", false);
	}
	private void SetTrigger(string triggerName){
		ResetTriggers ();
		animator.SetTrigger (triggerName);

	}
	private void ResetTriggers(){
		animator.ResetTrigger ("Attack");
		animator.ResetTrigger ("JumpAttack");
		animator.ResetTrigger ("Roll");
		animator.ResetTrigger ("Turn");
	}
}
