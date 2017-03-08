using UnityEngine;
using System.Collections;

public class PlayerVisualEffects : MonoBehaviour {

    [SerializeField]
    private ObjectToBePooled damagedEffectPoolData;
    private Vector3 lastDamageFromDirection = Vector3.zero;

    private Health health;
    
    void Awake()
    {
        health = GetComponent<Health>();
        health.OnTakeDamageEvent += TakeDamage;
        health.OnDeathEvent += Death;
    }

    public void TakeDamage(DamageInfo info)
    {
        BloodSplatCreator.CreateBloodSplats(transform.position, BloodSplatCreator.SplatSize.SMALL);
        ObjectPool.instance.GetObjectFromPool(damagedEffectPoolData, transform.position + new Vector3(0, 1.5f, 0), Quaternion.LookRotation(-info.fromDirection, Vector3.up));
        ObjectShaker.AddShake(Camera.main.transform, new Vector3(2, 2, 2), new Vector3(1, 1, 1), 0.35f);
        lastDamageFromDirection = info.fromDirection;

    }

    public void Death()
    {
        BloodSplatCreator.CreateBloodSplats(transform.position, BloodSplatCreator.SplatSize.LARGE);
        BloodSplatCreator.CreateBloodSplats(transform.position, BloodSplatCreator.SplatSize.LARGE);
    }

}
