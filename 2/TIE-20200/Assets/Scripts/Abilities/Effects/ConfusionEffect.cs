using UnityEngine;
using System.Collections;

public class ConfusionEffect : Effect, IOnTakeDamage {
	public override void Initialize (Unit _owner, int _duration)
	{
		name = "Confusion";
		base.Initialize (_owner, _duration);

	}
	public override void OnTurnChange (Node node)
	{
		Debug.Log ("Confusion on turn change");
		base.OnTurnChange (node);
		Debug.Log ("Confusion trigger");
		node.Unit.actionPoints = 0;
	}
	public void OnTakeDamage(Unit unit){
		EndEffect ();
	}
	public override void OnEffectApplied ()
	{
		base.OnEffectApplied ();
	}
	public override void OnEffectEnd ()
	{
		base.OnEffectEnd ();
	}
}
