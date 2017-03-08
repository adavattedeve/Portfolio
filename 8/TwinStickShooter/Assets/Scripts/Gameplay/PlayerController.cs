using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    private float speed = 4f;

    private Vector3 movementDirection = Vector3.zero;
    public Vector3 MovementDirection { set { movementDirection = value; } }

    private Vector3 aimDirection = Vector3.zero;
    public Vector3 AimDirection { set { aimDirection = value; } }
    
    private Animator animator;
    private CharacterController characterController;

    private Vector3 lastValidPos;

	void Awake ()
    {
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
        lastValidPos = transform.position;
    }
    void Start ()
    {
        PlayerStats stats = GetComponent<PlayerStats>();
        stats.GetStat(StatType.MOVEMENT_SPEED).OnStatChangeEvent += OnMovementSpeedChange;
        stats.GetStat(StatType.ATTACK_SPEED).OnStatChangeEvent += OnAttackSpeedChange;

        OnMovementSpeedChange(stats.GetStat(StatType.MOVEMENT_SPEED));
        OnAttackSpeedChange(stats.GetStat(StatType.ATTACK_SPEED));
    }
	
	void Update () {
        Move();
        UpdateAnimationParams();
        transform.rotation = Quaternion.Euler(new Vector3(0, AngleUtility.AngleBetween(Vector3.forward, aimDirection), 0));
        
    }

    private void Move() {
        characterController.Move(movementDirection * speed * Time.deltaTime);
        if (transform.position.y == 0)
            lastValidPos = transform.position;
        else
            transform.position = lastValidPos;
    }

    private void UpdateAnimationParams () {
        if (movementDirection == Vector3.zero)
        {
            animator.SetFloat("MovementDirectionX", 0);
            animator.SetFloat("MovementDirectionY", 0);
        }
        else {
            Vector3 animDirection = Quaternion.Euler(new Vector3(0, -AngleUtility.AngleBetween(movementDirection, aimDirection), 0)) * Vector3.forward;
            animDirection.Normalize();
            animator.SetFloat("MovementDirectionX", animDirection.x);
            animator.SetFloat("MovementDirectionY", animDirection.z);
        }
        
    }

    private void OnMovementSpeedChange(Stat stat)
    {
        speed = stat.Quantity;
        animator.SetFloat("MovementSpeed", stat.Quantity / stat.BaseQuantity);
    }

    private void OnAttackSpeedChange(Stat stat)
    {
        animator.SetFloat("AttackSpeed", stat.Quantity);
    }
}
