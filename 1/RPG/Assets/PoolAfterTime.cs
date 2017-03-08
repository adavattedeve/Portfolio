using UnityEngine;
using System.Collections;

public class PoolAfterTime : MonoBehaviour {
	public float time = 1f;
	void OnEnable(){
		Invoke ("PoolGameobject", time);
	}
	void OnDisable(){
		CancelInvoke ("PoolGameobject");
	}
	void PoolGameobject(){
		ObjectPool.instance.PoolObject (gameObject);
	}
}
