using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Taunt : Ability , IOnTurnChangeTrigger{
	private Grid grid;
	public void OnTurnChange(Node node){
		if (grid == null) {
			grid = GameManager.instance.GetComponent<Grid>();
		}
		if (CombatManager.instance.IsThisUnitsTurn (node.Unit)) {
			return;
		}
		if (CombatManager.instance.EnemyInNeighbour (node)) {
			Node[] neighbours = grid.GetNeighbours(node);
			List<Node> targets = new List<Node>();
			for (int i=0; i<neighbours.Length; ++i){
				if (neighbours[i]!=null && neighbours[i].Unit!= null && neighbours[i].Unit.owner != node.Unit.owner){
					if (node.Unit!=null){
						targets.Add(neighbours[i]);
					}
				}
			}
			for (int i=0; i<targets.Count; ++i){
				if (node.Unit==null){
					break;
				}
				if (targets[i].Unit==null){
					continue;
				}
				targets[i].Unit.actionPoints--;
				CombatManager.instance.Attack(targets[i].Unit, node.Unit);
			}
		}
	}
}
