using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DealDamage : StateMachineBehaviour {
	private AbilityManager aManager;
	private CharacterStats stats;
	private HitInfo hit;
	private Ray ray;
	private RaycastHit rayHit;
	private ActionBuffer buffer;
	private Ability ability;
	private IDamageable enemy;
	private List<IDamageable> damaged;

	private Vector3 temp;
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (buffer == null) {
			buffer = animator.GetComponent<ActionBuffer> ();
		}
		if (damaged == null) {
			damaged = new List<IDamageable> ();
		}
		if (aManager == null) {
			aManager = animator.GetComponent<AbilityManager> ();
		}
		if (stats == null) {
			stats = animator.GetComponent<CharacterStats> ();
		}
		damaged.Clear ();
		ability = null;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (ability == null) {
			ability = aManager.CurrentlyCasting;
		}
		if (animator.GetBool ("DealingDamage")) {
			for (int i=0; i<stats.WeaponRays.Length; ++i) {

				ray.origin = stats.WeaponRays[i].position;
				ray.direction = stats.WeaponRays[i].forward;
				if (Physics.Raycast (ray, out rayHit, stats.WeaponRayDistances[i], stats.damageLayer)) {
					enemy = rayHit.collider.GetComponent<IDamageable> ();
					if (!damaged.Contains (enemy) && !rayHit.collider.gameObject.CompareTag(animator.gameObject.tag)) {
						if (enemy != null){
							damaged.Add (enemy);
							ApplyDamage (enemy);
						}
						else{
							EffectManager.instance.ImpactSmoke(rayHit.point, EffectSize.SMALL);
						}
					}
				}
			}
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//animator.SetBool ("DealingDamage", false);
			//aManager.CurrentlyCasting = null;
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	private void ApplyDamage (IDamageable target){
		hit.damage = ability.DamageMultiplier * Random.Range (stats.GetStat(StatType.DAMAGEMIN), stats.GetStat(StatType.DAMAGEMAX));
		hit.impact = ability.ImpactMultiplier * stats.GetStat(StatType.IMPACT);
		target.TakeDamage (hit);
	}
}
