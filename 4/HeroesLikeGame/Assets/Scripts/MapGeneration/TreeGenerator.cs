using UnityEngine;
using System.Collections;



[System.Serializable]
public class TreeGenerator {
    [System.Serializable]
    public class TreeTemplate
    {
        public IntVector2 size;
        public GameObject prefab;
    }

    public TreeTemplate[] treeTemplates;
    public GameObject GetTree(IntVector2 size)
    {
        //Generate new tree SomeHow
        for (int i = 0; i < treeTemplates.Length; ++i)
        {
            if (size == treeTemplates[i].size)
            {
                return MonoBehaviour.Instantiate(treeTemplates[i].prefab) as GameObject;
            }
        }
        Debug.Log("Couldn't build tree with size: x " + size.x + " y " + size.y);
        return null;
    }
}
