using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class InstantiatePrefab : MonoBehaviour {
	public string path;
	public string pathStart ="Environment/";
	[SerializeField] GameObject prefab;
	[SerializeField] GameObject newObject;

	void Awake(){
		if (path != null) {
			prefab = Resources.Load (pathStart+path, typeof(GameObject)) as GameObject;
		}
		RefreshPrefab ();
	}
	#if UNITY_EDITOR
	void Update () {
		if (!Application.isPlaying) {
			InstantiateFromResource();
		}
	}
	 #endif
	void InstantiateFromResource(){
		if (path != null) {
			prefab = Resources.Load (pathStart+path, typeof(GameObject)) as GameObject;
		}
		if (prefab != null) {
			if (newObject == null) {
				RefreshPrefab ();
			} else  if (newObject.name != string.Format ("{0}(Clone)", prefab.name)) {
				print ("Changing object");
				DestroyImmediate (newObject);
				newObject = (GameObject)Instantiate (prefab, transform.position, transform.rotation);
				newObject.transform.parent = transform;
				newObject.transform.localRotation = prefab.transform.rotation;
			}
		} else if (newObject != null) {
			DestroyImmediate (newObject);
		}
	}

	void RefreshPrefab(){
		if (prefab != null) {
			Transform[] children = GetComponentsInChildren<Transform> ();
			for (int i=0; i<children.Length; ++i) {
				if (children [i].name == string.Format ("{0}(Clone)", prefab.name)) {
					newObject = children [i].gameObject;
					DestroyImmediate (newObject);
					break;
				}

			}
			newObject = (GameObject)Instantiate (prefab, transform.position, transform.rotation);
			newObject.transform.parent = transform;
			newObject.transform.localRotation = prefab.transform.rotation;
		}
	}
}
