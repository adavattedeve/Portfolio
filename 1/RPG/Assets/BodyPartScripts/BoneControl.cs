using UnityEngine;
using System.Collections;

public class BoneControl : MonoBehaviour {

	private Bone original;
	private Bone bones;
	void Awake () {
		original = new Bone (this.transform);
	}
	public void RestoreToOriginal(){
		original.Restore ();
	}
	public void SetBones(){
		bones = new Bone (this.transform);
	}
	public void RestoreBones(){
		bones.Restore ();
	}
}
