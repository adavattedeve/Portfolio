using UnityEngine;
using System.Collections;

public class Priest : MonoBehaviour {

    public float ritualLength = 10f;
    public int ritualStrength = 25;
    public int killReward = 10;
    public int damageOnContact = 25;
    public float ritualProgresLostOnHitInTime = 1f;

    public float timeBetweenRituals = 3f;

    [Header("RitualAnimParams")]
    public Vector2 maxScale;
    public Vector2 minScale;
    public float scaleChangeSpeed;
    private Vector3 defaultScale;
    private bool cantDealDamage;
    public Transform ritualParticleTransform;
    public GameObject ritaulParticlesPrefab;
    private float ritualProgres=0;
    public float RitualProgres {
        get {
            return ritualProgres;
        }
        set {
            ritualProgres = Mathf.Clamp01(value);
            if (OnRitualProgresChange != null)
            {
                OnRitualProgresChange(ritualProgres);
            }
        }
    }
    public delegate void RitualProgresChangeAction(float newRitualProgres);
    public event RitualProgresChangeAction OnRitualProgresChange;

    private float ritualTime = 0;

    void Awake()
    {
        defaultScale = transform.localScale;
        cantDealDamage = true;
    }
    void Start()
    {
        GameManager.OnGameOver += DeActivate;
        StartCoroutine(IdleForTime());
    }
    void OnDisable()
    {
        GameManager.OnGameOver -= DeActivate;
        StopAllCoroutines();
    }
    private IEnumerator IdleForTime()
    {
        RitualProgres = 0;
        yield return new WaitForSeconds(timeBetweenRituals);
        cantDealDamage = false;
        StartCoroutine(Ritual());
    }
    private IEnumerator Ritual()
    {
        ritualTime = 0;
        bool scalingXUp = true;
        Vector3 newScale;
        while (ritualTime < ritualLength)
        {
            yield return new WaitForEndOfFrame();
            newScale = transform.localScale;
            if (scalingXUp)
            {
                if (newScale.x <= minScale.x || newScale.y >= maxScale.y)
                {
                    scalingXUp = false;
                }
                else
                {
                    newScale.x -= scaleChangeSpeed * Time.deltaTime * RitualProgres;
                    newScale.y += scaleChangeSpeed * Time.deltaTime * RitualProgres;
                    transform.localScale = newScale;
                }
            }
            else
            {
                if (newScale.x >= maxScale.x || newScale.y <= minScale.y)
                {
                    scalingXUp = true;
                }
                else
                {
                    newScale.x += scaleChangeSpeed * Time.deltaTime * RitualProgres;
                    newScale.y -= scaleChangeSpeed * Time.deltaTime * RitualProgres;
                    transform.localScale = newScale;
                }
            }
           
            ritualTime += Time.deltaTime;
            RitualProgres = Mathf.Clamp(ritualTime / ritualLength, 0, 1);
        }
        AudioManager.instance.RitualCompleted();
         GameObject temp = Instantiate(ritaulParticlesPrefab, ritualParticleTransform.position, ritualParticleTransform.rotation) as GameObject;
        temp.GetComponent<Particles>().DetachAndDestroy(true);
        //ritualParticles.DetachAndDestroy(false);
        transform.localScale = defaultScale;
        if (GameManager.instance.PlayerBanishment != null)
        {
            GameManager.instance.PlayerBanishment.RitualCompleted(ritualStrength);
            StartCoroutine(IdleForTime());
        }
   
    }

    public void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            if (GameManager.instance.Player != null && !cantDealDamage)
            {
                GameManager.instance.Player.GetComponent<Health>().TakeDamage(damageOnContact);
            }

        }
    }

    public void DeActivate()
    {
        StopAllCoroutines();
    }
    public void LoseRitualProgres()
    {
        ritualTime -= ritualProgresLostOnHitInTime;
    }
}
