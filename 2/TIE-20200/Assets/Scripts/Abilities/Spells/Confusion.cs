using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Confusion : Spell, ISpellEffectComponent {
	public void ApplyEffects(List<Node> targets, int intelligence){
		for (int i=0; i< targets.Count; ++i){
			if (targets[i].Unit!=null){
				ConfusionEffect effect = new ConfusionEffect();
				effect.Initialize(targets[i].Unit, stats.GetStat(StatType.DURATION).Value+ intelligence/stats.GetStat(StatType.DURATIONSCALING).Value);
			}
		}
	}
	public void EffectVisualization (Node target){}
}
