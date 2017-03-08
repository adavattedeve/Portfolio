using UnityEngine;
using System.Collections;
[System.Serializable]
public class MeleePenalty : Ability, IOnAttackTrigger {
	public float penaltyMpl=0.5f;
	public void OnAttack(AttackInfo attackInfo){
		if (attackInfo.isMelee) {
			attackInfo.attack.damage = (int)(attackInfo.attack.damage*penaltyMpl);
		}
	}
	public void OnRetalitionAttack(AttackInfo attackInfo){
		if (attackInfo.isMelee) {
			attackInfo.retalition.damage = (int)(attackInfo.retalition.damage*penaltyMpl);
		}
	}
}
