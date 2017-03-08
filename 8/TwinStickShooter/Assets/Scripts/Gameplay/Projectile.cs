using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour {
    public DamageInfo damageInfo;
    public float speed = 20f;
    public float maxDistance = 30f;
    public string ingoreLayer = "";

    protected bool launched;
    protected float distanceTravelled = 0f;
    [Header("How many enemies can projectile pierce.")]
    public int piercing = 0;
    protected List<Health> damaged = new List<Health>();



    protected Rigidbody rb;
    [System.NonSerialized]
    public List<Projectile> childProjectiles = new List<Projectile>();

    public void Launch(DamageInfo _damageInfo) {
        damageInfo = _damageInfo;
        launched = true;
        for (int i = 0; i < childProjectiles.Count; ++i) {
            childProjectiles[i].Launch(damageInfo);
        }
    }
	// Use this for initialization
	void Awake () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (launched) {
            distanceTravelled += Time.deltaTime * speed;
            if (distanceTravelled >= maxDistance)
                DestroyProjectile();
            Vector3 newPos = transform.position;
            newPos.x += Time.deltaTime * speed * transform.forward.x;
            newPos.z += Time.deltaTime * speed * transform.forward.z;
            rb.MovePosition(newPos);
            
        }
	}

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.GetMask(ingoreLayer))
            return;
       
        Health health = other.GetComponent<Health>();
        if (health != null && !damaged.Contains(health))
        {
            damageInfo.fromDirection = transform.forward;
            damageInfo.fromDirection.y = 0;
            health.TakeDamage(damageInfo);
            damaged.Add(health);
            if (damaged.Count >= piercing)
                DestroyProjectile();
        }

    }

    private void DestroyProjectile() {
        launched = false;
        distanceTravelled = 0;
        childProjectiles.Clear();
        damaged.Clear();
        ObjectPool.instance.PoolObject(gameObject);
    }
}
