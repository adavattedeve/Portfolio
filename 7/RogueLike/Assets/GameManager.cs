using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    private MapGenerator mapGenerator;
    private DataBase db;
    private FogOfWar fow;
    public  FogOfWar Fow { get{ return fow; } }
    private static Grid grid;
    public static Grid Grid {
        get { return grid; } }

    private static GameLogic gameLogic;
    public static GameLogic GameLogic{
        get { return gameLogic; } }

    public GameObject playerPrefab;
    public static Player player;

    //PixelPerUnit
    public static int PPU = 64;
    public static float PixelSize { get { return 1f / 64; } }

    public static float SnapToPixel(float _x) {
        return Mathf.Floor(0.99f + (_x) / PixelSize) * PixelSize;
    }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            db = GetComponent<DataBase>();
            fow = GetComponent<FogOfWar>();
        }        
    }

	void Start ()
    {
        StartGame();
	}

    public void StartGame()
    {
        //FadeBlack with call back of LoadScene
        SceneManager.LoadScene(1);
    }

    public void OnLevelWasLoaded(int level)
    {
        Debug.Log("level loaded");
        //Fade from black
        if (level == 1)
        {
            mapGenerator = (Instantiate(db.GetMapGeneratorPrefab()) as GameObject).GetComponent<MapGenerator>();
            mapGenerator.GenerateMap(StartLevel);
        }
        
    }

    public void StartLevel(Grid _grid)
    {
        grid = _grid;
        fow.InitFogOfWar(grid);
        gameLogic = GetComponent<GameLogic>();
        player = new Player(grid.levelStart, Instantiate(playerPrefab) as GameObject);
        grid.levelStart.AddEntity(player);
        fow.UpdateFow();
        //Init pathfinding system
    }
}
