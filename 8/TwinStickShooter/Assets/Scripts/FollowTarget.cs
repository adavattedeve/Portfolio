using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour {
    public Transform target;
    private Vector3 targetPos;
    public float maxPredictionDistance = 5f;
    public float smoothing = 0.9f;

    private float predictionDistance = 0;
    private Vector3 offset = Vector3.zero;
	// Use this for initialization
	void Start () {
        if (target != null)
            RefreshOffset();
	}
	
	// Update is called once per frame
	void LateUpdate () {
        predictionDistance = Mathf.Lerp(0f, maxPredictionDistance, predictionDistance/maxPredictionDistance);
        targetPos = target.position + offset + target.forward * predictionDistance;
        //Vector3 newPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);
	}

    private void RefreshOffset() {
        offset = transform.position - target.position;
    }
}
