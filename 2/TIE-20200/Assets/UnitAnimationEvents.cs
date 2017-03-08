using UnityEngine;
using System.Collections;

public class UnitAnimationEvents : MonoBehaviour {
	[System.NonSerialized]public Unit unit;
	private UnitController controller;

	void Awake(){
		controller = GetComponentInParent<UnitController> ();
	}
	//animation event
	public void AttackAnimationDealDamageEvent(){
		if (controller.AttackWithProjectile ()) {
			return;
		}
		unit.state = UnitState.DEFAULT;
	}
	//Animation event
	public void TakeDamageAnimationFinishedEvent(){
		unit.state = UnitState.DEFAULT;
	}
	public void SpawnAttackProjectileEvent(){
		controller.SpawnAttackProjectile ();
	}
}
