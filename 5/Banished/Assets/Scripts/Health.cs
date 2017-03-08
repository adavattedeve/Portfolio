using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
    public float timeToDeath=10f;
    public int maxHealth=100;
    private int currentHealth;
    public int CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            if (OnHealthChange != null)
            {
                OnHealthChange((float)(currentHealth)/maxHealth);
            }
        }
    }
    public delegate void HealthChangeAction(float healthInPercents);
    public event HealthChangeAction OnHealthChange;

    void Awake () {
        currentHealth = maxHealth;
        Init();
	}
    protected virtual void Init()
    {

    }
	
    public virtual void TakeDamage(int damage)
    {
        
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Death();
        }
    }

    protected virtual void Death()
    {
        CameraShake.instance.ScreenShake();
        Destroy(gameObject);
    }
}
