using UnityEngine;
using System.Collections;

public class BloodDecal : MonoBehaviour {
	public float minScale;
	public float maxScale;
	void OnEnable(){
		float scale = Random.Range (minScale, maxScale);
		transform.localScale = new Vector3 (scale, scale, scale);
		float rotation = Random.Range (-180f, 180f);
		transform.rotation = Quaternion.identity;
		transform.Rotate (Vector3.right*90, Space.World);
		transform.Rotate (Vector3.up*rotation, Space.World);
		Vector3 pos = transform.position;
		pos.y = 0.01f;
		transform.position = pos;

	}
}
