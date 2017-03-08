using UnityEngine;
using System.Collections;

public class SlowEffect : Effect {
	private int deltaMovement;
	public override void Initialize (Unit _owner, int _duration)
	{
		name = "Slow";
		base.Initialize (_owner, _duration);

	}
	public override void OnEffectApplied ()
	{
		Stat movement = owner.stats.GetStat (StatType.MOVEMENT);
		deltaMovement = movement.Value / 2;
		movement.Value -= deltaMovement;
	}
	public override void OnEffectEnd ()
	{
		owner.stats.GetStat (StatType.MOVEMENT).Value += deltaMovement;
	}
}
