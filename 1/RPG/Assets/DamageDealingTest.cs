using UnityEngine;
using System.Collections;

public class DamageDealingTest : MonoBehaviour {
	HitInfo hit;
	void OnTriggerEnter(Collider coll){
		IDamageable health = coll.GetComponent<IDamageable> ();
		if (health!=null){

		hit.damage = 10;
		hit.impact = 35;
		health.TakeDamage(hit);}
	}
}
