using UnityEngine;
using System.Collections;

public class RemoveBodyPart : MonoBehaviour {
	public GameObject parentBone;
	public string pathToBodyPartFolder;
	public string bodyPartPath;
	public float minForce;
	public float maxForce;
	SkinnedMeshRenderer sMRenderer;
	GameObject staticObject;
	Rigidbody rB;
	CharacterEvents events;

	void Awake(){
		sMRenderer = GetComponent<SkinnedMeshRenderer> ();
		staticObject = Resources.Load (pathToBodyPartFolder+bodyPartPath, typeof(GameObject)) as GameObject;
		if (staticObject == null || parentBone==null) {
			
			Destroy(this);
		}
		staticObject = Instantiate (staticObject);
		staticObject.transform.SetParent (transform);
		staticObject.transform.position = parentBone.transform.position;
		staticObject.transform.rotation = parentBone.transform.rotation;
		rB = staticObject.GetComponent<Rigidbody> ();
		staticObject.SetActive (false);
		events = sMRenderer.transform.parent.parent.GetComponent<CharacterEvents> ();
	}
	void Start () {

		events.Death+=SeverBodypart;
//		DeathEvent.deathEvent += BloodExplosion;
	}
	void OnDisable(){
		events.Death-=SeverBodypart;
//		DeathEvent.deathEvent -= BloodExplosion;
	}
	public void SeverBodypart(){
		sMRenderer.enabled = false;
		staticObject.SetActive (true);
		staticObject.transform.parent = null;
		rB.AddForce (new Vector3(Random.Range(-maxForce, maxForce),Random.Range(minForce, maxForce),Random.Range(-maxForce, maxForce)), ForceMode.Force);
		rB.AddTorque (new Vector3(Random.Range(-maxForce, maxForce),Random.Range(minForce, maxForce),Random.Range(-maxForce, maxForce)));
	}
//	public void BloodExplosion(){
//		Collider[] colliders = Physics.OverlapSphere (transform.position, 2f);
//		for (int i=0; i<colliders.Length; ++i) {
//			IBloodines bloodines = colliders[i].gameObject.GetComponent("IBloodines") as IBloodines;
//			if (bloodines!=null){
//				bloodines.AddBlood(0.05f);
//			}
//		}
//	}
}
