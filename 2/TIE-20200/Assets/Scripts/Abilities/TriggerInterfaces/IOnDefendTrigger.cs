using UnityEngine;
using System.Collections;

public interface IOnDefendTrigger {
	void OnDefend(AttackInfo attackInfo);
	void OnRetalitionAttackDefend(AttackInfo attackInfo);
}
