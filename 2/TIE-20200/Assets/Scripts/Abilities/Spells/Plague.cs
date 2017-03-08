using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Plague : Spell, ISpellEffectComponent {
	public void ApplyEffects(List<Node> targets, int intelligence){
		for (int i=0; i< targets.Count; ++i){
			if (targets[i].Unit!=null){
				int damage = stats.GetStat(StatType.DAMAGE).Value + stats.GetStat(StatType.DAMAGESCALING).Value*intelligence;
				PlagueEffect effect = new PlagueEffect();
				effect.Initialize(targets[i].Unit, stats.GetStat(StatType.DURATION).Value+ intelligence/stats.GetStat(StatType.DURATIONSCALING).Value, damage);
			}
		}
	}
	public void EffectVisualization (Node target){}
}
