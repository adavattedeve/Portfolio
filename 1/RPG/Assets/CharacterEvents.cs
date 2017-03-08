using UnityEngine;
using System.Collections;

public class CharacterEvents : MonoBehaviour {
	private Animator anim;
	private LootSpawning loot;
	public delegate void TakeHitAction();
	public event TakeHitAction TakeHit;

	public delegate void HealthChangeAction();
	public event TakeHitAction OnHealthChange;

	public delegate void DeathAction ();
	public event DeathAction Death;

	private CharacterStats stats;

	public Transform footR;
	public Transform footL;
	public Transform torso;

	void Awake(){
		anim = GetComponent<Animator> ();
		stats = GetComponent<CharacterStats> ();
		loot = GetComponent<LootSpawning> ();
		if (loot) {
			Death += loot.SpawnLoot;
		}
		Death += Destroy;
	}
	public void TakeHitLaunch(){
		TakeHit ();
	}
	public void LaunchOnHealthChange(){
		if (OnHealthChange == null) {
			return;
		}
		OnHealthChange ();
	}
	public void DeathLaunch(){
		Death ();
	}
	void Destroy(){
		Destroy (gameObject, 0.5f);
	}
	//AnimationEvents
	public void ActivateRayCasting(){
			anim.SetBool ("DealingDamage", true);
	}
	public void DeActivateRayCasting(){
			anim.SetBool ("DealingDamage", false);
		}
	public void ImmortalityOn(){
		stats.immortal = true;
	}
	public void ImmortalityOff(){
		stats.immortal = false;
	}
	public void RunStepEvent(int right){
		if (anim.GetFloat ("Speed") > 0.9f) {
			if (right != 1) {
				EffectManager.instance.ImpactSmoke (footR.position, EffectSize.SMALL);
			} else {
				EffectManager.instance.ImpactSmoke (footL.position, EffectSize.SMALL);
			}
		}
	}
	public void RollHitGroundEvent(){
		EffectManager.instance.ImpactSmoke (torso.position, EffectSize.SMALL);
	}
}
