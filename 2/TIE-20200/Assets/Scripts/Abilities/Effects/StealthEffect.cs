using UnityEngine;
using System.Collections;

public class StealthEffect : Effect, IOnTakeDamage, IOnAttackTrigger {
	public override void Initialize (Unit _owner, int _duration)
	{
		name = "Stealth";
		base.Initialize (_owner, _duration);
	}
	public override void OnEffectApplied ()
	{
		base.OnEffectApplied ();

		MonoBehaviour.Instantiate (DataBase.instance.GetVisualEffect(name), owner.unitController.transform.position, Quaternion.identity);
		owner.Visible = false;
	}
	public override void OnEffectEnd ()
	{
		base.OnEffectEnd ();
		owner.Visible = true;
	}

	public void OnAttack(AttackInfo attackInfo){
		attackInfo.attack.damage = (int)(attackInfo.attack.damage * 1.5f);
		EndEffect ();
	}
	public void OnRetalitionAttack(AttackInfo attackInfo){

	}
	public void OnTakeDamage(Unit damageTaker){
		EndEffect ();
	}
}
