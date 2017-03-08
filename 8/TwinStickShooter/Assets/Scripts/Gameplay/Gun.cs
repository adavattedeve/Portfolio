using UnityEngine;
using System.Collections.Generic;

public class Gun : MonoBehaviour {
    public bool readyToShoot
    {
        get { return !reloading && timeBetweenShotsLeft <= 0 && currentClip > 0; }
    }

    public bool ableToReload { get{ return currentStats.clipSize > currentClip && ammoLeft > 0; } }
    public int MovementAnimationLayer { get { return movementAnimationLayer; } }
    public int UpperBodyOverdriveAnimationLayer { get { return upperBodyOverdriveAnimationLayer; } }
    public int UpperBodyAdditiveAnimationLayer { get { return upperBodyAdditiveAnimationLayer; } }
    public GunStats CurrentStats { get { return currentStats; } }
    public int CurrentClip { get { return currentClip; } }

    protected static bool requiredObjectsInPool = false;

    [SerializeField]
    protected string gunName = "Gun name";
    [SerializeField]
    protected ObjectToBePooled projectilePoolData;
    [SerializeField]
    protected int movementAnimationLayer = 0;   
    [SerializeField]
    protected int upperBodyOverdriveAnimationLayer = 0;
    [SerializeField]
    protected int upperBodyAdditiveAnimationLayer = 0;
    [SerializeField]
    protected Transform bulletSpawnPoint;

    [SerializeField]
    protected GunStats baseStats;

    protected GunStats currentStats;
    protected List<WeaponComponent> weaponEffects = new List<WeaponComponent>();

    protected int currentClip = 0;
    
    protected float reloadTimeLeft = 0;
    protected float timeBetweenShotsLeft = 0;
    protected int ammoLeft;
    protected bool reloading = false;

    protected Animator gunAnimator;
    protected CharacterStats characterStats;
    protected Stat bonusDamage;
    protected Stat damageMpl;
    protected Stat bonusImpact;
    protected Stat impactMpl;
    protected Stat attackSpeedMpl;

    void Awake() {
        currentStats = new GunStats();
        currentStats.CopyValuesFrom(baseStats);
        ammoLeft = currentStats.ammoCapacity;
        gunAnimator = GetComponent<Animator>();
        FinishReloading();
        Init();
    }
    void Start() {
        ObjectPool.instance.AddNewObjectToBePooled(projectilePoolData);
    }
    protected virtual void Init() {

    }

    void Update () {
        if (reloadTimeLeft > 0)
            reloadTimeLeft -= Time.deltaTime;
        else if (reloading)
            FinishReloading();

        if (timeBetweenShotsLeft > 0)
            timeBetweenShotsLeft -= Time.deltaTime;
    }

    public virtual void Shoot(List<WeaponComponent> effects) {

        gunAnimator.SetTrigger("Shoot");
        for (int i = 0; i < weaponEffects.Count; ++i) {
            effects.Add(weaponEffects[i]);
        }
        float damage = (currentStats.baseDamage + bonusDamage.Quantity) * damageMpl.Quantity;
        float impact = (currentStats.baseImpact + bonusImpact.Quantity) * impactMpl.Quantity;
        DamageInfo damageInfo = new DamageInfo(damage, impact);
        for (int i = 0; i < effects.Count; ++i) {
            if (effects[i] is IDamageModifier) {
                ((IDamageModifier)effects[i]).Modify(damageInfo);
            }
        }
        
        Projectile bullet = ObjectPool.instance.GetObjectFromPool(projectilePoolData).GetComponent<Projectile>();
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.transform.rotation = bulletSpawnPoint.rotation;

        for (int i = 0; i < effects.Count; ++i) {
            if (effects[i] is IBulletModifier) {
                ((IBulletModifier)effects[i]).Modify(bullet);
            }
        }
        bullet.Launch(damageInfo);
        --currentClip;
        if (currentClip > 0) {
            timeBetweenShotsLeft = currentStats.timeBetweenShots / attackSpeedMpl.Quantity;
        }

        for (int i = 0; i < weaponEffects.Count; ++i) {
            effects.Remove(weaponEffects[i]);
        }
    }

    public void UpdateStats(List<IStatModifier> statModifiers) {
        currentStats.CopyValuesFrom(baseStats);
        for (int i = 0; i < statModifiers.Count; ++i) {
            statModifiers[i].ModifyStats(currentStats);
        }
    }

    public void FinishReloading() {
        ammoLeft += currentClip;
        currentClip = ammoLeft > currentStats.clipSize ? currentStats.clipSize : ammoLeft;
        ammoLeft -= currentClip;
    }

    public virtual void Change(bool to) {
        if (to)
            characterStats.AddStatChange(currentStats.attackSpeed);
        else
            characterStats.RemoveStatChange(currentStats.attackSpeed);

        gameObject.SetActive(to);
    }

    public void SetStatReferences(CharacterStats stats)
    {
        Debug.Log("setting up stat refs");
        characterStats = stats;
        bonusDamage = stats.GetStat(StatType.BONUS_DAMAGE);
        damageMpl = stats.GetStat(StatType.DAMAGE_MULTIPLIER);
        bonusImpact = stats.GetStat(StatType.BONUS_IMPACT);
        impactMpl = stats.GetStat(StatType.IMPACT_MULTIPLIER);
        attackSpeedMpl = stats.GetStat(StatType.ATTACK_SPEED);
    }
}
