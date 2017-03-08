using UnityEngine;
using System.Collections;

public class Banishment : MonoBehaviour {
    public int maxBanishmentValue=100;
    private int currentBanishmentValue;
    public int CurrentBanishmentValue
    {
        get
        {
            return currentBanishmentValue;
        }
        set
        {
            currentBanishmentValue = value;
            if (OnBanishmentChange != null)
            {
                OnBanishmentChange((float)(currentBanishmentValue) / maxBanishmentValue);
            }
        }
    }
    public delegate void BanishmentChangeAction(float banishmentInPercents);
    public event BanishmentChangeAction OnBanishmentChange;

    private PlayerHealth health;

    void Awake()
    {
        health = GetComponent<PlayerHealth>();
        currentBanishmentValue = maxBanishmentValue;
    }
    public void RitualCompleted( int banishmentValue)
    {
        health.TakeDamage(banishmentValue);
    }
    public void RecoverBanishmentValue(int banishmentValueIncrease)
    {
        health.CurrentHealth += banishmentValueIncrease;
    }
}
