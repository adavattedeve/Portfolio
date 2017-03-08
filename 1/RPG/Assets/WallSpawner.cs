using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
public class WallSpawner : MonoBehaviour {
	[Range(0, 100)]public int wallAmount;
	public float width;
	public int[] doorPlaces;
	public int[] emptyPlace;
	public string wallPath;
	public string doorPath;
	private string pathStart ="Environment/";

	GameObject[] walls;
	[SerializeField] GameObject wallPrefab;
	[SerializeField] GameObject newWallObject;
	[SerializeField] GameObject doorPrefab;
	[SerializeField] GameObject newDoorObject;
	
	void Awake(){
		if (wallPath != null) {
			wallPrefab = Resources.Load (pathStart+wallPath, typeof(GameObject)) as GameObject;
		}
		if (doorPath != null) {
			doorPrefab = Resources.Load (pathStart+doorPath, typeof(GameObject)) as GameObject;
		}
		walls=new GameObject[wallAmount];
		DestroyOldGOs ();
		for (int i=0; i<walls.Length; ++i){
			RefreshPrefab (i);
		}
	}
	#if UNITY_EDITOR
	void Update () {
		if (!Application.isPlaying) {
			if (wallAmount!= walls.Length){
				GameObject[] temp = new GameObject[wallAmount];
				for (int i=0; i<wallAmount; ++i){
					if (walls.Length>i){
					temp[i] = walls[i];
					}else{
						temp[i]=null;
					}
				}
				walls = temp;
				DestroyOldGOs ();
			}
			for (int i=0; i<walls.Length; ++i){
				InstantiateFromResource(i);
			}
		}
	}
	#endif
	void InstantiateFromResource(int index){

		if (ArrayContains (doorPlaces, index)) {
			if (doorPath != null) {
				doorPrefab = Resources.Load (pathStart + doorPath, typeof(GameObject)) as GameObject;
			}
			if (doorPrefab != null) {
				if (walls [index] == null) {
					RefreshPrefab (index);
				} else  if (walls [index].name != string.Format ("{0}(Clone)", doorPrefab.name)) {
					DestroyImmediate (walls [index]);
					walls [index] = (GameObject)Instantiate (doorPrefab, transform.position, transform.rotation);
					walls [index].transform.parent = transform;
					walls [index].transform.localRotation = doorPrefab.transform.rotation;
					walls [index].transform.localPosition = new Vector3 (index * width + (width / 2), 0, 0);
				}
			} else if (walls [index] != null) {
				DestroyImmediate (walls [index]);
			}
		} else if (ArrayContains (emptyPlace, index)) {
			if (walls[index]){
				DestroyImmediate(walls[index]);
			}
			return;
		}

		else {
			if (wallPath != null) {
				wallPrefab = Resources.Load (pathStart+wallPath, typeof(GameObject)) as GameObject;
			}
			if (wallPrefab != null) {
				if (walls[index] == null) {
					RefreshPrefab (index);
				} 
				else  if (walls[index].name != string.Format ("{0}(Clone)", wallPrefab.name)) {
					DestroyImmediate (walls[index]);
					walls[index] = (GameObject)Instantiate (wallPrefab, transform.position, transform.rotation);
					walls[index].transform.parent = transform;
					walls[index].transform.localRotation = wallPrefab.transform.rotation;
					walls[index].transform.localPosition = new Vector3(index*width+(width/2),0,0);
				}
			} else if (walls[index] != null) {
				DestroyImmediate (walls[index]);
			}
		}
	}
	
	void RefreshPrefab(int index){
		if (ArrayContains (doorPlaces, index)) {
			walls[index] = (GameObject)Instantiate (doorPrefab, transform.position, transform.rotation);
			walls[index].transform.parent = transform;
			walls[index].transform.localRotation = doorPrefab.transform.rotation;
			walls[index].transform.localPosition = new Vector3(index*width+(width/2),0,0);
		}else if (ArrayContains (emptyPlace, index)) {
			if (walls[index]){
				DestroyImmediate(walls[index]);
			}
			return;
		}
		else {
			walls[index] = (GameObject)Instantiate (wallPrefab, transform.position, transform.rotation);
			walls[index].transform.parent = transform;
			walls[index].transform.localRotation = wallPrefab.transform.rotation;
			walls[index].transform.localPosition = new Vector3(index*width+(width/2),0,0);
		}
	}

	void DestroyOldGOs(){
		Transform[] children = GetComponentsInChildren<Transform> ();
		for (int i=0; i<children.Length; ++i) {
			if (children [i] && children [i].name == string.Format ("{0}(Clone)", wallPrefab.name)) {
				DestroyImmediate (children [i].gameObject);
			}else if (children [i] && children [i].name == string.Format ("{0}(Clone)", doorPrefab.name)){
				DestroyImmediate (children [i].gameObject);
			}
		}
	}

	bool ArrayContains(int[] array, int value){
		if (array == null) {
			return false;}
		for (int i=0; i<array.Length; ++i) {
			if (array [i] == value) {
				return true;
			}
		}
		return false;
	}
}
