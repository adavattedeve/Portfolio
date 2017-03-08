using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class FireExplosion : Spell, ISpellEffectComponent, ISpellDamageComponent {
	public void ApplyEffects(List<Node> targets, int intelligence){
		for (int i=0; i< targets.Count; ++i){
			BurningTileEffect effect = new BurningTileEffect();
			effect.Initialize(targets[i], stats.GetStat(StatType.DURATION).Value+ intelligence/stats.GetStat(StatType.DURATIONSCALING).Value);
		}
	}
	public int CalculateDamage(int intelligence){
		int damage = stats.GetStat (StatType.DAMAGE).Value + + intelligence * stats.GetStat (StatType.DAMAGESCALING).Value;
		return damage;
	}
	public void EffectVisualization (Node target){
	}
}
