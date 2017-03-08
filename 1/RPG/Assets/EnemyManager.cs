using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {
	Grid grid;
	public Grid Grid{get{return grid;}}
	void Awake () {
		grid = GameObject.Find ("PathFinding").GetComponent<Grid>();
	}
}
