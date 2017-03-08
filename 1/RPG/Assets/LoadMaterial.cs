using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class LoadMaterial : MonoBehaviour {
	public string path;
	private string pathStart = "Materials/";
	[SerializeField] Material material;
	[SerializeField] Material materialInUse;
	MeshRenderer meshRenderer;
	
	void Awake(){
		if (path != null) {
			material = Resources.Load (pathStart+path, typeof(Material)) as Material;
			if (material==null){
				material = Resources.Load (pathStart+path, typeof(ProceduralMaterial)) as Material;
			}
		}
		if (material != null) {
			meshRenderer = GetComponent<MeshRenderer>();
			if (meshRenderer != null){
				materialInUse = meshRenderer.sharedMaterial;
				if (material != materialInUse){
					meshRenderer.sharedMaterial = material;
					materialInUse = material;
				}
			}
		}
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
			material = Resources.Load (pathStart+path, typeof(Material)) as Material;
			if (material==null){
				material = Resources.Load (pathStart+path, typeof(ProceduralMaterial)) as Material;
			}
		}

		if (meshRenderer == null) {
			meshRenderer = GetComponent<MeshRenderer> ();
		}
		if (material != null) {
			if (meshRenderer != null) {
				materialInUse = meshRenderer.sharedMaterial;
				if (material != materialInUse) {
					Debug.Log ("changingMaterial");
					meshRenderer.sharedMaterial = material;
					materialInUse = material;
				}
			}
		}
		else if (materialInUse != null) {
			meshRenderer.sharedMaterial = null;
			materialInUse=null;
		}
	}
}
