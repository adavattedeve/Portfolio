using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode]
public class LocationsDataManager : MonoBehaviour
{
    [SerializeField]
    private float lockedYValue = 0;
    [SerializeField]
    private string levelName = "";
    [SerializeField]
    private string locationSetName = "";

    [SerializeField]
    private float locationGizmoSize = 1f;
    [SerializeField]
    private LocationsDataView editable;
    [SerializeField]
    private LocationsData copyFrom;
    [SerializeField]
    private List<LocationsDataView> locationDataViews;

    private Transform locationParent;
    private LocationsData lastEditable;
    private LocationsData lastCopyFrom;

    public void Update ()
    {
        if (locationParent == null)
        {
            if (transform.childCount > 0)
                locationParent = transform.GetChild(0);
            else
            {
                CreateNewLocationsParentGO();
            }
        }

        if (editable.data != null && editable.data != lastEditable)
        {
            LoadLocationData(editable.data);
        }
        if (copyFrom != null && copyFrom != lastCopyFrom)
        {
            CopyLocationsData();
        }
        lastEditable = editable.data;
        lastCopyFrom = copyFrom;

        for (int i = 0; i < locationParent.childCount; ++i)
        {
            Vector3 newPos = locationParent.GetChild(i).transform.position;
            newPos.y = lockedYValue;
            locationParent.GetChild(i).transform.position = newPos;
            DrawGizmoSphere drawGizmo = locationParent.GetChild(i).GetComponent<DrawGizmoSphere>();

            if (drawGizmo == null)
                drawGizmo = locationParent.GetChild(i).gameObject.AddComponent<DrawGizmoSphere>();
            drawGizmo.SetValues(editable.color, locationGizmoSize, Vector3.up);
        }
    }

    public void CreateNewLocationsData ()
    {
        if (EditorApplication.isPlaying)
            return;
        if (!IsLevelNameValid())
            return;
        if (!IsLocationSetNameValid())
            return;

        string path = ResourcePaths.LEVEL_DATAS_PATH + levelName + ResourcePaths.LOCATIONS_PATH;
        LocationsData newLocations = ScriptableObjectUtility.CreateAsset<LocationsData>(path, locationSetName);

        if (editable == null) {
            editable = new LocationsDataView(newLocations);
        } else
        {
            editable.data = newLocations;
        }
    }

    public void SaveChanges()
    {
        editable.data.locations = new Vector3[locationParent.childCount];
        for (int i = 0; i < editable.data.locations.Length; ++i)
        {
            editable.data.locations[i] = locationParent.GetChild(i).position;
        }
        //Debug.Log("There are alredy locations data named: " + locationSetName +".");
        string assetPath = AssetDatabase.GetAssetPath(editable.data.GetInstanceID());
        AssetDatabase.RenameAsset(assetPath, locationSetName);
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(editable.data);
    }

    public void SaveAsNew()
    {
        Transform locationsParentToBeSaved = locationParent;
        locationParent = null;
        CreateNewLocationsData();
        DestroyImmediate(locationParent.gameObject);
        locationParent = locationsParentToBeSaved;
        SaveChanges();
    }

    public void CopyLocationsData() {
        if (copyFrom != null)
        {
            for (int i = 0; i < copyFrom.locations.Length; ++i)
            {
                GameObject newLocation = new GameObject("location" + (locationParent.childCount + i));
                newLocation.transform.position = copyFrom.locations[i];
                newLocation.transform.SetParent(locationParent.transform);
            }
        }
        copyFrom = null;
    }
    /*
    public void SelectLocationWithRay(Ray ray)
    {
        Debug.Log("selecting");
        if (coll == null)
            coll = GetComponent<SphereCollider>();

        for (int i = 0; i < locationParent.childCount; ++i)
        {
            coll.center = locationParent.GetChild(i).transform.position + (Vector3.up * locationGizmoSize);
            coll.radius = locationGizmoSize;
            RaycastHit hit;
            if (coll.Raycast(ray, out hit, 500))
            {
                coll.radius = 0;
                Selection.activeGameObject = locationParent.GetChild(i).gameObject;
                return;
            }
        }
    }
    */
    private void LoadLocationData(LocationsData locations)
    {
        if (locationParent != null)
        {
            DestroyImmediate(locationParent.gameObject);
        }
        CreateNewLocationsParentGO();
        for (int i = 0; i < locations.locations.Length; ++i)
        {
            GameObject newLocation = new GameObject("location" + i);
            newLocation.transform.position = locations.locations[i];
            newLocation.transform.SetParent(locationParent.transform);
        }
        locationSetName = locations.name;
    }

    private bool IsLevelNameValid()
    {
        if (levelName == "" || !AssetDatabase.IsValidFolder(ResourcePaths.LEVEL_DATAS_PATH + levelName))
        {
            Debug.Log("Invalid level name: " + levelName);
            string path = ResourcePaths.LEVEL_DATAS_PATH + levelName;
            if (!AssetDatabase.IsValidFolder(path))
            {
                Debug.Log("No folder with name of " + levelName + " found in path: " + path);
            }
            return false;
        }

        return true;
    }

    private bool IsLocationSetNameValid()
    {
        if (locationSetName == "")
        {
            Debug.Log("Invalid location set name: " + locationSetName);
            return false;
        }
        return true;
    }

    private void CreateNewLocationsParentGO()
    {
        locationParent = new GameObject("LocationsParent").transform;
        locationParent.transform.position = Vector3.zero;
        locationParent.transform.SetParent(transform);
    }

    public void OnDrawGizmos()
    {
        /*
        if (editable.show && locationParent != null)
        {
            Gizmos.color = editable.color;
            for (int j = 0; j < locationParent.childCount; ++j)
            {
                Gizmos.DrawSphere(locationParent.GetChild(j).position + (Vector3.up * locationGizmoSize), locationGizmoSize);
            }
        }
        */
        for (int i = 0; i < locationDataViews.Count; ++i)
        {
            if (!locationDataViews[i].show)
                continue;

            Gizmos.color = locationDataViews[i].color;
            for (int j = 0; j < locationDataViews[i].data.locations.Length; ++j)
            {
                Gizmos.DrawSphere(locationDataViews[i].data.locations[j] + (Vector3.up * locationGizmoSize), locationGizmoSize);
            }
        }
    }


    [System.Serializable]
    private class LocationsDataView
    {

        public bool show = true;
        public Color color = Color.red;
        public LocationsData data;

        public LocationsDataView (LocationsData _data)
        {
            data = _data;
        }
    }
}
