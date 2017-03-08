using UnityEngine;
using System.Collections;

public static class CustomExtensions{

	public static float CalculateDistance(this Vector3 from, Vector3 to){
		return (to - from).sqrMagnitude;
	}
}
