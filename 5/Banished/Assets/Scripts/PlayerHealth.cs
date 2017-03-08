using UnityEngine;
using System.Collections;

public class PlayerHealth : Health {
    public float invincibleTimeOnDamageTaken=1.5f;
    public float deathForceMpl = 3f;
    private bool invincible = false;
    private Material mat;
    private bool alphaToLower = true;
    private float defaultAlpha;
    public float minAlpha = 0.15f;
    public float alphaChangeSpeed = 1;
    private Color materialColor;
    private playerBodyPartControl bodyPartControl;
    private Rigidbody2D rb;
    protected override void Init()
    {
        bodyPartControl = GetComponentInChildren<playerBodyPartControl>();
        rb = GetComponent<Rigidbody2D>();
        mat = GetComponent<SpriteRenderer>().material;
        Color matColor = GetComponent<SpriteRenderer>().sharedMaterial.color;
        matColor.a = 1;
        GetComponent<SpriteRenderer>().sharedMaterial.color = matColor;
        defaultAlpha = mat.color.a;
    }

    public override void TakeDamage(int damage)
    {
        if (!invincible)
        {
            AudioManager.instance.CharacterHitWithBullet();
            GameManager.instance.PlayerTookDamage();
            CameraShake.instance.ScreenShake();
            base.TakeDamage(damage);
            StartCoroutine(Invincible());
        }
    }
    protected override void Death()
    {
        AudioManager.instance.PlayerDeath();
        bodyPartControl.DeathAnimation(rb.velocity * deathForceMpl);
        base.Death(); 
    }
    void Update()
    {
        if (invincible)
        {

            materialColor = mat.color;
            if (alphaToLower)
            {
                if (materialColor.a <= minAlpha)
                {
                    alphaToLower = false;
                }
                else
                {
                    materialColor.a -= Time.deltaTime * alphaChangeSpeed;
                }

            }
            else
            {
                if (materialColor.a >= defaultAlpha)
                {
                    alphaToLower = true;
                }
                else
                {
                    materialColor.a += Time.deltaTime * alphaChangeSpeed;
                }
            }
            mat.color = materialColor;
        }

    }
    private IEnumerator Invincible()
    {
        invincible = true;
        yield return new WaitForSeconds(invincibleTimeOnDamageTaken);
        invincible = false;
        materialColor = mat.color;
        materialColor.a = defaultAlpha;
        mat.color = materialColor;
    }
       
}
