using UnityEngine;
public class EnemyStats : CharacterStats {

    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float attackSpeed;
    [SerializeField]
    private float attackDamage;
    [SerializeField]
    private float damageMultiplier;
    [SerializeField]
    private float maxStability;
    [SerializeField]
    private float stabilityRegen;
    [SerializeField]
    private float maxHealth;

    protected override void Init()
    {
        stats.Add(new Stat(StatType.MOVEMENT_SPEED, movementSpeed));
        stats.Add(new Stat(StatType.ATTACK_SPEED, attackSpeed));
        stats.Add(new Stat(StatType.ATTACK_DAMAGE, damageMultiplier));
        stats.Add(new Stat(StatType.DAMAGE_MULTIPLIER, damageMultiplier));
        stats.Add(new Stat(StatType.STABILITY, maxStability));
        stats.Add(new Stat(StatType.STABILITY_REGEN, stabilityRegen));
        stats.Add(new Stat(StatType.HEALTH, maxHealth));
    }
}
