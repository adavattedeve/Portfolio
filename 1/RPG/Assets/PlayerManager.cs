using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	public static PlayerManager instance;
	private GameObject player;
	public GameObject Player {
		get{
			if (!player) {
				player = GameObject.FindGameObjectWithTag ("Player");
			} 
			if (player) {
				return player;
			}
			return null;
		}
	}
	void Awake(){
		if (!instance) {
			instance = this;
			player = GameObject.FindGameObjectWithTag ("Player");
		}
	}
	void OnLevelWasLoaded(){
		Player.GetComponent<CharacterEvents> ().TakeHit += EffectManager.instance.CameraShake;
	}
}
