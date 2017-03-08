using UnityEngine;
using System.Collections;

public class BodyPartCollisionEffects : MonoBehaviour {

    [SerializeField]
    private float smallSplatCollisionImpulseLimit = 0.5f;
    [SerializeField]
    private float mediumSplatCollisionImpulseLimit = 10f;

    public void OnCollisionEnter(Collision coll) {
        if (!coll.transform.gameObject.CompareTag("Floor")) {
            return;
        }
        bool spawnSplat = false;
        BloodSplatCreator.SplatSize splatSize = BloodSplatCreator.SplatSize.SMALL;
        if (coll.impulse.sqrMagnitude >= mediumSplatCollisionImpulseLimit * mediumSplatCollisionImpulseLimit)
        {
            splatSize = BloodSplatCreator.SplatSize.MEDIUM;
            spawnSplat = true;
        }
        else if (coll.impulse.sqrMagnitude >= smallSplatCollisionImpulseLimit * smallSplatCollisionImpulseLimit)
        {
            splatSize = BloodSplatCreator.SplatSize.SMALL;
            spawnSplat = true;
        }

        if (spawnSplat)
        {
            Vector3 point = coll.contacts[0].point;
            point.y = coll.transform.position.y;
            BloodSplatCreator.CreateBloodSplats(point, splatSize);
            /*
           Vector3 point = coll.contacts[0].point;
           Vector3 dir = -coll.contacts[0].normal; 

           point -= dir;
           RaycastHit hitInfo;
           if (coll.collider.Raycast(new Ray(point, dir), out hitInfo, 2))
           //if (Physics.Raycast(new Ray(point, dir), out hitInfo, 2, LayerMask.GetMask("Environment")))
           {
               if (Vector3.Angle(hitInfo.normal, Vector3.up) < 10f)
                   bloodSplatCreator.CreateSmallSplats(hitInfo.normal);
           }
           */
        }

    }

}
