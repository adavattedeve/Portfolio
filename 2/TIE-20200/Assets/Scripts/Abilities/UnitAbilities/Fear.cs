using UnityEngine;
using System.Collections;
[System.Serializable]
public class Fear : Ability, IOnAttackTrigger {
	public int fearLength =2;
	private Grid grid;
	public void OnAttack(AttackInfo attackInfo){
		if (grid == null) {
			grid = GameManager.instance.GetComponent<Grid>();
		}
		attackInfo.isRetalitiated = false;
		Node attacker = grid.UnitsNode(attackInfo.retalition.target);
		Node defender = grid.UnitsNode(attackInfo.attack.target);
		int xDir = Mathf.Clamp (defender.gridX - attacker.gridX, -1, 1);
		int yDir = Mathf.Clamp (defender.gridY - attacker.gridY, -1, 1);
		Node target = null;
		for (int i=1; i<fearLength+1; ++i) {
			int posX =  Mathf.Clamp (defender.gridX+xDir*i , 0, grid.gridSizeX-1);
			int posY =  Mathf.Clamp (defender.gridY+yDir*i , 0, grid.gridSizeY-1);
			Node node = grid.GetNode (posX, posY);
			if (node.Unit!=null || !node.walkable){
				break;
			}
			target = node;
		}
		if (target!=null) {
			attackInfo.movementVisualizationAfterAttack = target;
			defender.Unit.unitController.currentMovementTarget=target;
			CombatManager.instance.MoveUnit (defender, target, false);

		}
		attackInfo.isRetalitiated=false;
	}
	public void OnRetalitionAttack(AttackInfo attackInfo){
//		if (grid == null) {
//			grid = GameManager.instance.GetComponent<Grid>();
//		}
//		Node attacker = grid.UnitsNode(attackInfo.defender);
//		Node defender = grid.UnitsNode(attackInfo.attacker);
//		int xDir = Mathf.Clamp (defender.gridX - attacker.gridX, -1, 1);
//		int yDir = Mathf.Clamp (defender.gridY - attacker.gridY, -1, 1);
//		int posX =  Mathf.Clamp (defender.gridX+xDir*fearLength , 0, grid.gridSizeX-1);
//		int posY =  Mathf.Clamp (defender.gridY+xDir*fearLength , 0, grid.gridSizeY-1);
//		CombatManager.instance.MoveUnit (defender, grid.GetNode(posX, posY));
	}
}
