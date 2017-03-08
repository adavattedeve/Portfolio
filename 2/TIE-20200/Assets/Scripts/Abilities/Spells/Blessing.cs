using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Blessing : Spell, ISpellEffectComponent {
	public void ApplyEffects(List<Node> targets, int intelligence){
		for (int i=0; i< targets.Count; ++i){
			if (targets[i].Unit!=null){
				BlessEffect effect = new BlessEffect();
				effect.Initialize(targets[i].Unit, stats.GetStat(StatType.DURATION).Value+ intelligence/stats.GetStat(StatType.DURATIONSCALING).Value);
			}
		}
	}
	public void EffectVisualization (Node target){}
}
