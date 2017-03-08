using UnityEngine;
using System.Collections;

public class Particles : MonoBehaviour {
    public float timeToDestroy = 3f;
    public ParticleSystem ps;

    public void DetachAndDestroy(bool particlesEnabled)
    {
        ParticleSystem.EmissionModule em = ps.emission;
        em.enabled = particlesEnabled;
        transform.SetParent(null);
        transform.localScale = Vector3.one;
        Destroy(gameObject, timeToDestroy);
    }
}
