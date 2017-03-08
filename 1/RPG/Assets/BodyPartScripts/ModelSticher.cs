using UnityEngine;
using System.Collections;

public class ModelSticher : MonoBehaviour {
	private BoneControl boneControl;
	private SkinnedMeshRenderer baseRenderer;
	void Awake(){
		boneControl = transform.parent.gameObject.GetComponentInChildren<BoneControl> ();
	} 
	public GameObject AddLimb(GameObject limb){
		boneControl.SetBones ();
		boneControl.RestoreToOriginal ();
		baseRenderer = GetComponentInChildren<SkinnedMeshRenderer> ();
		SkinnedMeshRenderer newRenderer = limb.GetComponent<SkinnedMeshRenderer> ();
		GameObject stichedModel = ProcesNewLimb (newRenderer, this.gameObject, boneControl.gameObject, baseRenderer);
		boneControl.RestoreBones();
		return stichedModel;
	}

	private GameObject ProcesNewLimb ( SkinnedMeshRenderer limbRenderer, GameObject root, GameObject bonesRoot,
	                            SkinnedMeshRenderer baseRenderer){
		GameObject newLimb = new GameObject ();
		newLimb.name = limbRenderer.gameObject.name;
		newLimb.transform.parent = root.transform;
		SkinnedMeshRenderer newRenderer = newLimb.AddComponent <SkinnedMeshRenderer>();
		Transform[] bones = new Transform[baseRenderer.bones.Length];
		for (int i = 0; i<bones.Length; ++i) {
			bones[i] = GetChildrenByName(baseRenderer.bones[i].gameObject.name, bonesRoot);
		}

		newRenderer.bones = bones;
		newRenderer.sharedMesh = limbRenderer.sharedMesh;
		newRenderer.materials = limbRenderer.materials;
		return newRenderer.gameObject;
	}
	private Transform GetChildrenByName(string name, GameObject root){
		Transform[] allChildren = root.GetComponentsInChildren<Transform> ();
		for (int i=0; i< allChildren.Length; ++i) {
			if (name ==allChildren[i].name){
				return allChildren[i];
			}
		}
		return null;
	}
}
