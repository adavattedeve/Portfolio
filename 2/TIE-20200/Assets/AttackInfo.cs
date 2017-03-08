using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AttackInfo : CombatInfo{
	public HitInfo attack;
	public HitInfo retalition;
	public List<HitInfo> additionalTargets;
	public List<HitInfo> retalitionAdditionalTargets;
	public bool isRetalitiated;
	public bool isMelee;
	public Node movementVisualizationAfterAttack;

	public AttackInfo(){
		isRetalitiated=false;
		isMelee = true;
		movementVisualizationAfterAttack = null;
		additionalTargets=new List<HitInfo>();
		retalitionAdditionalTargets = new List<HitInfo> ();
	}
}
