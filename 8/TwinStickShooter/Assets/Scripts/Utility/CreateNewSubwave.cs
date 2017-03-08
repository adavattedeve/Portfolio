using UnityEditor;

public class CreateNewSubwave
{
    [MenuItem("Assets/Create/EnemySubwave")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<EnemySubWave>(ResourcePaths.LEVEL_DATAS_PATH + "New" + ResourcePaths.SUBWAVES, "NewSubwave");
    }
}