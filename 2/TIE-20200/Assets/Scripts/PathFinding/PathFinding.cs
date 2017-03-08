using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathFinding : MonoBehaviour {
	//public bool useSimplifiedPaths;
	public int maximumNodes;
	//PathRequestManager requestManager;
	Grid grid;

	void Awake() {
		//requestManager = GetComponent<PathRequestManager>();
		grid = GetComponent<Grid>();

	}

//	public void StartFindPath(Vector3 startPos, Vector3 targetPos) {
//		StartCoroutine(FindPath(startPos,targetPos));
//	}
	public bool IsValidMovement(Node startNode, Node targetNode){
		int movement = startNode.Unit.stats.GetStat (StatType.MOVEMENT).Value*10;
		Node[] path = FindPath (startNode.worldPosition, targetNode.worldPosition);
		if (path == null || path.Length==0) {
			return false;
		}
		movement -= GetDistance( startNode, path[0]);
		for (int i=0; i< path.Length-2; ++i) {
			movement -= GetDistance( path[i], path[i+1]);
		}
		if (movement>0) {
			return true;
		}
		return false;
	}
	public Node[] FindPath(Vector3 start, Vector3 target) {

		bool pathSuccess = false;
		Node startNode = grid.NodeFromWorldPoint (start);
		Node targetNode = grid.NodeFromWorldPoint (target);
		Heap<Node> openSet= new Heap<Node>(grid.MaxSize);
		HashSet<Node> closedSet= new HashSet<Node>();
		openSet.Add(startNode);
		int nodeCount=-1;
			while (openSet.Count > 0) {
				++nodeCount;
				if (nodeCount>maximumNodes){
					break;
				}
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);

				if (currentNode == targetNode) {
					pathSuccess = true;
					break;
				}
				Node[] neighbours = grid.GetNeighbours(currentNode);
				for (int i=0; i<neighbours.Length; ++i){
					if (neighbours[i]==null){
						continue;
					}
					if (!neighbours[i].walkable || closedSet.Contains(neighbours[i])) {
								if (neighbours[i] != targetNode){
									continue;
								}
						}
				if (neighbours[i].Unit!=null && (neighbours[i].Unit.Visible || CombatManager.instance.IsThisUnitsTurn(neighbours[i].Unit))){
						if (neighbours[i] != targetNode){
							continue;
						}
					}
					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbours[i]);
					if (newMovementCostToNeighbour < neighbours[i].gCost || !openSet.Contains(neighbours[i])) {
						neighbours[i].gCost = newMovementCostToNeighbour;
						neighbours[i].hCost = GetDistance(neighbours[i], targetNode);
						neighbours[i].parent = currentNode;
						if (!openSet.Contains(neighbours[i]))
							openSet.Add(neighbours[i]);
					}
				}

			}
//		yield return null;
		if (pathSuccess) {
			return RetracePath (startNode, targetNode);
			//requestManager.FinishedProcessingPath (waypoints, pathSuccess);
		} else {
			return null;
			//requestManager.FinishedProcessingPath (waypoints, pathSuccess);
		}


	}

	Node[] RetracePath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();

		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		Node[] wayPoints = path.ToArray();
		//simplify
		Array.Reverse (wayPoints);
		return wayPoints;
			
	}
//
//	Vector3[] SimplifyPath(Node[] path) {
//		List<Vector3> waypointList = new List<Vector3> ();
//		Vector2 directionOld = Vector2.zero;
//		Vector2 directionNew;
//		Vector2 directionNext;
//		Array.Reverse(path);
//			for (int i = 1; i < path.Length; i ++) {
//				waypointList.Add(path [i].worldPosition);
//			}
//		return waypointList.ToArray();
//	}

	int GetDistance(Node nodeA, Node nodeB) {
		int currentX = nodeA.gridX;
		int currentY = nodeA.gridY;
		int dirX = nodeB.gridX - nodeA.gridX;
		int dirY = nodeB.gridY - nodeA.gridY;
		int dstX = Mathf.Abs(dirX);
		int dstY = Mathf.Abs(dirY);
		dirX = Mathf.Clamp (dirX, -1, 1);
		dirY = Mathf.Clamp (dirY, -1, 1);
		int distance = 0;
		while (dstX>0 || dstY>0) {
			if (dstX>0 && dstY>0){
				currentX+=dirX;
				currentY+=dirY;
				dstX--;
				dstY--;
				distance+=(int)(15 *grid.GetNode(currentX, currentY).movementCost);
			}else if(dstX>0){
				currentX+=dirX;
				dstX--;
				distance+=(int)(10*grid.GetNode(currentX, currentY).movementCost);
			}else{
				currentY+=dirY;
				dstY--;
				distance+= (int)(10*grid.GetNode(currentX, currentY).movementCost);
			}
		}
		return distance;
//		if (dstX > dstY) {
//			return 15 * dstY + 10 * (dstX - dstY);
//		}
//		return 15*dstX + 10 * (dstY-dstX);
	}


}
