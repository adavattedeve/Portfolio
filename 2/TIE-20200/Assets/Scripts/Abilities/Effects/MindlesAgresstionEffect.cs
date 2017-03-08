using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MindlesAgresstionEffect : Effect, IOnTakeDamage {
	private Grid grid;
	public override void Initialize (Unit _owner, int _duration)
	{
		name = "Mindles Agression";
		base.Initialize (_owner, _duration);

	}
	public override void OnTurnChange (Node node)
	{
		if (grid == null) {
			grid = CombatManager.instance.GetComponent<Grid>();
		}
		base.OnTurnChange (node);
		if (!CombatManager.instance.IsThisUnitsTurn (node.Unit)) {
			return;
		}
		Unit thisUnit = node.Unit;
		Unit target = CombatManager.instance.ClosestUnit (thisUnit);
		List<Node> validMovementTargets = CombatManager.instance.ValidMovementTargets (node);
		Node[] neighbours = grid.GetNeighbours (grid.UnitsNode(target));
		for (int i=0; i<neighbours.Length; ++i) {
			if (validMovementTargets.Contains(neighbours[i])){
				Debug.Log (node.Unit);
				CombatManager.instance.MoveUnit(node, neighbours[i]);
				CombatManager.instance.Attack(thisUnit, target);
				break;
			}
		}
		thisUnit.actionPoints = 0;
	}
	public void OnTakeDamage(Unit unit){
		EndEffect ();
	}
	public override void OnEffectApplied ()
	{
		base.OnEffectApplied ();
	}
	public override void OnEffectEnd ()
	{
		base.OnEffectEnd ();
	}
}
