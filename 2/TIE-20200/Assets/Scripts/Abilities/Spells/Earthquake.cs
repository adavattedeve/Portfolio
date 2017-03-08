using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Earthquake : Spell, ISpellEffectComponent, ISpellDamageComponent{
	private List<Node> targetNodes;
	public void ApplyEffects(List<Node> targets, int intelligence){
		targetNodes = targets;
		float newMovementCost = DataBase.instance.GetTileData (TileID.ROUGH).movementCost;
		for (int i=0; i<targetNodes.Count; ++i) {
			targetNodes[i].movementCost = newMovementCost;
		}
	}
	public int CalculateDamage(int intelligence){
		int damage = stats.GetStat (StatType.DAMAGE).Value + + intelligence * stats.GetStat (StatType.DAMAGESCALING).Value;
		return damage;
	}
	public void EffectVisualization (Node target){
		for (int i=0; i<targetNodes.Count; ++i) {
			targetNodes[i].InitializeFromData (DataBase.instance.GetTileData(TileID.ROUGH));
		}
	}
}
