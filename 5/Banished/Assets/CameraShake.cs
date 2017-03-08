using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
    public static CameraShake instance;
    [Header("Screenshake params")]
    public float amplitude = 1f;
    public float duration = 0.2f;
    [Header("TranslateToDefault params")]
    public float speed = 0.5f;
    private Vector3 defaultPos;

    void Awake()
    {
        defaultPos = transform.position;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
    public void ScreenShake()
    {     
        StopAllCoroutines();
        transform.position = defaultPos;
        StartCoroutine(ShakeScreen());
    }

    private IEnumerator ShakeScreen()
    {
        Vector3 newPos;
        float time = 0;
        while (time <duration)
        {
            newPos = transform.position;
            newPos += new Vector3(Random.Range(-amplitude, amplitude)*Time.deltaTime, Random.Range(-amplitude, amplitude) * Time.deltaTime, 0);
            transform.position = newPos;
            yield return new WaitForEndOfFrame();

            time += Time.deltaTime;
        }
        StartCoroutine(TranslateToDefault());
    }
    private IEnumerator TranslateToDefault()
    {
        Vector3 direction = defaultPos - transform.position;
        while (Vector3.Distance(defaultPos, transform.position) > 0.05f)
        {
            direction = defaultPos - transform.position;
            yield return new WaitForEndOfFrame();
            transform.Translate(direction* Time.deltaTime*speed);
        }
    }
}
