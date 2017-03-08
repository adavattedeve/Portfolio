using UnityEngine;
using System.Collections;

public interface IOnAttackTrigger {
	void OnAttack(AttackInfo attackInfo);
	void OnRetalitionAttack(AttackInfo attackInfo);
}
