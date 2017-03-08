using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
    public float baseShootForce = 10f;
    private GunManager gunManager;
    private Rigidbody2D rb;
	// Use this for initialization
	void Awake () {
        gunManager = GetComponentInChildren<GunManager>();
        rb = GetComponent<Rigidbody2D>();
	}

    void Start()
    {
        gunManager.ChangeGun(0);
    }
	void Update () {
        //Input
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        gunManager.RotateTowards(mousePosition);
        #region Input
        //float scrollWheel = Input.GetAxisRaw("Mouse ScrollWheel");
        //if (scrollWheel != 0)
        //{
        //    if (scrollWheel > 0)
        //    {
        //        gunManager.ChangeGun(gunManager.CurrentGunIndex + 1);
        //    }
        //    else {
        //        gunManager.ChangeGun(gunManager.CurrentGunIndex - 1);
        //    }
        //}
        if (Input.GetButtonDown("Pistol"))
        {

            Vector3 force = gunManager.Shoot(0) * baseShootForce;

            rb.AddForce(force, ForceMode2D.Impulse);
        }
        else if (Input.GetButtonDown("Shotgun"))
        {

            Vector3 force = gunManager.Shoot(1) * baseShootForce;

            rb.AddForce(force, ForceMode2D.Impulse);
        }
        //else if (Input.GetButtonDown("Weapon1"))
        //{
        //    gunManager.ChangeGun(0);
        //}
        //else if (Input.GetButtonDown("Weapon2"))
        //{
        //    gunManager.ChangeGun(1);
        //}
        //else if (Input.GetButtonDown("Weapon3"))
        //{
        //    gunManager.ChangeGun(2);
        //}

        #endregion
    }
}
