using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SpellInfo : CombatInfo {
	public List<HitInfo> hitInfos;
	public Spell spell;
	public Node target;
	public SpellInfo(Spell _spell, Node _target){
		spell = _spell;
		target = _target;
		hitInfos = new List<HitInfo> ();
	}
}
