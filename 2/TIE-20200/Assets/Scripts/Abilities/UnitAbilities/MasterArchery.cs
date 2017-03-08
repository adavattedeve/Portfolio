using UnityEngine;
using System.Collections;
[System.Serializable]
public class MasterArchery : Ability , IOnAttackTrigger{
	public void OnAttack(AttackInfo attackInfo){
		if (!attackInfo.isMelee) {
			attackInfo.attack.damage= (int)(attackInfo.attack.damage/DataBase.instance.gameData.rangePenaltyMpl);
		}
	}
	public void OnRetalitionAttack(AttackInfo attackInfo){
	}
}
