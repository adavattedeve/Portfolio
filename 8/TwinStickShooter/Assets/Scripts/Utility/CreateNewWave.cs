using UnityEditor;

public class CreateNewWave
{
    [MenuItem("Assets/Create/EnemyWave")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<EnemyWave>(ResourcePaths.LEVEL_DATAS_PATH + "New" + ResourcePaths.ENEMY_WAVES, "NewWave");
    }
}
