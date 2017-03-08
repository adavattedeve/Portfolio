using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathFinding : MonoBehaviour {
	public bool useSimplifiedPaths;
	public int maximumNodes;
	PathRequestManager requestManager;
	Grid grid;

	void Awake() {
		requestManager = GetComponent<PathRequestManager>();
		grid = GetComponent<Grid>();

	}

	public void StartFindPath(Vector3 startPos, Vector3 targetPos, bool noticeOccupied) {
		StartCoroutine(FindPath(startPos,targetPos, noticeOccupied));
	}

	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos, bool noticeOccupied) {

		Vector3[] waypoints=new Vector3[0];
		bool pathSuccess = false;

		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		bool abortPF = false;
		if (!targetNode.walkable) {
			Node[] neighbours = grid.GetNeighbours(targetNode);
			for (int i=0; i<neighbours.Length; ++i){
				if (neighbours[i]==null){
					continue;
				}else if (neighbours[i].walkable){
					abortPF = false;
					targetNode=neighbours[i];
					break;
				}
				abortPF = true;
			}
		}
			Heap<Node> openSet= new Heap<Node>(grid.MaxSize);
			HashSet<Node> closedSet= new HashSet<Node>();
			openSet.Add(startNode);
			int nodeCount=-1;
			while (openSet.Count > 0) {
				++nodeCount;
				if (nodeCount>maximumNodes){
					break;
				}
				else if (abortPF){
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
						continue;
					}
					if (noticeOccupied && neighbours[i].occupied){
						continue;
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
		yield return null;
		if (pathSuccess) {
			waypoints = RetracePath (startNode, targetNode, noticeOccupied);
			requestManager.FinishedProcessingPath (waypoints, pathSuccess);
		} else {
			requestManager.FinishedProcessingPath (waypoints, pathSuccess);
		}


	}

	Vector3[] RetracePath(Node startNode, Node endNode, bool noticeOccupied) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		if (!noticeOccupied) {
			int occupiedIndex =0;
			for (int i=0; i<path.Count; ++i){
					if (path[i].occupied){
						occupiedIndex=i;
					}
			}
			path.RemoveRange(0,occupiedIndex+1);
		}
		Vector3[] waypoints = SimplifyPath(path.ToArray());
		return waypoints;
			
	}

	Vector3[] SimplifyPath(Node[] path) {
		List<Vector3> waypointList = new List<Vector3> ();
		Vector2 directionOld = Vector2.zero;
		Vector2 directionNew;
		Vector2 directionNext;
		Array.Reverse(path);
			for (int i = 1; i < path.Length; i ++) {
				if (useSimplifiedPaths) {
					directionNew = new Vector2 (path [i - 1].gridX - path [i].gridX, path [i - 1].gridY - path [i].gridY);
					if (i + 1 < path.Length) {
						directionNext = new Vector2 (path [i].gridX - path [i + 1].gridX, path [i].gridY - path [i + 1].gridY);
					} else {
						directionNext = Vector2.zero;
					}
					if (directionNew != directionOld) {
					waypointList.Add (path [i].worldPosition);
					} else if (directionNew != directionNext) {
					waypointList.Add (path [i].worldPosition);
					}
					directionOld = directionNew;
			}else {
				waypointList.Add(path [i].worldPosition);
			}
			}
		return waypointList.ToArray();
	}

	int GetDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}


}
