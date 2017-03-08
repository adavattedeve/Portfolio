using UnityEngine;
using System.Collections;

public class Gun: MonoBehaviour {

    public float forceMpl = 5f;
    public float tBetweenShots = 0.1f;
    public int clipSize=10;
    public float reloadTime = 1f;
    public int bulletsPerShot = 5;
    public GameObject bulletPrefab;
    public Transform bulletStartPos;
    public float bulletSpeed = 15f;
    public int damage=25;
    public float bulletOffset = 0.25f;
    public bool unlimitedAmmo = false;
    private int bulletsLeft;
    private bool reloading = false;
    private bool onCD = false;


    private GunAnimation anim;
    private bool CanShoot {
        get {
            return !reloading && !onCD;
        }
    }
    void Awake()
    {
        bulletsLeft = clipSize;
        anim = GetComponent<GunAnimation>();
    }
    public Vector3 Shoot()
    {
        if (CanShoot)
        {
            if (name == "Pistol")
            {
                AudioManager.instance.PistolShoot();
            }
            else if (name == "Shotgun")
            {
                AudioManager.instance.ShotgunShoot();
            }
            if (!unlimitedAmmo)
            {
                --bulletsLeft;
            }
           
            StartCoroutine(GunCooldown());
            if (bulletsLeft<=0)
            {
                StartCoroutine(Reload());
            }
            for (int i = 0; i < bulletsPerShot; ++i)
            {
                GameObject go = Instantiate(bulletPrefab, bulletStartPos.position, transform.rotation) as GameObject;

                go.GetComponent<Bullet>().Launch((transform.right + transform.up * Random.Range(-bulletOffset, bulletOffset)) * bulletSpeed, damage);
            }
            Vector3 force = -1 * transform.right * forceMpl;
            if (anim != null)
            {
                anim.ShootAnimation();
            }
            return force;
        }
        return Vector3.zero;
    }
    public void UnEquip()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
    public void Equip()
    {
        gameObject.SetActive(true);
        if (onCD)
            StartCoroutine(GunCooldown());
        if (reloading)
            StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        bulletsLeft = clipSize;
        reloading = false;
    }
    private IEnumerator GunCooldown()
    {
        onCD = true;
        yield return new WaitForSeconds(tBetweenShots);
        onCD = false;
    }

}
