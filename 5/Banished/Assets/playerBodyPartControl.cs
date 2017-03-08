using UnityEngine;
using System.Collections;

public class playerBodyPartControl : MonoBehaviour {
    public float forceMpl;
    public float animationTime;
    public float fadingSpeed = 3f;
    private Rigidbody2D[] bodyParts;
    private Material bodyPartMaterial;

	// Use this for initialization
	void Awake () {
        bodyParts = GetComponentsInChildren<Rigidbody2D>();
        bodyPartMaterial = bodyParts[0].GetComponent<SpriteRenderer>().sharedMaterial;
    }
    void Start()
    {
        gameObject.SetActive(false);
    }
    public void DeathAnimation(Vector3 initialForce)
    {
        gameObject.SetActive(true);
        transform.SetParent(null);
        for (int i = 0; i < bodyParts.Length; ++i)
        {
            bodyParts[i].AddForce(initialForce+ Vector3.right * Random.Range(-1, 1), ForceMode2D.Impulse);
        }
        StartCoroutine(AddConstantForce());

    }

    private IEnumerator AddConstantForce()
    {
        float time = 0;
        Color matColor = bodyPartMaterial.color;
        matColor.a = 1;
        bodyPartMaterial.color = matColor;
        while (time < animationTime)
        {
            
            yield return new WaitForFixedUpdate();
            for (int i = 0; i < bodyParts.Length; ++i)
            {
                bodyParts[i].AddForce(Vector3.up*forceMpl, ForceMode2D.Impulse);
            }
            matColor.a -= Time.deltaTime * fadingSpeed;
            bodyPartMaterial.color = matColor;
            if (matColor.a <= 0)
            { break; }
        }
        for (int i = 0; i < bodyParts.Length; ++i)
        {
            Destroy(bodyParts[i].gameObject);
        }
        GameManager.instance.GameOver();
    }
}
