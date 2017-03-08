using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour {
    [SerializeField]
    private Transform gunSlot;
    public bool AbleToShoot {
        get { return currentGun != null && currentGun.readyToShoot; } }
    public bool AbleToReload
    {
        get { return currentGun != null && currentGun.ableToReload; }
    }

    private List<Gun> guns = new List<Gun>();
    private Gun currentGun;


    List<WeaponComponent> weaponEffects = new List<WeaponComponent>();
    List<IStatModifier> statModifiers = new List<IStatModifier>();
    List<IUpdateable> updateableModifiers = new List<IUpdateable>();

    private Animator anim;
    
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    void Start ()
    {
        //AddGun("OtherGun");
        AddGun("Shotgun");
        AddGun("DefaultGun");


        ChangeGun(0);
    }

    void Update() {
        for (int i = 0; i < updateableModifiers.Count; ++i) {
            if (!updateableModifiers[i].Update(Time.deltaTime)) {
                RemoveWeaponEffect((WeaponComponent)updateableModifiers[i]);
            }
            
        }
    }

    public void ChangeGun(int gunIndex) {

        if (gunIndex >= guns.Count)
            return;
        else if (guns[gunIndex] == currentGun)
            return;

        if (currentGun != null) {
            anim.SetLayerWeight(currentGun.MovementAnimationLayer * 3 + 1, 0f);
            anim.SetLayerWeight(currentGun.UpperBodyOverdriveAnimationLayer * 3 + 2, 0f);
            anim.SetLayerWeight(currentGun.UpperBodyAdditiveAnimationLayer * 3 + 3, 0f);
            currentGun.Change(false);
        }
        
        guns[gunIndex].Change(true);
        currentGun = guns[gunIndex];

        anim.SetLayerWeight(currentGun.MovementAnimationLayer * 3 + 1, 1f);
        anim.SetLayerWeight(currentGun.UpperBodyOverdriveAnimationLayer * 3 + 2, 1f);
        anim.SetLayerWeight(currentGun.UpperBodyAdditiveAnimationLayer * 3 + 3, 1f);
        anim.SetInteger("ActiveLayer", currentGun.UpperBodyOverdriveAnimationLayer);
        anim.SetTrigger("Equip");
        if (currentGun.CurrentClip == 0) {
            TryReload();
        }
    }

    public void ChangeGun()
    {
        for (int i = 0; i < guns.Count; ++i) {
            if (guns[i] != currentGun) {
                ChangeGun(i);
                return;
            }
        }
    }

    public void TryShoot() {
        if (AbleToShoot) {
            anim.SetTrigger("Shoot");
        } else if (currentGun.CurrentClip == 0)
            TryReload();
    }
    //Called from animator
    public void Shoot() {
        currentGun.Shoot(weaponEffects);
        if (currentGun.CurrentClip == 0)
            TryReload();
    }
   
    public void TryReload() {
        if (AbleToReload) {
            anim.SetTrigger("Reload");
        }
    }
    //Called from animator
    public void Reload() {
        currentGun.FinishReloading();
    }
    public void AddGun(GameObject gunObject) {
        Gun gun = gunObject.GetComponent<Gun>();
        if (gun == null)
            return;

        gun.transform.SetParent(gunSlot);
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localRotation = Quaternion.identity;
        gun.gameObject.SetActive(false);
        gun.SetStatReferences((CharacterStats)GetComponent(typeof(CharacterStats)));
        guns.Add(gun);
    }

    public void AddGun(string gunName) {
        GameObject gunGO = Resources.Load<GameObject>("Prefabs/Guns/" + gunName) as GameObject;
        AddGun(Instantiate(gunGO));
    }

    public void AddWeaponEffect(WeaponComponent weaponComponent) {

        RemoveWeaponEffect(weaponComponent);

        if (weaponComponent is IStatModifier) {
            statModifiers.Add((IStatModifier)weaponComponent);
            for (int j = 0; j < guns.Count; ++j) {
                guns[j].UpdateStats(statModifiers);
            }
        } else {
            weaponEffects.Add(weaponComponent);
        }

        if (weaponComponent is IUpdateable) {
            updateableModifiers.Add((IUpdateable)weaponComponent) ;
        }
    }

    public void RemoveWeaponEffect(WeaponComponent weaponComponent) {
        if (weaponComponent is IStatModifier) {
            statModifiers.Remove((IStatModifier)weaponComponent);
        }
        else {
            weaponEffects.Remove(weaponComponent);
        }

        if (weaponComponent is IUpdateable) {
            updateableModifiers.Remove((IUpdateable)weaponComponent);
        }
    }
}
