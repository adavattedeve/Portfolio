using UnityEngine;
using System.Collections;

public class CharacterVisualization : MonoBehaviour, IVisualizationController {

    public float movementSpeed = 2f;
    private Vector3 targetPosition = new Vector3();

    private Vector3 direction = new Vector3();
    private System.Action movementCallback = null;

    //References
    private Animator animator;

	void Awake () {
        animator = GetComponent<Animator>();
	}
	
    void Start() {
        targetPosition = transform.position;
    }

	void Update () {
        direction = targetPosition - transform.position;
        direction.x = targetPosition.x - transform.position.x;
        direction.y = targetPosition.y - transform.position.y;

        if (targetPosition != transform.position) {
            direction.Normalize();
            animator.SetBool("Running", true);
            //float step = GameManager.SnapToPixel(Time.deltaTime * movementSpeed);
            float step = Time.deltaTime * movementSpeed;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        }
        else if (movementCallback != null) {
            animator.SetBool("Running", false);
            movementCallback();
            movementCallback = null;
        }
	}
    public void Move(int _x, int _y, System.Action callback) {
        targetPosition = new Vector3(_x, _y, transform.position.z);
        movementCallback = callback;
    }
}
