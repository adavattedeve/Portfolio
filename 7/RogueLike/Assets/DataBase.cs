using UnityEngine;
using System.Collections;

public class DataBase : MonoBehaviour {
    public static DataBase instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            levelGraphics.Init();
        }
    }

    [SerializeField]
    private GameObject mapGeneratorPrefab;

    [SerializeField]
    private LevelGraphics levelGraphics;

    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    private Sprite wallSprite;

    [SerializeField]
    private Sprite floorSprite;

    public GameObject GetMapGeneratorPrefab()
    {
        return mapGeneratorPrefab;
    }

    public LevelGraphics GetLevelGraphics()
    {
        return levelGraphics;
    }

    //neighbourIsFloor array contains true if neighbour is floor in order of north, east, south, west
    public Sprite GetWallTileSprite(bool[] neighbourIsFloor)
    {
        return wallSprite;
    }
    public Sprite GetFloorTileSprite()
    {
        return floorSprite;
    }
}
