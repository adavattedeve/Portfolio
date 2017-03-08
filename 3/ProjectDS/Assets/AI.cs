using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour, ICharacterInput {
	public
		enum AiState
	{
		AGRESSIVE,
		PASSIVE
	}

	//Params
	public float attackFrequency=0.25f;
	public float attackRange = 2f;
	public Vector2 rollAttackRange = new Vector2(3,4);
	public Vector2 circlingRange = new Vector2(4,7);
	public Vector2 circlingMovementTime = new Vector2(1.5f,5f);
	public Vector2 agressiveTime = new Vector2(4, 8);
	public Vector2 passiveTime = new Vector2(12, 16);
	//Refs
	private EnemyControl control;

	//variables
	public AiState state;
	private float attackTimer;
	private float distance;
	private Vector3 direction;

	private IEnumerator passiveMovement;
	private IEnumerator stateSwitching;
	private bool ingoreAI;
	void Awake(){
		control = GetComponent<EnemyControl> ();
		attackTimer = 0;
		ingoreAI = false;
	}
	void Start(){
		control.ToLockOn ();
		passiveMovement = MovementInput ();
		stateSwitching = StateSwitching ();
		StartCoroutine (stateSwitching);
	}
	void Update(){

	}
	private IEnumerator MovementInput(){
		while (true) {
			direction=Vector3.zero;
			if (distance>=circlingRange.y*circlingRange.y){
				direction=Vector3.forward;
			}else if (distance<=circlingRange.x*circlingRange.x){
				direction=-Vector3.forward;
			}
			direction.x = Random.Range(-1,2);

			float waitTime = Random.Range(circlingMovementTime.x, circlingMovementTime.y);
			yield return new WaitForSeconds(waitTime);
		}
	}
	private IEnumerator StateSwitching(){
		while (true) {
			ToPassive();
			yield return new WaitForSeconds(Random.Range(passiveTime.x, passiveTime.y));
			ToAgressive();
			yield return new WaitForSeconds(Random.Range(agressiveTime.x, agressiveTime.y));
		}
	}
	public void IngoreInput (float t){
		StartCoroutine (DisableAI(t));
	}
	private IEnumerator DisableAI(float t){
		ingoreAI = true;
		yield return new WaitForSeconds (t);
		ingoreAI = false;
	}
	private void ToPassive(){
		StartCoroutine(passiveMovement);
		state = AiState.PASSIVE;
	}
	private void ToAgressive(){
		StopCoroutine (passiveMovement);
		state = AiState.AGRESSIVE;
	}
	public void RefreshInput(){
		if (ingoreAI)
			return;
		if (control.LockOnTarget != null) {
			distance = Vector3.SqrMagnitude (transform.position - control.LockOnTarget.transform.position);
			switch (state) {
			case AiState.AGRESSIVE:
				control.Block(false);
				direction = Vector3.forward;
				if (distance <= attackRange * attackRange) {
					if (attackTimer > attackFrequency){
						attackTimer=0;
						control.Attack ();
						//StartCoroutine(DisableAI(0.4f));
					}
				}else if (distance>=rollAttackRange.x*rollAttackRange.x && distance<=rollAttackRange.y*rollAttackRange.y){
					control.SetDirection(transform.forward);
					control.Roll();
					//StartCoroutine(DisableAI(0.5f));
				}
				break;
			case AiState.PASSIVE:
				control.Block(true);
				if (distance<=attackRange*attackRange){
					control.SetDirection(-transform.forward);
					control.Roll();
					//StartCoroutine(DisableAI(0.5f));

				}
				break;
			}
			attackTimer += Time.deltaTime;
		}
		else {
			control.ToLockOn();
			direction = Vector3.zero;
		}

		control.SetDirection (direction);
	}
}
