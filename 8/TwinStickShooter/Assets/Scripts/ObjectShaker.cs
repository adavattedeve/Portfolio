using UnityEngine;
using System.Collections.Generic;

public class ObjectShaker : MonoBehaviour {

    public float shakeTime = 4f;
    public Vector3 positionAmplitudes;
    public Vector3 rotationAmplitudes;

    private readonly static float smoothAmount = 5f;
    private readonly static float smoothTime = 0.25f;

    private static List<Shakeable> shakeables = new List<Shakeable>();
	
    public void AddSmallShake()
    {
        Shakeable test = new Shakeable(transform, positionAmplitudes, rotationAmplitudes, shakeTime);
        AddShake(test);
    }

    public void AddBigShake()
    {
        Shakeable test = new Shakeable(transform, positionAmplitudes * 2, rotationAmplitudes * 2, shakeTime * 3);
        AddShake(test);
    }

    public static void AddShake(Transform _transform, Vector3 posAmplitudes, Vector3 rotAmplitudes, float time)
    {
        AddShake(new Shakeable(_transform, posAmplitudes, rotAmplitudes, time));
    }

    public static void AddShake(Shakeable shakeable)
    {
        for (int i = 0; i <shakeables.Count; ++i)
        {
            if (shakeables[i].transform == shakeable.transform)
            {
                shakeables[i].maxTime = shakeables[i].maxTime > shakeable.timeLeft ? shakeables[i].maxTime : shakeable.timeLeft;

                shakeables[i].timeLeft += shakeable.timeLeft;
                shakeables[i].timeLeft = Mathf.Clamp(shakeables[i].timeLeft, 0, shakeables[i].maxTime);

                shakeables[i].positionAmplitudes = shakeables[i].positionAmplitudes.sqrMagnitude >= shakeable.positionAmplitudes.sqrMagnitude ? 
                    shakeables[i].positionAmplitudes : shakeable.positionAmplitudes;

                shakeables[i].rotationAmplitudes = shakeables[i].rotationAmplitudes.sqrMagnitude >= shakeable.rotationAmplitudes.sqrMagnitude ?
                    shakeables[i].rotationAmplitudes : shakeable.rotationAmplitudes;
                return;
            }
        }
        shakeable.Init();
        shakeables.Add(shakeable);
    }

    void Update () {
        if (shakeables.Count == 0)
            return;
        for (int i = 0; i < shakeables.Count; ++i)
        {
            if (shakeables[i].transform == null)
            {
                shakeables.RemoveAt(i);
                i--;
                continue;
            }
            else if (shakeables[i].timeLeft < 0.025f)
            {
                shakeables[i].ShakeFinished();
                shakeables.RemoveAt(i);
                i--;
                continue;
            }
            else if (shakeables[i].timeLeft < smoothTime)
            {
                if (shakeables[i].transform.rotation != shakeables[i].rotationLastframe)
                {
                    shakeables[i].originalRotation *= Quaternion.Inverse(shakeables[i].rotationLastframe) * shakeables[i].transform.rotation;
                }
                //rotation
                shakeables[i].transform.rotation = Quaternion.Lerp(shakeables[i].transform.rotation, shakeables[i].originalRotation, 1 - shakeables[i].timeLeft / smoothTime);
                shakeables[i].rotationLastframe = shakeables[i].transform.rotation;

                //position
                Vector3 posDelta = Vector3.Lerp(Vector3.zero, -shakeables[i].positionDelta, 1 - shakeables[i].timeLeft / smoothTime);
                shakeables[i].positionDelta += posDelta;
                shakeables[i].transform.position += posDelta;
            }
            else
            {
                //rotation
                if (shakeables[i].rotationAmplitudes != Vector3.zero)
                {
                    Vector3 rotationAxis = Random.insideUnitSphere;
                    rotationAxis.x *= shakeables[i].rotationAmplitudes.x;
                    rotationAxis.y *= shakeables[i].rotationAmplitudes.y;
                    rotationAxis.z *= shakeables[i].rotationAmplitudes.z;

                    if (shakeables[i].transform.rotation != shakeables[i].rotationLastframe)
                    {
                        shakeables[i].originalRotation *= Quaternion.Inverse(shakeables[i].rotationLastframe) * shakeables[i].transform.rotation;
                    }
                    shakeables[i].transform.rotation *= Quaternion.AngleAxis(Mathf.Lerp(0, rotationAxis.sqrMagnitude, Time.deltaTime * smoothAmount), rotationAxis);

                    shakeables[i].rotationLastframe = shakeables[i].transform.rotation;
                }
                
                //position
                Vector3 positionOffset = Random.insideUnitSphere;
                positionOffset.x *= shakeables[i].positionAmplitudes.x;
                positionOffset.y *= shakeables[i].positionAmplitudes.y;
                positionOffset.z *= shakeables[i].positionAmplitudes.z;

                Vector3 posDelta = Vector3.Lerp(Vector3.zero, positionOffset, Time.deltaTime * smoothAmount);
                shakeables[i].positionDelta += posDelta;
                shakeables[i].transform.position += posDelta;
            }

            shakeables[i].timeLeft -= Time.deltaTime;
        }
    }
    
    public class Shakeable
    {
        public Transform transform;
        
        public Vector3 positionAmplitudes;
        public Vector3 rotationAmplitudes;

        public Vector3 positionDelta;
        public Quaternion originalRotation;
        public Quaternion rotationLastframe;
        public float maxTime;
        public float timeLeft;

        public Shakeable (Transform _transform, Vector3 posAmplitudes, Vector3 rotAmplitudes, float time)
        {
            transform = _transform;
            timeLeft = time;
            maxTime = time;
            positionAmplitudes = posAmplitudes;
            rotationAmplitudes = rotAmplitudes;
        }
        public void Init()
        {
            originalRotation = transform.rotation;
            rotationLastframe = transform.rotation;
        }

        public void ShakeFinished ()
        {
            timeLeft = 0;
            transform.position -= positionDelta;
            transform.rotation = originalRotation;
        }

    }
}
