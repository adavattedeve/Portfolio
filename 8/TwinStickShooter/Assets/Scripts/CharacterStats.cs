using UnityEngine;
using System.Collections.Generic;

public class CharacterStats : MonoBehaviour {

    protected List<Stat> stats = new List<Stat>();
    protected List<StatChange> statChanges = new List<StatChange>();

    protected Animator anim;

    void Awake()
    {
        Init();
        if (GetStat(StatType.HEALTH) != null)
        {
            GetComponent<Health>().MaxHealthStat = GetStat(StatType.HEALTH);
        }
        anim = GetComponent<Animator>();
    }

    protected virtual void Init()
    {

    }

    void Update()
    {
        bool statsDirty = false;
        for (int i = 0; i < statChanges.Count; ++i)
        {
            if (statChanges[i].permanent)
                continue;

            statChanges[i].timeLeft -= Time.deltaTime;
            if (statChanges[i].timeLeft <= 0)
            {
                statChanges.RemoveAt(i);
                statsDirty = true;
                --i;
            }
        }

        if (statsDirty)
            UpdateStats();
    }

    public Stat GetStat(StatType _type)
    {
        for (int i = 0; i < stats.Count; ++i)
        {
            if (stats[i].Type == _type)
                return stats[i];
        }
        return null;
    }
    public void AddStatChange(StatChange change)
    {
        statChanges.Add(change);
        UpdateStats();
    }

    public void RemoveStatChange(StatChange change)
    {
        statChanges.Remove(change);
        UpdateStats();
    }

    protected void UpdateStats()
    {
        for (int i = 0; i < stats.Count; ++i)
        {
            UpdateStat(stats[i]);
        }

    }

    protected void UpdateStat(Stat stat)
    {
        float newQuantity = stat.BaseQuantity;
        float quantityMpl = 1;
        for (int i = 0; i < statChanges.Count; ++i)
        {
            if (stat.Type == statChanges[i].type)
            {
                newQuantity += statChanges[i].flatIncrease;
                quantityMpl *= statChanges[i].mpl;
            }
        }
        newQuantity *= quantityMpl;

        stat.Quantity = newQuantity;
    }

}
