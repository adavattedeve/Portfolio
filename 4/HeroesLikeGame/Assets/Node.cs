using UnityEngine;
using System.Collections;
public abstract class Node : IHeapItem<Node> {
	public float movementCost=1;
    public abstract bool walkable {
        get;
    }
	public bool occupied;
	public Vector3 worldPosition;
	public IntVector2 gridIndex;

	public int gCost;
	public int hCost;
	public Node parent;
	int heapIndex;


	public Node( Vector3 _worldPos, IntVector2 _gridIndex) {
		worldPosition = _worldPos;
        gridIndex = _gridIndex;
        occupied = false;
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
