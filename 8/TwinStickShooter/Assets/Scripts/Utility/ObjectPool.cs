using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
	
	public static ObjectPool instance;
	
	/// <summary>
	/// Different objects in the pool
	/// </summary>
    [SerializeField]
    private List<ObjectToBePooled> objectsToBePooled;

    /// <summary>
    /// The pooled objects currently available.
    /// </summary>
    private List<List<GameObject>> pooledObjects;
	
	public int defaultBufferAmount = 3;
	
	/// <summary>
	/// The container object that we will keep unused pooled objects so we dont clog up the editor with objects.
	/// </summary>
	protected GameObject containerObject;
	
	void Awake ()
	{
        if (instance != null) {
            return;
        }
		instance = this;

        containerObject = new GameObject("PooledObjects");
        pooledObjects = new List<List<GameObject>>();

        for (int i = 0; i < objectsToBePooled.Count; ++i)
        {
            if (!objectsToBePooled[i].Init())
            {
                continue;
            }

            pooledObjects.Add(new List<GameObject>());
            InstantiateObjectsToPool(objectsToBePooled[i]);
        }
    }
	
    /// <summary>
    /// Adds new object in the pool.
    /// </summary>
    /// <param name="prefab"> object's prefab </param>
    /// <param name="path"> path in resources folder </param>
    /// <param name="buffer"> how many objects should be instantiated in initializing method </param>
    public void AddNewObjectToBePooled(GameObject prefab = null, string path = "", int buffer = 10) {
        ObjectToBePooled newObject = new ObjectToBePooled();
        newObject.path = path;
        newObject.prefab = prefab;
        newObject.buffer = buffer;
        AddNewObjectToBePooled(newObject);
    }

    /// <summary>
    /// Adds new object in the pool.
    /// </summary>
    /// <param name="newObject"> new object's pool data </param>
    /// <returns> returns true if succesful </returns>
    public bool AddNewObjectToBePooled(ObjectToBePooled newObject)
    {
        if (IsObjectAlreadyInPool(newObject))
            return false;
        if (!newObject.Init()) {
            return false;
        }
        objectsToBePooled.Add(newObject);
        pooledObjects.Add(new List<GameObject>());
        InstantiateObjectsToPool(newObject);

        return true;
    }

    /// <summary>
    /// Gets object from the object pool.  If this kind of object doesn't exist in the pool, we will try to add it in the pool.
    /// If succesful -> return object and else return null.
    /// </summary>
    /// /// <param name='objectToBePooled'> Objects pool data. </param>
    /// <returns> The object that corresponds the data in objectToBePooled param. </returns>
    public GameObject GetObjectFromPool (ObjectToBePooled objectToBePooled)
	{
        bool searchByName = objectToBePooled.prefab != null;
 
		for(int i=0; i< objectsToBePooled.Count; i++)
		{
			GameObject prefab = objectsToBePooled[i].prefab;
			if((searchByName && prefab.name == objectToBePooled.prefab.name) || 
                (!searchByName && objectsToBePooled[i].path == objectToBePooled.path))
			{
                return GetObjectFromIndex(i);
			}
		}

        if (AddNewObjectToBePooled(objectToBePooled)) {
            return GetObjectFromIndex(pooledObjects.Count - 1);
        }

        Debug.Log("Object not found from pool. prefab is null: " + objectToBePooled.prefab == null + " path is: " + objectToBePooled.path);
        return null;
	}
    /// <summary>
    /// Gets object from the object pool in wanted position and rotation.  If this kind of object doesn't exist in the pool, we will try to add it in the pool.
    /// If succesful -> return object and else return null.
    /// </summary>
    /// <param name="objectToBePooled"> Objects pool data. </param>
    /// <param name="pos"> objects position </param>
    /// <param name="rot"> objects rotation </param>
    /// <returns> The object that corresponds the data in objectToBePooled param with position and rotation given as parameters. </returns>
    public GameObject GetObjectFromPool(ObjectToBePooled objectToBePooled, Vector3 pos, Quaternion rot)
    {
        GameObject go = GetObjectFromPool(objectToBePooled);
        if (go != null) {
            go.transform.position = pos;
            go.transform.rotation = rot;
        }
        return go;
    }

    /// <summary>
    /// Gets object from the object pool by name. If object was not found, return null
    /// </summary>
    /// <param name="objectName"> object's name </param>
    /// <returns> If object was not found, return null </returns>
    public GameObject GetObjectFromPool(string objectName)
    {

        for (int i = 0; i < objectsToBePooled.Count; i++)
        {
            GameObject prefab = objectsToBePooled[i].prefab;
            if ( prefab.name == objectName)
            {
                return GetObjectFromIndex(i);
            }
        }
        Debug.Log("Object not found from pool. Objectname: " + objectName);
        return null;
    }

    /// <summary>
    /// Pools object that is given as parameter. Won't be pooled if this kind of object doent's exists in the pool
    /// </summary>
    /// <param name='obj'> Object to be pooled. </param>
    public void PoolObject ( GameObject obj )
	{
		for ( int i=0; i<objectsToBePooled.Count; i++)
		{
			if(objectsToBePooled[i].prefab.name == obj.name)
			{
				obj.SetActive(false);
				obj.transform.parent = containerObject.transform;
				pooledObjects[i].Add(obj);
				return;
			}
		}
	}

    /// <summary>
    /// Checks if there is object pooled that corresponds the object's pool data that is given as parameter
    /// </summary>
    /// <param name="objectToBePooled"> object's pool data </param>
    /// <returns></returns>
    public bool IsObjectAlreadyInPool(ObjectToBePooled objectToBePooled)
    {
        bool searchByName = objectToBePooled.prefab != null;
        for (int i = 0; i < objectsToBePooled.Count; i++)
        {
            GameObject prefab = objectsToBePooled[i].prefab;
            if ((searchByName && prefab.name == objectToBePooled.prefab.name) ||
                (!searchByName && objectsToBePooled[i].path == objectToBePooled.path))
            {
                return true;
            }
        }
        return false;
    }

    private void InstantiateObjectsToPool(ObjectToBePooled objectToBePooled) {
        int buffer = objectToBePooled.buffer > 0 ? objectToBePooled.buffer : defaultBufferAmount;
        for (int i = 0; i < buffer; ++i)
        {
            GameObject newObj = Instantiate(objectToBePooled.prefab) as GameObject;
            newObj.name = objectToBePooled.prefab.name;
            PoolObject(newObj);
        }
    }

    private GameObject GetObjectFromIndex(int index) {
        if (pooledObjects[index].Count > 0)
        {
            GameObject pooledObject = pooledObjects[index][0];
            pooledObjects[index].RemoveAt(0);
            pooledObject.transform.parent = null;
            pooledObject.SetActive(true);

            return pooledObject;

        }
       GameObject newGO = Instantiate(objectsToBePooled[index].prefab) as GameObject;
       newGO.name = objectsToBePooled[index].prefab.name;
       return newGO;
    }
}