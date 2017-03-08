using UnityEngine;
using System.Collections;
[System.Serializable]
public class UnlimitedRetalitions : Ability ,IOnDefendTrigger{
	public void OnDefend(AttackInfo attackInfo){
		if (attackInfo.isMelee) {
			attackInfo.attack.target.ableToRetaliate = true;
		}
	}
	public void OnRetalitionAttackDefend(AttackInfo attackInfo){}
}
