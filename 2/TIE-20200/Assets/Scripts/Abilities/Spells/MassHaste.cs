using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class MassHaste : SpellMultipleSingleVisuals, ISpellEffectComponent {
	public void ApplyEffects(List<Node> _targets, int intelligence){
		List<Node> validTargets = new List<Node> ();
		for (int i=0; i< _targets.Count; ++i){
			if (_targets[i].Unit!=null ){
				validTargets.Add (_targets[i]);
				HasteEffect effect = new HasteEffect();
				effect.Initialize(_targets[i].Unit, stats.GetStat(StatType.DURATION).Value+ intelligence/stats.GetStat(StatType.DURATIONSCALING).Value);
			}
		}
		targets = validTargets;
	}
	public void EffectVisualization (Node target){}
}