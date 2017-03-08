using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility
{
    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    public static T CreateAsset<T>(string _path = "", string _name = "") where T : ScriptableObject
    {

        T asset = ScriptableObject.CreateInstance<T>();
        
        string path = _path == "" ? AssetDatabase.GetAssetPath(Selection.activeObject) : _path;
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }
        
        string name = _name;
        if (name == "")
        {
            name = "New " + typeof(T).ToString();
        }
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + name + ".asset");
        /*
        if (!AssetDatabase.IsValidFolder(path)) {
            Directory.CreateDirectory(path);
        }
        */
        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        return asset;
    }
}