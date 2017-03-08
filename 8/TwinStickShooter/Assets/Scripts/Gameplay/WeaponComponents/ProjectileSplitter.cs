using UnityEngine;
using System.Collections;
[System.Serializable]
public class ProjectileSplitter : WeaponComponent, IBulletModifier {
    public int amount = 4;
    [Range(10, 180)]public float coneWidth = 45;
    private Projectile[] additionalProjectiles;
    public void Modify(Projectile bullet) {
        if (additionalProjectiles == null) {
            additionalProjectiles = new Projectile[amount];
        }

        for (int i = 0; i < amount; ++i) {
            additionalProjectiles[i] = ObjectPool.instance.GetObjectFromPool(bullet.gameObject.name).GetComponent<Projectile>();
            additionalProjectiles[i].transform.position = bullet.transform.position;
            additionalProjectiles[i].transform.rotation = bullet.transform.rotation;

            float additionalRotation = i / ((float)(amount - 1) / 2) * coneWidth - coneWidth;
            additionalProjectiles[i].transform.Rotate(Vector3.up, additionalRotation);
            bullet.childProjectiles.Add(additionalProjectiles[i]);
        }
        
    }
}
