using UnityEngine;
using System.Collections;
[System.Serializable]
public class DoubleStrike : Ability, IOnAfterAttacks{
	public void OnAfterAttacks(AttackInfo attackInfo){
		if (!attackInfo.retalition.targetDead && !attackInfo.attack.targetDead && !attackInfo.retalition.target.doubleStriked) {
			attackInfo.retalition.target.doubleStriked=true;
			CombatManager.instance.Attack (attackInfo.retalition.target, attackInfo.attack.target);
		} else {
			attackInfo.retalition.target.doubleStriked=false;
		}
	}
}
