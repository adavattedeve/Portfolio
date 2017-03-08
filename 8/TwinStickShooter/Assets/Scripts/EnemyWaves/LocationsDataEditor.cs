using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(LocationsDataManager))]
public class LocationsDataEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LocationsDataManager locationsDataManager = (LocationsDataManager)target;
        if (GUILayout.Button("Create new"))
        {
            EditorApplication.delayCall += locationsDataManager.CreateNewLocationsData;
        }
        else if (GUILayout.Button("Save changes"))
        {
            EditorApplication.delayCall += locationsDataManager.SaveChanges;
        }
        else if (GUILayout.Button("Save as new"))
        {
            EditorApplication.delayCall += locationsDataManager.SaveAsNew;
        }
    }

    /*
    public void OnSceneGUI()
    {
        if (Event.current.type == EventType.mouseDown)
        {
            Debug.Log("onscene gui");
            LocationsDataManager locationsDataManager = (LocationsDataManager)target;
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            locationsDataManager.SelectLocationWithRay(ray);
        }
    }
    */

}
