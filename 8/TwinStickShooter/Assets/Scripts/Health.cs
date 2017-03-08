using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    public float maxHealth = 50;

    private float currentHealth;
    private Stat maxHealthStat;

    public Stat MaxHealthStat { set{ maxHealthStat = value; } }

    public delegate void OnTakeDamage(DamageInfo info);
    public delegate void OnDeath();

    public event OnTakeDamage OnTakeDamageEvent;
    public event OnDeath OnDeathEvent;

    void Start() {
        if (maxHealthStat != null)
        {
            maxHealth = maxHealthStat.Quantity;
            maxHealthStat.OnStatChangeEvent += OnMaxHealthChange;
        }

        currentHealth = maxHealth;
    }

    public void TakeDamage(DamageInfo damageInfo) {
        if (currentHealth <= 0)
            return;
        currentHealth -= damageInfo.damage;

        if (OnTakeDamageEvent != null)
            OnTakeDamageEvent(damageInfo);
        if (currentHealth <= 0)
            Death();
    }

    private void Death() {
        if (OnDeathEvent != null)
            OnDeathEvent();
        if (gameObject.name != "Player")
            Destroy(gameObject);
    }

    private void OnMaxHealthChange(Stat healthStat)
    {
        float difference = healthStat.Quantity - maxHealth;
        maxHealth += difference;
        currentHealth += difference;

        maxHealth = maxHealth > 0 ? maxHealth : 1;
        currentHealth = currentHealth > 0 ? currentHealth : 1;
    }
}
