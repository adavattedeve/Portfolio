using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public bool displayGridGizmos;
	public LayerMask unwalkableMask;
	public Vector2 gridEnvironmentExtensionSize;
	public Vector2 gridBattlefieldSize;
	public Vector2 unwalkableAmount, unwalkableBoundsByGrid;
	public float nodeRadius;
	[Range(0.1f, 1f)]public float UnWalkableSphereRadius;
	Node[,] grid;
	public Node[,] GetGrid{get{return grid;}}
	private GameObject grassTilePrefab;
	private GameObject visualizationTile;
	float nodeDiameter;
	[System.NonSerialized]public int gridSizeX, gridSizeY;
	private Node[] neighbours;
	private PathFinding pathFinding;
	private int gridWithEnvironmentSizeX, gridWithEnvironmentSizeY;
	[Header("formations depending on unit stacks amount")]
	public int[] form1;
	public int[] form2;
	public int[] form3;
	public int[] form4;
	public int[] form5;
	public int[] form6;

	private GameObject[] unwalkableObjectPrefabs;
	void Awake() {
		pathFinding = GetComponent<PathFinding> ();
		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridBattlefieldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridBattlefieldSize.y/nodeDiameter);
		gridWithEnvironmentSizeX =  Mathf.RoundToInt((gridBattlefieldSize.x+gridEnvironmentExtensionSize.x)/nodeDiameter);
		gridWithEnvironmentSizeY =  Mathf.RoundToInt((gridBattlefieldSize.y+gridEnvironmentExtensionSize.y)/nodeDiameter);
		neighbours = new Node[9];


	}
	void Start(){
		grassTilePrefab = DataBase.instance.gameData.defaultTilePrefab;
		visualizationTile = DataBase.instance.gameData.visualizationTilePrefab;
		unwalkableObjectPrefabs = (GameObject[])Resources.LoadAll<GameObject> (DataBase.instance.gameData.unwalkableObjectsPrefabPath);
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
		int battlefieldNodeMinX = (gridWithEnvironmentSizeX-gridSizeX) / 2;
		int battlefieldNodeMaxX = gridWithEnvironmentSizeX - (gridWithEnvironmentSizeX-gridSizeX) / 2;
		int battlefieldNodeMinY = (gridWithEnvironmentSizeY-gridSizeY) / 2;
		int battlefieldNodeMaxY = gridWithEnvironmentSizeY - (gridWithEnvironmentSizeY-gridSizeY) / 2;

		Vector3 worldBottomLeft = transform.position - Vector3.right * (gridBattlefieldSize.x+gridEnvironmentExtensionSize.x)/2 - Vector3.forward * (gridBattlefieldSize.y+gridEnvironmentExtensionSize.y)/2;
		Debug.Log (battlefieldNodeMinX + "  " + battlefieldNodeMaxX + "  " + battlefieldNodeMinY +"  " +battlefieldNodeMaxY);
		//SpawnUnwalkables!!
		int unwalkableCount = Random.Range ((int)unwalkableAmount.x, (int)unwalkableAmount.y+1);
		List<Vector2> unwalkableNodes = new List<Vector2> ();
		while (unwalkableCount>0){
			UnwalkableObject unwalkableObject = (Instantiate(unwalkableObjectPrefabs[Random.Range(0,unwalkableObjectPrefabs.Length)]) as GameObject).GetComponent<UnwalkableObject>();
			Vector2 tempBounds = unwalkableBoundsByGrid;
			int randomRotationCount = Random.Range(0,4);
			if (randomRotationCount==0 || randomRotationCount==2 ){
				tempBounds.x+=unwalkableObject.sizeX;
				tempBounds.y+=unwalkableObject.sizeY;
			}else {
				tempBounds.x+=unwalkableObject.sizeY;
				tempBounds.y+=unwalkableObject.sizeX;
			}
			Debug.Log (tempBounds.ToString());
			Vector2 randomPos = new Vector2(Random.Range((int)tempBounds.x, (int)(gridSizeX-tempBounds.x)-1),Random.Range((int)tempBounds.y, (int)(gridSizeY-tempBounds.y))-1);
			unwalkableObject.transform.position =  worldBottomLeft + 
				Vector3.right * (randomPos.x * nodeDiameter + battlefieldNodeMinX*nodeDiameter + nodeRadius) + 
					Vector3.forward * (randomPos.y * nodeDiameter+ battlefieldNodeMinY*nodeDiameter + nodeRadius);
			unwalkableObject.transform.Rotate(Vector3.up, randomRotationCount*90);
			--unwalkableCount;
		}
		//SpawnTiles!!
		TileData defaultTile = DataBase.instance.GetTileData (TileID.GRASS);

		for (int x = 0; x < gridWithEnvironmentSizeX; x ++) {
			for (int y = 0; y < gridWithEnvironmentSizeY; y ++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				if (x>=battlefieldNodeMinX && x<battlefieldNodeMaxX && y>=battlefieldNodeMinY && y<battlefieldNodeMaxY){
					bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius*UnWalkableSphereRadius,unwalkableMask));
					grid[x-battlefieldNodeMinX,y-battlefieldNodeMinY] = new Node(walkable ,worldPoint, x-battlefieldNodeMinX,y-battlefieldNodeMinY, visualizationTile, DataBase.instance.GetTileData(TileID.GRASS));
				}else {
					GameObject temp = (MonoBehaviour.Instantiate (defaultTile.prefab) as GameObject);
					temp.transform.position = worldPoint;
					temp.transform.Rotate (Vector3.up, Random.Range(0,2)*180, Space.World);
					temp.GetComponent<MeshRenderer> ().material.SetTexture ("_BumpMap", defaultTile.normalMaps[Random.Range(0,defaultTile.normalMaps.Length)]);;
				}

			}
		}
//		for (int x = 0; x < gridSizeX; x ++) {
//			for (int y = 0; y < gridSizeY; y ++) {
//				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
//				bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius*UnWalkableSphereRadius,unwalkableMask));
//				grid[x,y] = new Node(walkable ,worldPoint, x,y, visualizationTile, DataBase.instance.GetTileData(TileID.GRASS));
//			}
//		}
	}
	//private void 
	public List<Node> GetValidMovement(Node node){
		List<Node> movementTargets = new List<Node> ();
		int minX = node.gridX - node.Unit.stats.GetStat (StatType.MOVEMENT).Value;
		int maxX = node.gridX + node.Unit.stats.GetStat (StatType.MOVEMENT).Value;
		int minY = node.gridY - node.Unit.stats.GetStat (StatType.MOVEMENT).Value;
		int maxY = node.gridY + node.Unit.stats.GetStat (StatType.MOVEMENT).Value;
		minX =Mathf.Clamp (minX, 0,gridSizeX-1);
		maxX = Mathf.Clamp (maxX, 0,gridSizeX-1);
		minY=Mathf.Clamp (minY, 0,gridSizeY-1);
		maxY=Mathf.Clamp (maxY, 0,gridSizeY-1);
		for (int x = minX; x <= maxX; x ++) {
			for (int y = minY; y<=maxY; y ++) {
				if (grid[x,y].Unit==null || (!grid[x,y].Unit.Visible && !CombatManager.instance.IsThisUnitsTurn(grid[x,y].Unit))){
					if (grid[x,y].walkable && pathFinding.IsValidMovement(node, grid[x,y])){
						movementTargets.Add(grid[x,y]);
					}
				}
			}
		}
		return movementTargets;
	}
	public Node UnitsNode(Unit unit){

		if (unit != null) {
			Node node = NodeFromWorldPoint (unit.unitController.transform.position);
			if (unit.state==UnitState.MOVING || node.Unit==null){
				if (unit.unitController.currentMovementTarget==null){
					return node;
				}
				return unit.unitController.currentMovementTarget;
			}
			return node;
		}
		return null;
	}
	public void SetUnitsToGrid(Troop troop, PlayerID playerID){
		int x;
		Quaternion defaultRotation;
		if (playerID == PlayerID.LEFT) {
			x = 0;
			defaultRotation = Quaternion.identity * Quaternion.Euler (0, 90, 0);
		} else {
			x = gridSizeX-1;
			defaultRotation = Quaternion.identity * Quaternion.Euler (0, -90, 0);
		}
		if (troop.hero != null) {
			troop.hero.InstantiateTo((grid[x, gridSizeY-1].worldPosition) + new Vector3(0,0,nodeDiameter), defaultRotation);
		}
		int unitCount = 0;
		for (int i=0; i<troop.units.Count; ++i) {
			if (troop.units[i]==null){
				continue;
			}
			++unitCount;
		}
		//formations!!
		switch(unitCount){
		case 1:
			for (int i=0; i<troop.units.Count; ++i) {
				if (troop.units[i]==null){
					continue;
				}
				grid[x, form1[1-unitCount]].Unit = troop.units[i];
				troop.units[i].InstantiateTo(grid[x,  form1[1-unitCount]].worldPosition, defaultRotation);
				--unitCount;
			}
			break;
		case 2:
			for (int i=0; i<troop.units.Count; ++i) {
				if (troop.units[i]==null){
					continue;
				}
				grid[x, form2[2-unitCount]].Unit = troop.units[i];
				troop.units[i].InstantiateTo(grid[x,  form2[2-unitCount]].worldPosition, defaultRotation);
				--unitCount;
			}
			break;
		case 3:
			for (int i=0; i<troop.units.Count; ++i) {
				if (troop.units[i]==null){
					continue;
				}
				grid[x, form3[3-unitCount]].Unit = troop.units[i];
				troop.units[i].InstantiateTo(grid[x,  form3[3-unitCount]].worldPosition, defaultRotation);
				--unitCount;
			}
			break;
		case 4:
			for (int i=0; i<troop.units.Count; ++i) {
				if (troop.units[i]==null){
					continue;
				}
				grid[x, form4[4-unitCount]].Unit = troop.units[i];
				troop.units[i].InstantiateTo(grid[x,  form4[4-unitCount]].worldPosition, defaultRotation);
				--unitCount;
			}
			break;
		case 5:
			for (int i=0; i<troop.units.Count; ++i) {
				if (troop.units[i]==null){
					continue;
				}
				grid[x, form5[5-unitCount]].Unit = troop.units[i];
				troop.units[i].InstantiateTo(grid[x,  form5[5-unitCount]].worldPosition, defaultRotation);
				--unitCount;
			}
			break;
		case 6:
			for (int i=0; i<troop.units.Count; ++i) {
				if (troop.units[i]==null){
					continue;
				}
				grid[x, form6[6-unitCount]].Unit = troop.units[i];
				troop.units[i].InstantiateTo(grid[x,  form6[6-unitCount]].worldPosition, defaultRotation);
				--unitCount;
			}
			break;
		default:
			break;
		}
	}
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

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					if (x ==0 || y==0){
						neighbours[currentIndex]=grid[checkX,checkY];
						continue;}
					else {
						if (grid[checkX+ (-1)*x,checkY].walkable && grid[checkX,checkY+ (-1)*y].walkable){
							//if (!grid[checkX+ (-1)*x,checkY].occupied && !grid[checkX,checkY+ (-1)*y].occupied){
								neighbours[currentIndex]=grid[checkX,checkY];
								continue;
							//}
						}
					}
				}
				neighbours[currentIndex]=null;
			}
		}

		return neighbours;
	}

	public int DistanceBetween(Node nodeA, Node nodeB){
			int x = Mathf.Abs(nodeA.gridX - nodeB.gridX);
			int y = Mathf.Abs(nodeA.gridY - nodeB.gridY);
			
		if (x > y) {
			return y + (x - y);
		}
		return x + (y-x);
	}

	public Node NodeFromWorldPoint(Vector3 worldPosition) {
		float percentX = (worldPosition.x + gridBattlefieldSize.x/2) / gridBattlefieldSize.x;
		float percentY = (worldPosition.z + gridBattlefieldSize.y/2) / gridBattlefieldSize.y;
		float nodePercentX = 1f/gridSizeX;
		float nodePercentY = 1f/gridSizeY;
		int x = Mathf.FloorToInt(percentX/nodePercentX);
		int y = Mathf.FloorToInt(percentY/nodePercentY);
//		int x = Mathf.Clamp(Mathf.FloorToInt(percentX/nodePercentX), 0, gridSizeX-1);
//		int y = Mathf.Clamp (Mathf.FloorToInt(percentY/nodePercentY), 0, gridSizeY-1);

		if (x < gridSizeX && y < gridSizeY && x>=0 && y>=0) {
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
		if (x < gridSizeX && y < gridSizeY && y>=0  && x>=0) {
				return grid [x, y];
		}else{
			Debug.Log ("Coordinates out of range");
			return null;
		}
	}
	public Node[] GetAllNodes(){
		Node[] nodes = new Node[gridSizeX*gridSizeY];
		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				nodes[y+x*gridSizeY]= grid[x,y];
			}
		}
		return nodes;
	}
	void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position,new Vector3(gridBattlefieldSize.x,1,gridBattlefieldSize.y));
		if (grid != null && displayGridGizmos) {
			foreach (Node n in grid) {
				Gizmos.color = (n.walkable)?Color.white:Color.red;
				Gizmos.color = (n.occupied)?Color.cyan:Gizmos.color;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));
			}
		}
	}
}