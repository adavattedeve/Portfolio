using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(WeaponController))]
public class PlayerInput : MonoBehaviour {

    public bool actionInputEnabled = true;
    public bool movementInputEnabled = true;

    public float actionBufferTime = 0.25f;

    private PlayerController controller;
    private WeaponController weaponController;

    private float bufferTimer = 0;
    private System.Action actionBuffer = null;

    // Use this for initialization
    void Awake () {
        controller = GetComponent<PlayerController>();
        weaponController = GetComponent<WeaponController>();
    }

	
    void Update () {

        bufferTimer += Time.deltaTime;

        if (bufferTimer >= actionBufferTime) {
            bufferTimer = 0;
            actionBuffer = null;
        }

        if (movementInputEnabled) {
            MovementInput();
        }
        ActionInput();

        if (actionInputEnabled) {
            if (actionBuffer != null) {
                actionBuffer();
                actionBuffer = null;
            }
            else if (Input.GetButton("Fire"))
                weaponController.TryShoot();


        }

    }

    private void AddToActionBuffer(System.Action action) {
        actionBuffer = action;
        bufferTimer = 0;
    }

    private void MovementInput () {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        controller.MovementDirection = (new Vector3(h, 0, v)).normalized;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50f, LayerMask.GetMask("Input")))
        {
            Vector3 aimDir = hit.point - transform.position;
            aimDir.y = 0;
            controller.AimDirection = aimDir;
        }
    }

    private void ActionInput () {
        if (Input.GetButtonDown("Reload"))
        {
            AddToActionBuffer(weaponController.TryReload);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddToActionBuffer(weaponController.ChangeGun);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddToActionBuffer(weaponController.ChangeGun);
        }
        else if (Input.GetButtonDown("Fire"))
        {
            AddToActionBuffer(weaponController.TryShoot);
        }
    }
}
