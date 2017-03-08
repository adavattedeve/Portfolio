using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

    public float updateRate = 0.25f;
    public float targetRadius = 0.5f;
    public int otherEnemyCheckRayResolution = 3;
    public float rayCastingWidth = 0.5f;
    public float rangeToTarget = 1.5f;
    public bool isMoving;

    private float updateTimer = 0;

    private NavMeshAgent agent;
    private Transform goal;
    private Rigidbody rb;

    public Transform Goal
    { set
        {
            goal = value;
            Vector3 newTarget = goal.position;
            newTarget.y = 0;
            agent.destination = newTarget;
        }
    }
 
    void Awake ()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        if (isMoving) {
            updateTimer += Time.deltaTime;
            if (updateRate <= updateTimer)
            {
                updateTimer = 0;
                Vector3 newTarget = goal.position;
                newTarget.y = 0;
                agent.destination = newTarget;
            }
        }
        float distanceToTarge = DistanceToTarget();
        if (distanceToTarge <= rangeToTarget && isMoving) {
            StopMoving();
        } else if (distanceToTarge > rangeToTarget && !isMoving) {
            StartMoving();
        }
        lol();
        if (distanceToTarge <= agent.radius + targetRadius) {
            rb.MovePosition(transform.position + (transform.position - goal.transform.position) * (agent.radius + targetRadius - distanceToTarge));
        }
    }

    public void StopMoving() {
        isMoving = false;
        agent.velocity = Vector3.zero;
        agent.ResetPath();
    }

    public void StartMoving() {
        isMoving = true;
        Vector3 newTarget = goal.position;
        newTarget.y = 0;
        agent.destination = newTarget;
    }

    private float DistanceToTarget() {
        return Vector3.Distance(transform.position, goal.position);
    }

    void lol () {
        Vector3[] path = agent.path.corners;
        if (path.Length <= 1) {
            return;
        }

        Ray ray = new Ray();
        ray.direction = path[1] - transform.position;
        Vector3 pos = transform.position;
        pos.y = agent.height / 2;
        RaycastHit hit;
        bool otherEnemyInWay = false;
        for (int i = 0; i < otherEnemyCheckRayResolution; ++i)
        {
            ray.origin = rayCastingWidth * transform.right * (i / ((float)(otherEnemyCheckRayResolution - 1) / 2) - 1) + pos;
            if (Physics.Raycast(ray, out hit, agent.radius * 3, LayerMask.GetMask("Enemy")))
            {
                EnemyMovement other = hit.collider.GetComponent<EnemyMovement>();
                if (!other.isMoving) {
                    if (isMoving) {                       
                        StopMoving();
                        return;
                    }
                    otherEnemyInWay = true;
                }
            }
        }
        if (!isMoving && !otherEnemyInWay) {
            StartMoving();
        }

    }
}
