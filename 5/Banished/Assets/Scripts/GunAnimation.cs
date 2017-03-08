using UnityEngine;
using System.Collections;

public class GunAnimation : MonoBehaviour {
    [Header("ShootAnimation params")]
    public float shootAngle = 20f;
    public float rotationSpeed= 10f;

    private float deltaAngle = 0;
    public void ShootAnimation()
    {
        float angle = shootAngle;
        if (transform.right.x > 0)
        {            
        }
        else {
            angle *= -1;
        }
        transform.Rotate(Vector3.forward, angle);
        deltaAngle += angle;

    }
	void Update () {
        
        if (Mathf.Abs(deltaAngle) < 0.001)
            return;

        float angle = Time.deltaTime * rotationSpeed;
        if (deltaAngle > 0)
        {
            angle *= -1;
        }

        transform.Rotate(Vector3.forward, angle);
        deltaAngle += angle;

    }
}
