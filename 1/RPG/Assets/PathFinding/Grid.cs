using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public bool displayGridGizmos;
	public LayerMask unwalkableMask;
	public LayerMask input;
	private LayerMask ingoreInput;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	[Range(0.1f, 1f)]public float UnWalkableSphereRadius;
	[Range(0f, 1f)]public float occupiedRangeMultiplier;
	Node[,] grid;

	float nodeDiameter;
	int gridSizeX, gridSizeY;
	private Node[] neighbours;
	void Awake() {
		ingoreInput = ~input;
		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
		neighbours = new Node[9];
		CreateGrid();
	}
	void OnLevelWasLoaded(int level){
		if (level != 0) {
			CreateGrid ();
		}
	}
	public int MaxSize {
		get {
			return gridSizeX * gridSizeY;
		}
	}


	public void OnNodeChange(Vector3 worldCoordinates, bool newWalkable){
		NodeFromWorldPoint (worldCoordinates).walkable=newWalkable;
	}
	public void CreateGrid() {
		grid = new Node[gridSizeX,gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;

		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius*UnWalkableSphereRadius,unwalkableMask));
				if (walkable){
					walkable = Physics.CheckSphere(worldPoint,nodeRadius*UnWalkableSphereRadius, ingoreInput);
				}
				grid[x,y] = new Node(walkable,worldPoint, x,y);
			}
		}
	}

	public Node[] GetNeighbours(Node node) {
		int currentIndex=-1;
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				currentIndex++;
				if (x == 0 && y == 0){
					neighbours[currentIndex]=null;
					continue;}

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					if (x ==0 || y==0){
						neighbours[currentIndex]=grid[checkX,checkY];
						continue;}
					else {
						if (grid[checkX+ (-1)*x,checkY].walkable && grid[checkX,checkY+ (-1)*y].walkable){
							if (!grid[checkX+ (-1)*x,checkY].occupied && !grid[checkX,checkY+ (-1)*y].occupied){
								neighbours[currentIndex]=grid[checkX,checkY];
								continue;
							}
						}
					}
				}
				neighbours[currentIndex]=null;
			}
		}
		return neighbours;
	}
	

	public Node NodeFromWorldPoint(Vector3 worldPosition) {
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		if (x < gridSizeX && y < gridSizeY) {
			if (grid!=null){
			return grid [x, y];
			}else{
				Debug.Log ("grid is null");
				return null;
			}
		}
		else {
			Debug.Log ("NodeFromWorldPoint, coordinates out of range " + x + "  " + y);
			return null;

		}
	}
	
	void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));
		if (grid != null && displayGridGizmos) {
			foreach (Node n in grid) {
				Gizmos.color = (n.walkable)?Color.white:Color.red;
				Gizmos.color = (n.occupied)?Color.cyan:Gizmos.color;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));
			}
		}
	}
}