using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGrid : MonoBehaviour {

    public bool displayGridGizmos;
    private Node[,] grid;
    private NodeInWorld[,] gridAsWorldNodes;
    public Node[,] GetGrid { get { return grid; } }
    public NodeInWorld[,] GetGridAsWorldNodes
    {
        get {
            return gridAsWorldNodes;
        }
    }
	private Node[] neighbours;
    private float nodeRadius;
    public float NodeDiameter
    {
        get
        {
            return nodeDiameter;
        }
    }
    private float nodeDiameter;
    private IntVector2 gridSize;
    public IntVector2 GridSize
    {
        get
        {
            return gridSize;
        }
    }
    public int MaxSize
    {
        get
        {
            return gridSize.x * gridSize.y;
        }
    }
	void Awake() {
		neighbours = new Node[9];

	}

	//public void OnNodeChange(Vector3 worldCoordinates, bool newWalkable){
	//	NodeFromWorldPoint (worldCoordinates).walkable=newWalkable;
	//}
	public void CreateGrid(MapGenerationData mapData, float _nodeRadius)
    {
        Debug.Log("creatingGrid");
        nodeRadius = _nodeRadius;
        nodeDiameter = 2 * _nodeRadius;
        gridSize = new IntVector2(mapData.mapHeightMap.GetLength(0)-1, mapData.mapHeightMap.GetLength(1) - 1);
        grid = new Node[gridSize.x, gridSize.y];
        gridAsWorldNodes = new NodeInWorld[gridSize.x, gridSize.y];
        for (int y = 0; y < gridSize.y; ++y)
        {
            for (int x = 0; x < gridSize.x; ++x)
            {
                //Choose TerrainType for node
                TerrainType nodeTerrainType = TerrainType.DEFAULT;

                if (mapData.vertexTerrainType[x, y] == TerrainType.WATER ||
                mapData.vertexTerrainType[x + 1, y] == TerrainType.WATER ||
                mapData.vertexTerrainType[x, y + 1] == TerrainType.WATER ||
                mapData.vertexTerrainType[x + 1, y + 1] == TerrainType.WATER)
                {
                    nodeTerrainType = TerrainType.WATER;
                }

                else if (mapData.vertexTerrainType[x, y] == TerrainType.MOUNTAIN ||
                     mapData.vertexTerrainType[x + 1, y] == TerrainType.MOUNTAIN ||
                     mapData.vertexTerrainType[x, y + 1] == TerrainType.MOUNTAIN ||
                     mapData.vertexTerrainType[x + 1, y + 1] == TerrainType.MOUNTAIN)
                {
                    nodeTerrainType = TerrainType.MOUNTAIN;
                }
                else if (mapData.vertexTerrainType[x, y] == TerrainType.FOREST ||
                     mapData.vertexTerrainType[x + 1, y] == TerrainType.FOREST ||
                     mapData.vertexTerrainType[x, y + 1] == TerrainType.FOREST ||
                     mapData.vertexTerrainType[x + 1, y + 1] == TerrainType.FOREST)
                {
                    nodeTerrainType = TerrainType.FOREST;
                }
                //Choose Owner for node
                IntVector2 nodeOwner = mapData.vertexAreaOwner[x, y];
                gridAsWorldNodes[x, y] = new NodeInWorld(new Vector3(x * nodeDiameter, 0, y * nodeDiameter), new IntVector2(x, y), nodeTerrainType, nodeOwner);
                grid[x, y] = (Node)gridAsWorldNodes[x, y];
            }
        }
        //grid = new Node[gridSizeX,gridSizeY];
        //int battlefieldNodeMinX = (gridWithEnvironmentSizeX-gridSizeX) / 2;
        //int battlefieldNodeMaxX = gridWithEnvironmentSizeX - (gridWithEnvironmentSizeX-gridSizeX) / 2;
        //int battlefieldNodeMinY = (gridWithEnvironmentSizeY-gridSizeY) / 2;
        //int battlefieldNodeMaxY = gridWithEnvironmentSizeY - (gridWithEnvironmentSizeY-gridSizeY) / 2;
        //Debug.Log (battlefieldNodeMinX + "  " + battlefieldNodeMaxX + "  " + battlefieldNodeMinY +"  " +battlefieldNodeMaxY);
        //TileData defaultTile = DataBase.instance.GetTileData (TileID.GRASS);
        //Vector3 worldBottomLeft = transform.position - Vector3.right * (gridBattlefieldSize.x+gridEnvironmentExtensionSize.x)/2 - Vector3.forward * (gridBattlefieldSize.y+gridEnvironmentExtensionSize.y)/2;
        //for (int x = 0; x < gridWithEnvironmentSizeX; x ++) {
        //	for (int y = 0; y < gridWithEnvironmentSizeY; y ++) {
        //		Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
        //		if (x>=battlefieldNodeMinX && x<battlefieldNodeMaxX && y>=battlefieldNodeMinY && y<battlefieldNodeMaxY){
        //			bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius*UnWalkableSphereRadius,unwalkableMask));
        //			grid[x-battlefieldNodeMinX,y-battlefieldNodeMinY] = new Node(walkable ,worldPoint, x-battlefieldNodeMinX,y-battlefieldNodeMinY, visualizationTile, DataBase.instance.GetTileData(TileID.GRASS));
        //		}else {
        //			GameObject temp = (MonoBehaviour.Instantiate (defaultTile.prefab) as GameObject);
        //			temp.transform.position = worldPoint;
        //			temp.transform.Rotate (Vector3.up, Random.Range(0,2)*180, Space.World);
        //			temp.GetComponent<MeshRenderer> ().material.SetTexture ("_BumpMap", defaultTile.normalMaps[Random.Range(0,defaultTile.normalMaps.Length)]);;
        //		}

        //	}
        //}

    }
	//public List<Node> GetValidMovement(Node node){
	//	List<Node> movementTargets = new List<Node> ();
	//	int minX = node.gridX - node.Unit.stats.GetStat (StatType.MOVEMENT).Value;
	//	int maxX = node.gridX + node.Unit.stats.GetStat (StatType.MOVEMENT).Value;
	//	int minY = node.gridY - node.Unit.stats.GetStat (StatType.MOVEMENT).Value;
	//	int maxY = node.gridY + node.Unit.stats.GetStat (StatType.MOVEMENT).Value;
	//	minX =Mathf.Clamp (minX, 0,gridSizeX-1);
	//	maxX = Mathf.Clamp (maxX, 0,gridSizeX-1);
	//	minY=Mathf.Clamp (minY, 0,gridSizeY-1);
	//	maxY=Mathf.Clamp (maxY, 0,gridSizeY-1);
	//	for (int x = minX; x <= maxX; x ++) {
	//		for (int y = minY; y<=maxY; y ++) {
	//			if (grid[x,y].Unit==null || (!grid[x,y].Unit.visible && !CombatManager.instance.IsThisUnitsTurn(grid[x,y].Unit))){
	//				if (grid[x,y].walkable && pathFinding.IsValidMovement(node, grid[x,y])){
	//					movementTargets.Add(grid[x,y]);
	//				}
	//			}
	//		}
	//	}
	//	return movementTargets;
	//}

	public bool IsNeighbours(Node node, Node other){

		Node[] neighbours = GetNeighbours (node);
		for (int i=0; i<neighbours.Length; ++i) {
			if (neighbours[i]==other){
				return true;
			}
		}
		return false;
	}
	public Node GetNearestNeighbour(Node node, Vector3 position){
		Vector3 center = node.worldPosition;
		//left sector
		if (center.x - nodeRadius + (nodeDiameter / 3) > position.x) {
			center.x-=nodeDiameter;
			//lower sector
			if (center.z - nodeRadius + (nodeDiameter / 3) > position.z) {
				center.z -=nodeDiameter;
			}
			//upper sector
			else if (center.z + nodeRadius - (nodeDiameter / 3) < position.z) {
				center.z +=nodeDiameter;
			}
		}
		//rigth sector
		else if (center.x + nodeRadius - (nodeDiameter / 3) < position.x) {
			center.x+=nodeDiameter;
			//lower sector
			if (center.z - nodeRadius + (nodeDiameter / 3) > position.z) {
				center.z -=nodeDiameter;
			}
			//upper sector
			else if (center.z + nodeRadius - (nodeDiameter / 3) < position.z) {
				center.z +=nodeDiameter;
			}
		}
		//middle sector
		else {
			//lower sector
			if (center.z - nodeRadius + (nodeDiameter / 3) > position.z) {
				center.z -=nodeDiameter;
			}
			//upper sector
			else if (center.z + nodeRadius - (nodeDiameter / 3) < position.z) {
				center.z +=nodeDiameter;
			}
			else {
				if (center.x> position.x){
					center.x-=nodeDiameter;
				}else{
					center.x+=nodeDiameter;
				}
			}
		}
		return NodeFromWorldPoint(center);
	}
	public Node[] GetNeighbours(Node node) {
		int currentIndex=-1;
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				currentIndex++;
				if (x == 0 && y == 0){
					neighbours[currentIndex]=null;
					continue;}

				int checkX = node.gridIndex.x + x;
				int checkY = node.gridIndex.y + y;

				if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y) {
					if (x ==0 || y==0){
						neighbours[currentIndex]=grid[checkX,checkY];
						continue;}
					else {
						if (grid[checkX+ (-1)*x,checkY].walkable && grid[checkX,checkY+ (-1)*y].walkable){
								neighbours[currentIndex]=grid[checkX,checkY];
								continue;
						}
					}
				}
				neighbours[currentIndex]=null;
			}
		}

		return neighbours;
	}

	public int DistanceBetween(Node nodeA, Node nodeB){
			int x = Mathf.Abs(nodeA.gridIndex.x - nodeB.gridIndex.x);
			int y = Mathf.Abs(nodeA.gridIndex.y - nodeB.gridIndex.y);
			
		if (x > y) {
			return y + (x - y);
		}
		return x + (y-x);
	}

	public Node NodeFromWorldPoint(Vector3 worldPosition) {
        float percentX = (worldPosition.x) / (gridSize.x*nodeDiameter);
        float percentY = (worldPosition.y) / (gridSize.y * nodeDiameter);
        float nodePercentX = 1f/ gridSize.x;
		float nodePercentY = 1f/ gridSize.y;
		int x = Mathf.FloorToInt(percentX/nodePercentX);
		int y = Mathf.FloorToInt(percentY/nodePercentY);

		if (x < gridSize.x && y < gridSize.y && x>=0 && y>=0) {
			if (grid!=null){
				if (grid [x, y]==null){
					Debug.Log (worldPosition.ToString() + "  " + x + "  " + y);
				}
				return grid [x, y];
			}else{
				Debug.Log ("grid is null");
				return null;
			}
		}
		else {
			return null;

		}
	}
	public Node GetNode(int x, int y){
		if (x < gridSize.x && y < gridSize.y && y>=0  && x>=0) {
				return grid [x, y];
		}else{
			Debug.Log ("Coordinates out of range");
			return null;
		}
	}
	public Node[] GetAllNodes(){
		Node[] nodes = new Node[gridSize .x* gridSize.y];
		for (int x = 0; x < gridSize.x; x ++) {
			for (int y = 0; y < gridSize.y; y ++) {
				nodes[y+x* gridSize.y] = grid[x,y];
			}
		}
		return nodes;
	}
    void OnDrawGizmos()
    {
        
        if (grid != null && displayGridGizmos)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, 1, gridSize.y));
            foreach (NodeInWorld n in grid)
            {
                switch (n.type) {
                    case TerrainType.DEFAULT:
                        Gizmos.color = Color.white;
                        break;
                    case TerrainType.FOREST:
                        Gizmos.color = Color.green;
                        break;
                    case TerrainType.MOUNTAIN:
                        Gizmos.color = Color.grey;
                        break;
                    case TerrainType.WATER:
                        Gizmos.color = Color.blue;
                        break;

                }
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}