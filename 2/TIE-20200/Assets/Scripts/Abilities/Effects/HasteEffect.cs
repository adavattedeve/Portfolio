using UnityEngine;
using System.Collections;

public class HasteEffect : Effect {
	private int deltaMovement;
	public override void Initialize (Unit _owner, int _duration)
	{
		name = "Haste";
		base.Initialize (_owner, _duration);

	}
	public override void OnEffectApplied ()
	{
		owner.stats.GetStat (StatType.MOVEMENT).Value+=2;
	}
	public override void OnEffectEnd ()
	{
		owner.stats.GetStat (StatType.MOVEMENT).Value-=2;
	}
}
