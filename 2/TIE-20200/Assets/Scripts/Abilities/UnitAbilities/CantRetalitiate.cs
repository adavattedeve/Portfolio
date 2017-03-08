using UnityEngine;
using System.Collections;
[System.Serializable]
public class CantRetalitiate : Ability ,IOnDefendTrigger  {
	public void OnDefend(AttackInfo attackInfo){
		attackInfo.isRetalitiated = false;
	}
	public void OnRetalitionAttackDefend(AttackInfo attackInfo){}
}
