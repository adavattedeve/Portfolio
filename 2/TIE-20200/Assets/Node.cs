using UnityEngine;
using System.Collections;
public class Node : IHeapItem<Node> {
	public TileID id;
	public float movementCost=1;
	public bool walkable;
	public bool occupied;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;
	public Node parent;
	int heapIndex;

	public VisualizationTile visualizationTile;
	private GameObject tileGO;
	private MeshRenderer rend;


	private Unit unit;
	public Unit Unit{
		set{ unit = value;
			if (OnUnitOnNodeChange !=null){
				string name ="null";
				if (unit!=null){
					name = unit.name;
				}
				Debug.Log (name + "  to " + gridX + "  " + gridY);
				OnUnitOnNodeChange(unit);
			}
		}
		get {return unit;}
	}

	public delegate void OnUnitOnNodeChangeAction (Unit newUnit);
	public event OnUnitOnNodeChangeAction OnUnitOnNodeChange;

	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, GameObject _visualizationTilePrefab, TileData data) {
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
		occupied = false;
		visualizationTile = (MonoBehaviour.Instantiate (_visualizationTilePrefab, worldPosition, Quaternion.identity) as GameObject).GetComponent<VisualizationTile>();
		visualizationTile.Node = this;

		InitializeFromData (data);
		if (!walkable) {
			//visualizationTile.ScaleToZero();
			tileGO.GetComponent<MeshCollider>().enabled=false;
		}
	}
	public void InitializeFromData(TileData data){
		if (tileGO != null) {
			MonoBehaviour.Destroy(tileGO);
		}
		tileGO = (MonoBehaviour.Instantiate (data.prefab) as GameObject);
		tileGO.transform.position = worldPosition;
		tileGO.transform.Rotate (Vector3.up, Random.Range(0,2)*180, Space.World);
		rend = tileGO.GetComponent<MeshRenderer> ();
		rend.material.SetTexture ("_BumpMap", data.normalMaps[Random.Range(0,data.normalMaps.Length)]);
		movementCost = data.movementCost;
	}
	public int fCost {
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}
