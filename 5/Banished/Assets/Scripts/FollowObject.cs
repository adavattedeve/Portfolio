using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {
    public Transform followableObject;
    private Vector3 offSet;
	void Awake () {
        offSet = transform.position - followableObject.position;
    }

	void Update () {
        if (followableObject == null)
        {
            Destroy(gameObject);
            return;
        }
        transform.position = followableObject.position + offSet;
	}
}
