using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlagueEffect : Effect {
	int damage;
	int originalDuration;
	public void Initialize (Unit _owner, int _duration, int _damage){
		name = "Plague";
		base.Initialize (_owner, _duration);
		damage = _damage;
		originalDuration = duration;

	}
	public override void OnTurnChange (Node node)
	{
		base.OnTurnChange (node);
		CombatManager.instance.DamageFromNeutralSource (node, damage);
	}
	public override void OnEffectApplied ()
	{
		base.OnEffectApplied ();
	}
	public override void OnEffectEnd ()
	{
		List<Unit> targets = CombatManager.instance.ClosestUnitsInOrder (owner, 1);
		for (int i=0; i<targets.Count; ++i){
			PlagueEffect effect = new PlagueEffect();
			effect.Initialize(targets[i], originalDuration, damage);
		}
		base.OnEffectEnd ();
	}
}
