using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunManager : MonoBehaviour {
    public List<Gun> guns;
    private int currentGunIndex=-1;
    public int CurrentGunIndex {
        get {
            return currentGunIndex;
        }
    }
    // Use this for initialization
    void Awake() {
        Transform[] gunTransforms = transform.GetComponentsInChildren<Transform>();
        guns = new List<Gun>();
        for (int i = 0; i < gunTransforms.Length; ++i)
        {
            Gun newGun = (Gun)gunTransforms[i].GetComponent(typeof(Gun));
            if (newGun != null)
            {
                newGun.UnEquip();
                guns.Add(newGun);
            }
        }
    }
    public void RotateTowards(Vector3 target)
    {
        Vector3 targetDir = target - transform.position;
        targetDir.z = 0;
        Vector3 right = transform.right;
        right.z = 0;
        float angle = Vector3.Angle(transform.right, targetDir);
        if (Vector3.Cross(right, targetDir).z<0)
        {
            angle *= -1;
        }
        transform.Rotate(Vector3.forward, angle);
    }

    public void ChangeGun(int targetGunIndex)
    {

        
        if (CheckGunIndex(targetGunIndex))
        {
            if (CheckGunIndex(currentGunIndex))
            {
                guns[currentGunIndex].UnEquip();
            }
            Debug.Log("equipping new gun at: " + targetGunIndex);
            guns[targetGunIndex].Equip();
            currentGunIndex = targetGunIndex;
        }
    }

    public Vector3 Shoot(int gunIndex)
    {
        if (currentGunIndex != gunIndex)
        {
            ChangeGun(gunIndex);
        }
        if (CheckGunIndex(currentGunIndex))
        {
            Vector3 force = guns[currentGunIndex].Shoot();
            return force;
        }
        return Vector3.zero;
    }

    private bool CheckGunIndex(int index) {
        if (index >= 0 && index < guns.Count && guns[index] != null)
        {
            return true;
        }
        return false;
    }
}
