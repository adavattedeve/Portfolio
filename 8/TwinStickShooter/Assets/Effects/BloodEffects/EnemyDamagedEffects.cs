using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyDamagedEffects : MonoBehaviour {

    private static bool requiredObjectsInPool = false;

    [SerializeField]
    private int bodyPartCount = 8;
    [SerializeField]
    private Vector2 forceRange = new Vector2(4, 8);
    [SerializeField]
    private float spawnBoxSize = 0.7f;
    [SerializeField]
    private Vector3 forceDirOffset = new Vector3(0.4f, 1f, 0.4f);
    [SerializeField]
    private ObjectToBePooled bodypartPoolData;
    [SerializeField]
    private ObjectToBePooled deathEffectPoolData;
    [SerializeField]
    private ObjectToBePooled damagedEffectPoolData;

    private Health health;
    private Vector3 lastDamageFromDirection = Vector3.zero;

	void Awake () {
        health = GetComponent<Health>();
        health.OnTakeDamageEvent += TakeDamage;
        health.OnDeathEvent += Death;
    }

    void Start () {
        if (!requiredObjectsInPool)
            AddRequiredObjectsToPool();
    }
	
    private void AddRequiredObjectsToPool() {
        ObjectPool.instance.AddNewObjectToBePooled(deathEffectPoolData);
        ObjectPool.instance.AddNewObjectToBePooled(damagedEffectPoolData);
        ObjectPool.instance.AddNewObjectToBePooled(bodypartPoolData);
        requiredObjectsInPool = true;
    }

    public void TakeDamage(DamageInfo info) {
        BloodSplatCreator.CreateBloodSplats(transform.position, BloodSplatCreator.SplatSize.SMALL);
        ObjectPool.instance.GetObjectFromPool(damagedEffectPoolData, transform.position + new Vector3(0, 1.5f, 0), Quaternion.LookRotation(-info.fromDirection, Vector3.up));
        ObjectShaker.AddShake(transform, new Vector3(1, 1, 1), new Vector3(10, 0, 10), 0.35f);
        lastDamageFromDirection = info.fromDirection;

    }

    public void Death() {
        SlingBodyParts();
        BloodSplatCreator.CreateBloodSplats(transform.position, BloodSplatCreator.SplatSize.LARGE);
        ObjectShaker.AddShake(Camera.main.transform, new Vector3(1, 1, 1), new Vector3(1, 1, 1), 0.4f);
        ObjectPool.instance.GetObjectFromPool(deathEffectPoolData, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
    }

    private void SlingBodyParts() {
        Vector3 pos = transform.position;
        pos.y = 1.5f;
        for (int i = 0; i < bodyPartCount; ++i) {
            Rigidbody bodyPart = (ObjectPool.instance.GetObjectFromPool(bodypartPoolData,
                pos + new Vector3(Random.Range(-spawnBoxSize, spawnBoxSize), Random.Range(-spawnBoxSize, spawnBoxSize), Random.Range(-spawnBoxSize, spawnBoxSize)),
                transform.rotation)).GetComponent<Rigidbody>();

            float force = Random.Range(forceRange.x, forceRange.y);

            Vector3 forceDir = lastDamageFromDirection;
            forceDir.x += Random.Range(-forceDirOffset.x, forceDirOffset.x);
            forceDir.y += Random.Range(0, forceDirOffset.y);
            forceDir.z += Random.Range(-forceDirOffset.z, forceDirOffset.z);
            
            bodyPart.AddForce(forceDir * force, ForceMode.Impulse);
            bodyPart.AddTorque(forceDir);
        }
    }

}
