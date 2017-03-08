using UnityEngine;
using System.Collections;

public class WeaponModelInfo : MonoBehaviour {
	public float[] rayRanges;
	private Transform[] rays;
	public Transform[] Rays{ get { return rays; } }
	void Awake(){
		rays = new Transform[transform.childCount];
		int i = 0;
		foreach (Transform child in transform) {
			rays [i] = child;
			++i;
		}
	}

}
