using UnityEngine;
using System.Collections;

public class RestartScene : MonoBehaviour {
	
	void Update () {
		if (Input.GetKeyDown ("r")) {
			Application.LoadLevel(0);
		}
	}
}
