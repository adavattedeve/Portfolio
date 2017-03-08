using UnityEngine;
using System.Collections;

public class BloodControl : MonoBehaviour {
	public ParticleSystem hitParticles;
	public GameObject bloodDecal;
	Health health;
	IBloodines[] bloodines;
	CharacterEvents events;

	void Awake(){
		events = GetComponent<CharacterEvents> ();
		hitParticles.gameObject.SetActive (false);
		bloodines = (IBloodines[])GetComponentsInChildren<IBloodines>(true);
		health = GetComponent<Health> ();
	}
	void Start(){
		events.TakeHit += BloodDecal;
		events.TakeHit += BloodParticles;
	}

	void BloodDecal(){
			EffectManager.instance.BloodDecal (transform.position);
	}
	void BloodParticles(){
		hitParticles.gameObject.SetActive (false);
		hitParticles.gameObject.SetActive (true);
	}
}
