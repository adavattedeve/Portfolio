using UnityEngine;
using System.Collections;

public class DamageInfo {
    public float damage;
    public float impact;
    public Vector3 fromDirection = Vector3.zero;
    public DamageInfo() {

    }
    public DamageInfo(float _damage, float _impact) {
        damage = _damage;
        impact = _impact;
    }
}
