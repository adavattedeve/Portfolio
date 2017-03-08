using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class HitInfo  {
	public int damage;
	public int deathCount;
	public Node targetsNode;
	public bool targetDead;
	public Unit target;
	public HitInfo(Node _targetsNode, int _damage, int _deathCount){
		targetsNode = _targetsNode;
		if (targetsNode.Unit != null) {
			target = targetsNode.Unit;
		}
		targetDead = false;
		damage = _damage;
		deathCount = _deathCount;
	}
}
