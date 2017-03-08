using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Flying : Ability, IOverrideMovement {
	private Grid grid;
	public List<Node> OnCheckValidMovement(Node node){
		if (grid ==null){
			grid = CombatManager.instance.GetComponent<Grid>();
		}
		List<Node> movementTargets = new List<Node> ();
		int minX = node.gridX - node.Unit.stats.GetStat (StatType.MOVEMENT).Value;
		int maxX = node.gridX + node.Unit.stats.GetStat (StatType.MOVEMENT).Value;
		int minY = node.gridY - node.Unit.stats.GetStat (StatType.MOVEMENT).Value;
		int maxY = node.gridY + node.Unit.stats.GetStat (StatType.MOVEMENT).Value;
		minX =Mathf.Clamp (minX, 0,grid.gridSizeX-1);
		maxX = Mathf.Clamp (maxX, 0,grid.gridSizeX-1);
		minY=Mathf.Clamp (minY, 0,grid.gridSizeY-1);
		maxY=Mathf.Clamp (maxY, 0,grid.gridSizeY-1);
		for (int x = minX; x <= maxX; x ++) {
			for (int y = minY; y<=maxY; y ++) {
				if ((grid.GetNode(x,y).Unit==null || (!grid.GetNode(x,y).Unit.Visible && !CombatManager.instance.IsThisUnitsTurn(grid.GetNode(x,y).Unit))) && grid.GetNode (x, y).walkable){
					movementTargets.Add(grid.GetNode(x,y));
				}
			}
		}
		return movementTargets;
	}
	public Node[] FindPath(Vector3 start, Vector3 target, PathFinding pathFinding){
		Debug.Log ("flying path");
		bool pathSuccess = false;
		Node startNode = grid.NodeFromWorldPoint (start);
		Node targetNode = grid.NodeFromWorldPoint (target);
		List<Node> path = new List<Node>();
		Node currentNode = startNode;
		while (currentNode!=targetNode) {
			int xDir = Mathf.Clamp (targetNode.gridX - currentNode.gridX, -1, 1);
			int yDir = Mathf.Clamp (targetNode.gridY - currentNode.gridY, -1, 1);
//		int xDir = targetNode.gridX - startNode.gridX;
//		int yDir = targetNode.gridY - startNode.gridY;
			int posX = Mathf.Clamp (currentNode.gridX + xDir, 0, grid.gridSizeX - 1);
			int posY = Mathf.Clamp (currentNode.gridY + yDir, 0, grid.gridSizeY - 1);
			currentNode = grid.GetNode (posX, posY);
			path.Add (currentNode);
		}

		return path.ToArray();
	}
}
