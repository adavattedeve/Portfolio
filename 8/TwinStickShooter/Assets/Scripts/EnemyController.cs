using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour, ICharacterControl{

    public float attackRange = 1f;

    [SerializeField]
    private LayerMask playerMask;
    private float width;
    private float sqrtStartAttackDistance;
    private bool controlsEnabled = false;
    private Vector3 knockbackDirection = Vector3.zero;

    private Stat attackDamage;
    private Stat damageMpl;
    
    private Animator anim;
    private EnemyMovement movement;
    private NavMeshAgent agent;
    private Transform target;
 
    //private List<StatusEffect> statusEffects

    public Transform Target { get { return target; } }
    public bool ControlsEnabled
    {
        get
        {
            return controlsEnabled;
        }
        set
        {
            controlsEnabled = value;

            if (controlsEnabled == false)
            {
                movement.StopMoving();
                movement.enabled = false;
            }
            else
            {
                movement.StartMoving();
                movement.enabled = true;
            }
        }
    }
    public Vector3 KnockbackDirection
    {
        get
        {
            return knockbackDirection;
        }
    }

    void Awake ()
    {
        anim = GetComponent<Animator>();
        movement = GetComponent<EnemyMovement>();
        agent = GetComponent<NavMeshAgent>();

    }

    void Start ()
    {
        EnemyStats stats = GetComponent<EnemyStats>();
        attackDamage = stats.GetStat(StatType.ATTACK_DAMAGE);
        damageMpl = stats.GetStat(StatType.DAMAGE_MULTIPLIER);
        stats.GetStat(StatType.MOVEMENT_SPEED).OnStatChangeEvent += OnMovementSpeedChange;
        stats.GetStat(StatType.ATTACK_SPEED).OnStatChangeEvent += OnAttackSpeedChange;

        OnMovementSpeedChange(stats.GetStat(StatType.MOVEMENT_SPEED));
        OnAttackSpeedChange(stats.GetStat(StatType.ATTACK_SPEED));

        target = GameObject.Find("Player").transform;
        width = GetComponent<CapsuleCollider>().radius * 2;
        attackRange += width / 2 + target.GetComponent<CharacterController>().radius;
        sqrtStartAttackDistance = (attackRange * 0.85f) * (attackRange * 0.85f);
        movement.Goal = target;
        ControlsEnabled = true;
    }
	
    void Update ()
    {
        if (!ControlsEnabled)
            return;
        if (Vector3.SqrMagnitude(target.position - transform.position) <= sqrtStartAttackDistance)
        {
            StartAttack();
        }
    }

    public void Knockback(Vector3 direction)
    {
        knockbackDirection = direction;
        ControlsEnabled = false;
        transform.rotation = Quaternion.LookRotation(-direction, Vector3.up);
        anim.SetTrigger("Knockback");
    }

    public void StartAttack()
    {
        ControlsEnabled = false;
        anim.SetTrigger("Attack");
    }
    public void ExecuteAttackLogic()
    {
        Collider[] colls = Physics.OverlapCapsule(transform.position, transform.position + transform.forward * attackRange, width, playerMask);
        if (colls.Length == 0)
            return;

        DamageInfo damageInfo = new DamageInfo(attackDamage.Quantity * damageMpl.Quantity, 0);
        damageInfo.fromDirection = transform.forward;
        for (int i = 0; i < colls.Length; ++i)
        {
            colls[i].GetComponent<Health>().TakeDamage(damageInfo);
        }
    }

    private void OnMovementSpeedChange(Stat stat)
    {
        agent.speed = stat.Quantity;
        anim.SetFloat("MovementSpeed", stat.Quantity/stat.BaseQuantity);
    }

    private void OnAttackSpeedChange(Stat stat)
    {
        anim.SetFloat("AttackSpeed", stat.Quantity);
    }
}
