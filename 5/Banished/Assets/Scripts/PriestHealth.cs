using UnityEngine;
using System.Collections;

public class PriestHealth : Health {
    private Priest priest;
    protected override void Init()
    {
        priest = GetComponent<Priest>();
    }
    public override void TakeDamage(int damage)
    {
        priest.LoseRitualProgres();
        base.TakeDamage(damage);

    }
    protected override void Death()
    {
        base.Death();
        GameManager.instance.PriestDeath(priest.killReward);
    }
}
