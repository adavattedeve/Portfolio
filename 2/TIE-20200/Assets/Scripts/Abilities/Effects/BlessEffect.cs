using UnityEngine;
using System.Collections;

public class BlessEffect : Effect {
	private int deltaMinDamage;
	public override void Initialize (Unit _owner, int _duration)
	{
		name = "Blessing";
		base.Initialize (_owner, _duration);

	}
	public override void OnEffectApplied ()
	{
		Stat damage = owner.stats.GetStat (StatType.DAMAGE);
		deltaMinDamage = damage.Value - damage.AdditionalValue;
		damage.AdditionalValue += deltaMinDamage;
	}
	public override void OnEffectEnd ()
	{
		Stat damage = owner.stats.GetStat (StatType.DAMAGE);
		damage.AdditionalValue -= deltaMinDamage;
	}
}
