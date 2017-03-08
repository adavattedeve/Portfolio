using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour {
    public int damage;
    public ParticleSystem bloodParticles;
    public void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            if (GameManager.instance.PlayerBanishment != null)
            {
                GameManager.instance.Player.GetComponent<Health>().TakeDamage(damage);
                if (bloodParticles != null)
                {
                    bloodParticles.Stop();
                    bloodParticles.transform.position = coll.contacts[0].point;
                    bloodParticles.Play();
                }

            }
        }
        else if (coll.gameObject.layer == LayerMask.NameToLayer("Enemy") )
        {
            coll.gameObject.GetComponent<Health>().TakeDamage(damage);
            if (bloodParticles != null)
            {
                bloodParticles.Stop();
                bloodParticles.transform.position = coll.contacts[0].point;
                bloodParticles.Play();
            }
        }
    }
}
