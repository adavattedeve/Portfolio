using UnityEngine;
public class PlayerStats : CharacterStats
{
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float attackSpeed;
    [SerializeField]
    private float bonusDamage;
    [SerializeField]
    private float damageMultiplier;
    [SerializeField]
    private float bonusImpact;
    [SerializeField]
    private float impactMultiplier;
    [SerializeField]
    private float maxHealth;

	protected override void Init()
    {
        stats.Add(new Stat(StatType.MOVEMENT_SPEED, movementSpeed));
        stats.Add(new Stat(StatType.ATTACK_SPEED, attackSpeed));
        stats.Add(new Stat(StatType.BONUS_DAMAGE, bonusDamage));
        stats.Add(new Stat(StatType.DAMAGE_MULTIPLIER, damageMultiplier));
        stats.Add(new Stat(StatType.BONUS_IMPACT, bonusImpact));
        stats.Add(new Stat(StatType.IMPACT_MULTIPLIER, impactMultiplier));
        stats.Add(new Stat(StatType.HEALTH, maxHealth));
    }

}


