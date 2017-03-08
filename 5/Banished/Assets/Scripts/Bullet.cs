using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    public float minSqrtVelocity = 16f;
    private int damage;
    private Rigidbody2D rb;
    public Particles smokeParticles;
    public Particles bloodParticles;
    public Particles sparkParticles;

    // Use this for initialization
    void Awake () {
        rb = GetComponent<Rigidbody2D>();
	}

    public void Launch(Vector3 force, int _damage)
    {
        damage = _damage;
        rb.AddForce(force, ForceMode2D.Impulse);
    }
    void Update()
    {
        if (rb.velocity.sqrMagnitude < minSqrtVelocity)
        {
            rb.AddForce(rb.velocity*2, ForceMode2D.Impulse);
        }
    }
    public void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            AudioManager.instance.WallHitWithBullet();
            sparkParticles.gameObject.SetActive(true);
            sparkParticles.DetachAndDestroy(true);
        }
        else if (coll.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            AudioManager.instance.CharacterHitWithBullet();
            bloodParticles.gameObject.SetActive(true);
            bloodParticles.DetachAndDestroy(true);

            Health enemy = coll.gameObject.GetComponent<Health>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }


        }
        else {
            return;
        }
        smokeParticles.DetachAndDestroy(false);
        Destroy(gameObject);
    }
}
