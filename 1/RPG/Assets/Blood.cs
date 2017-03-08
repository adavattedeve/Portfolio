using UnityEngine;
using System.Collections;

public class Blood :MonoBehaviour, IBloodines {
	ProceduralMaterial material;
	void Awake(){
		material = GetComponent<SkinnedMeshRenderer> ().material as ProceduralMaterial;
		material.SetProceduralFloat ("Bloodines", 0);
		material.RebuildTextures ();
	}
	public void SetBlood(float amount){
		material.SetProceduralFloat ("Bloodines", amount);
		material.RebuildTextures ();
	}
}
