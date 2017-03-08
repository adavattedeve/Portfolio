using UnityEngine;

public interface ICharacterControl {
    bool ControlsEnabled { get; set; }
    Vector3 KnockbackDirection { get; }

    void Knockback(Vector3 direction);
    //void AddStatusEffect(StatusEffect statusEffect)
}
