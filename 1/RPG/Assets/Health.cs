using UnityEngine;
using System.Collections;

public struct HitInfo{
	public float damage;
	public float impact;
}
public class Health : MonoBehaviour, IDamageable {
	private float currentHealth;
	public float CurrentHealth {get {return currentHealth;}}

	private float currentStability;
	public float CurrentStability {get {return currentStability;}}
	public float stabilityMultiplier;
	public float stabilityResetTime = 5f;
	public int stabilityStageAmount = 2;
	private int stabilityStage;
	CharacterEvents events;
	Animator animator;
	CapsuleCollider coll;
	private CharacterStats stats;
	void Awake () {
		coll = GetComponent<CapsuleCollider> ();
		stats = GetComponent<CharacterStats> ();
		events = GetComponent<CharacterEvents> ();
		animator = GetComponent<Animator> ();
		events.Death += SetAnimatorTrigger;
		events.Death += DisableCollider;
		currentHealth = stats.baseMaxHealth;
		currentStability = stats.baseMaxStability;
		stabilityStage = 0;

	}

	public void TakeDamage(HitInfo hit){
		if (!stats.immortal) {
			currentHealth -= hit.damage;
			if (currentHealth <= 0) {
				Death ();
			}

			currentStability -= hit.impact;
			if (currentStability <= 0) {
				if (stabilityStage < stabilityStageAmount-1) {
					animator.SetInteger ("GotHit", 1);
					++stabilityStage;
					currentStability = Mathf.Pow (stabilityMultiplier, stabilityStage) * stats.GetStat(StatType.STABILITY);
				} else {
					animator.SetInteger ("GotHit", 2);
					currentStability = stats.GetStat(StatType.STABILITY);
					stabilityStage = 0;
				}
			}
			events.TakeHitLaunch ();
			events.LaunchOnHealthChange ();
			StartCoroutine(ResetStability());
		}
	}

	IEnumerator ResetStability(){
		yield return new WaitForSeconds (stabilityResetTime);
		stabilityStage = 0;
		currentStability = stats.GetStat (StatType.STABILITY);
	}

	void DisableCollider(){
		coll.enabled = false;
	}
	void SetAnimatorTrigger(){
		animator.SetBool ("Death", true);
	}
	void Death(){
		events.DeathLaunch ();
	}


	public void Restore (Restoration restoration){
		if (restoration.type == Resource.HEALTH) {
			currentHealth += restoration.amount;
			if (currentHealth> stats.GetStat(StatType.MAXHEALTH)){
				currentHealth=stats.GetStat(StatType.MAXHEALTH);
			}
		}
		events.LaunchOnHealthChange ();
	}
}
