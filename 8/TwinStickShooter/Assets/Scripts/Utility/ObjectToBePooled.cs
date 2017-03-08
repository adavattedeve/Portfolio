using UnityEngine;

[System.Serializable]
public class ObjectToBePooled
{
    [Header("Fill either prefab or path")]
    public GameObject prefab;
    public string path = "Prefabs/";
    public int buffer = 10;

    public bool Init()
    {
        if (prefab == null)
        {
            prefab = Resources.Load<GameObject>(path);
        }

        if (prefab == null)
        {
            Debug.Log("Couldn't find prefab from path: " + path);
            return false;
        }
        return true;
    }


}