using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
    public float speed = 10f;
    public float ppuMultiplier = 2;
    public float minimumCameraSize = 1.9f;
    private Vector3 targetPosition;

    private bool follow = false;
    private GameObject objectToFollow;

    private Camera cam;
	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
        if (Screen.height%2 == 0)
	        cam.orthographicSize = (Screen.height/(ppuMultiplier*GameManager.PPU))*0.5f;
        else
            cam.orthographicSize = ((Screen.height+1) / (ppuMultiplier * GameManager.PPU)) * 0.5f;

        while (cam.orthographicSize < minimumCameraSize) {
            cam.orthographicSize *= 2;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (follow && objectToFollow != null) {
            Vector3 newPos = Vector3.Lerp(transform.position, new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y, transform.position.z), 0.6f);
            transform.position = newPos;
        }
	}

    public void MoveCameraTo(float _x, float _y) {
        targetPosition = new Vector3(_x, _y, transform.position.z);
    }

    public void Follow(GameObject _objectToFollow) {
        objectToFollow = _objectToFollow;
        follow = true;
    }
    public void StopFollow() {
        follow = false;
    }
}
