
public enum StatType { MOVEMENT_SPEED, ATTACK_SPEED, HEALTH, STABILITY, STABILITY_REGEN, ATTACK_DAMAGE, BONUS_DAMAGE, DAMAGE_MULTIPLIER, BONUS_IMPACT, IMPACT_MULTIPLIER }

public class Stat
{
    private StatType type;
    private float baseQuantity;
    private float quantity;

    public StatType Type { get { return type; } }
    public float BaseQuantity { get { return baseQuantity; } set { baseQuantity = value; } }
    public float Quantity { get { return quantity; }
        set
        {
            quantity = value;
            if (OnStatChangeEvent != null)
            {
                OnStatChangeEvent(this);
            }
        }
    }

    public delegate void OnStatChange(Stat stat);

    public event OnStatChange OnStatChangeEvent;

    public Stat(StatType _type, float _quantity)
    {
        type = _type;
        baseQuantity = _quantity;
        quantity = _quantity;
    }
}