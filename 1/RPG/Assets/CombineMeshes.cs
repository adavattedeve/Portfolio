using UnityEngine;
using System.Collections;

public class CombineMeshes : MonoBehaviour {
	private CombineInstance[] combine;
	private MeshFilter[] filters;
	void Awake () {
		filters = GetComponentsInChildren<MeshFilter> ();
		combine = new CombineInstance[filters.Length];
	}
	void Start () {
		for (int i=0; i<filters.Length; ++i){
			combine[i].mesh = filters[i].sharedMesh;
			combine[i].transform = filters[i].transform.localToWorldMatrix;
		
			filters[i].gameObject.SetActive(false);
		}
		gameObject.AddComponent<MeshRenderer> ();
		Mesh mesh = gameObject.AddComponent<MeshFilter>().mesh = new Mesh();
		mesh.CombineMeshes(combine, true, true);
	}
}
