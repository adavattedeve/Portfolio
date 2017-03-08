using UnityEngine;

[RequireComponent(typeof(Health))]
public class Stability : MonoBehaviour {

    private Stat maxStability;
    private Stat stabilityPerSecond;

    private float currentStability;

    private ICharacterControl characterControl;
    void Awake()
    {
        GetComponent<Health>().OnTakeDamageEvent += TakeDamage;
        characterControl = (ICharacterControl)GetComponent(typeof(ICharacterControl));
    }

    void Start ()
    {
        maxStability = GetComponent<EnemyStats>().GetStat(StatType.STABILITY);
        stabilityPerSecond = GetComponent<EnemyStats>().GetStat(StatType.STABILITY_REGEN);
        currentStability = maxStability.Quantity;
    }

    void Update()
    {
        currentStability += stabilityPerSecond.Quantity * Time.deltaTime;
        currentStability = currentStability > maxStability.Quantity ? maxStability.Quantity : currentStability;
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        currentStability -= damageInfo.impact;
        if (currentStability < 0)
        {
            characterControl.Knockback(damageInfo.fromDirection);
            currentStability = maxStability.Quantity;
        }
        
    }

}
