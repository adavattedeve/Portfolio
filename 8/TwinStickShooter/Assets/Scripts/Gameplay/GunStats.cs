using UnityEngine;
using System.Collections;
[System.Serializable]
public class GunStats {

    [Header("Gun properties")]

    public int clipSize = 8;
    public float reloadTime = 1f;
    public float timeBetweenShots = 0.15f;
    public float baseAttackSpeed = 1f;
    public int baseDamage = 10;
    public int baseImpact = 20;
    public int ammoCapacity = 120;

    public StatChange attackSpeed;

    public void CopyValuesFrom(GunStats _stats) {
        clipSize = _stats.clipSize;
        reloadTime = _stats.reloadTime;
        timeBetweenShots = _stats.timeBetweenShots;
        baseAttackSpeed = _stats.baseAttackSpeed;
        baseDamage = _stats.baseDamage;
        baseImpact = _stats.baseImpact;
        ammoCapacity = _stats.ammoCapacity;

        attackSpeed = new StatChange(StatType.ATTACK_SPEED, 0, baseAttackSpeed);
        
    }
}
