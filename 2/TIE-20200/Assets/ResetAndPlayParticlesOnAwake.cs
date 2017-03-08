using UnityEngine;
using System.Collections;

public class ResetAndPlayParticlesOnAwake : MonoBehaviour {
	private ParticleSystem particles;
	void Awake(){
		particles = GetComponent<ParticleSystem> ();
	}
	void OnEnable(){
		particles.Clear ();
		particles.Play ();
	}
}
