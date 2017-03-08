using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaticBatchingManager : MonoBehaviour {
	public static StaticBatchingManager instance;
	public List<List<GameObject>> batches;
	void Awake(){
		instance = this;
	}
	void Start(){

	}
}
