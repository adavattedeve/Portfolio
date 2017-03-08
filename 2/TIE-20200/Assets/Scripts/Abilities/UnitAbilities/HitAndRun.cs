using UnityEngine;
using System.Collections;
[System.Serializable]
public class HitAndRun : Ability, IOnAttackTrigger {
	public void OnAttack(AttackInfo attackInfo){
		attackInfo.retalition.target.actionPoints++;
		attackInfo.retalition.target.ableToAttack = false;
		attackInfo.retalition.target.ableToMove = true;
	}
	public void OnRetalitionAttack(AttackInfo attackInfo){
	}
}
