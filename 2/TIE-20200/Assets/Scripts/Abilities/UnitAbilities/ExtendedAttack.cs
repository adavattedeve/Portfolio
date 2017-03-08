using UnityEngine;
using System.Collections;
[System.Serializable]
public class ExtendedAttack : Ability, IOnAttackTrigger {
	public int extensionRange =1;
	private Grid grid;
	public void OnAttack(AttackInfo attackInfo){
		if (grid == null) {
			grid = GameManager.instance.GetComponent<Grid>();
		}
		Node attacker = grid.UnitsNode(attackInfo.retalition.target);
		Node defender = grid.UnitsNode(attackInfo.attack.target);
		int xDir = Mathf.Clamp (defender.gridX - attacker.gridX, -1, 1);
		int yDir = Mathf.Clamp (defender.gridY - attacker.gridY, -1, 1);
		int posX = defender.gridX+xDir*extensionRange;
		int posY = defender.gridY+yDir*extensionRange;
		Node target = grid.GetNode (posX, posY);

		if (target !=null && target.Unit != null) {
			attackInfo.additionalTargets.Add(new HitInfo(grid.GetNode (posX, posY), 0,0));
		}
	}
	public void OnRetalitionAttack(AttackInfo attackInfo){
		if (grid == null) {
			grid = GameManager.instance.GetComponent<Grid>();
		}
		Node attacker = grid.UnitsNode(attackInfo.retalition.target);
		Node defender = grid.UnitsNode(attackInfo.attack.target);
		int xDir = Mathf.Clamp (attacker.gridX - defender.gridX, -1, 1);
		int yDir = Mathf.Clamp (attacker.gridY - defender.gridY, -1, 1);
		int posX = attacker.gridX+xDir*extensionRange;
		int posY = attacker.gridY+yDir*extensionRange;
		Node target = grid.GetNode (posX, posY);
		
		if (target !=null && target.Unit != null) {
			attackInfo.retalitionAdditionalTargets.Add(new HitInfo(grid.GetNode (posX, posY), 0,0));
		}
	}
}
