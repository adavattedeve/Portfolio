using UnityEngine;
using System.Collections.Generic;


public class MapGenerator : MonoBehaviour {
    public static MapGenerator instance;

    [SerializeField]
    private int startingCells = 150;
    [SerializeField]
    private Vector2 roomAmount = new Vector2(7, 18);

    [Header("Min and Max values")]
    [SerializeField]
    private Vector2 cellSizeX = new Vector2(3, 6);
    [SerializeField]
    private Vector2 cellSizeY = new Vector2(3, 6);
    [SerializeField]
    private Vector2 roomSizeX = new Vector2(6, 10);
    [SerializeField]
    private Vector2 roomSizeY = new Vector2(6, 10);

    [SerializeField]
    private int minDistBetweenStartEndRooms = 3;

    [SerializeField]
    private Vector2 cellColorRanges = new Vector2(0.1f, 0.25f);

    [SerializeField]
    private int cellPosX = 20;
    [SerializeField]
    private int cellPosY = 20;

    //[Header("Runs without any visualization as fast as possible")]
    //[SerializeField]
    private bool optimizedGeneration = false;
    [SerializeField]
    private bool doStep = false;
    //controls if the algorithum should animate the process step by step
    [SerializeField]
    private bool animate = false;
    //time between steps
    [SerializeField]
    private float animateTime = 0.5f;
    private float animateTimer = 0;

    //Controls if map generation should continue to next step
    private bool canContinue;

    [SerializeField]
    private DelunayTriangulation delTriangulation;
    [SerializeField]
    private MinimalSpanningTree minimalSpanningTree;
    [SerializeField]
    private GridMetaData gridMetaData;
    [SerializeField]
    private CorridorDigger corridorDigger;
    private StartEndRoomDecider startEndRoomDecider;


    private enum GenerationState {INACTIVE, CELLSGENERATED, CELLSSTILL, DELUNAYTRINGULATION, MINIMALSPANNINGTREE, FINALPATHS, TILEMETADATA, STARTENDROOMDECIDER, BUILDINGGRID, ADDINGCONTENT}
    private GenerationState stage;
    private bool firstTime = true; //first time with current state in update loop

    private Vector2 mapSizeX;
    private Vector2 mapSizeY;

    private List<Cell> allCells; //all spawned nodes
    private List<VertexNode> rooms; //all nodes which are chosen to be rooms
    private List<Edge> dtEdges; //Delunay triangulation final edges

    private Vector2 mapBoundingBoxMin;
    private Vector2 mapBoundingBoxMax;

    [Header("Final Paths")]
    [SerializeField]
    [Range(0f, 1f)]private float percentOfDTPaths = 0.2f;
    [SerializeField]
    private Color finalPathColor;

    private List<Edge> finalPaths;

    private System.Action<Grid> callback;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            allCells = new List<Cell>();
            rooms = new List<VertexNode>();
            dtEdges = new List<Edge>();
            finalPaths = new List<Edge>();
            instance = this;
            stage = GenerationState.INACTIVE;
            canContinue = true;
            startEndRoomDecider = new StartEndRoomDecider();
        }
        
    }
    void Update()
    {
        switch (stage)
        {
            case GenerationState.INACTIVE:
                break;
            case GenerationState.CELLSGENERATED:
                if (firstTime)
                {
                    if (!canContinue)
                        break;

                    InitializeCells();
                    firstTime = false;
                }
                else if (CellsStill())
                {
                    canContinue = !animate && !doStep;
                    stage = GenerationState.CELLSSTILL;
                    break;
                }
                for (int i = 0; i < allCells.Count; ++i)
                {
                    allCells[i].Update();
                }
                break;
            case GenerationState.CELLSSTILL:
                if (canContinue)
                {
                    canContinue = !animate && !doStep;
                    DetermineRooms();
                    CalculateMapBoundingBox(allCells);
                    firstTime = true;
                    stage = GenerationState.DELUNAYTRINGULATION;
                    break;
                }
                break;
            case GenerationState.DELUNAYTRINGULATION:
                if (firstTime)
                {
                    if (!canContinue)
                        break;
                    delTriangulation.Init(rooms, mapBoundingBoxMin, mapBoundingBoxMax, !optimizedGeneration);
                    firstTime = false;
                }
                if (delTriangulation.IsDone)
                {
                    canContinue = !animate && !doStep;
                    dtEdges = delTriangulation.FinalTriangulation;
                    stage = GenerationState.MINIMALSPANNINGTREE;
                    firstTime = true;
                    if (!optimizedGeneration)
                        Edge.DrawEdges(dtEdges);
                    break;
                }
                delTriangulation.Update();
                
                break;
            case GenerationState.MINIMALSPANNINGTREE:
                if (firstTime)
                {
                    if (!canContinue)
                        break;
                    canContinue = !animate && !doStep;
                    minimalSpanningTree.Init(rooms, dtEdges, !optimizedGeneration);
                    if (!optimizedGeneration)
                        Edge.StopDrawEdges(dtEdges);
                    firstTime = false;
                }
                else if (minimalSpanningTree.IsDone)
                {
                    stage = GenerationState.FINALPATHS;
                    firstTime = true;
                    if (!optimizedGeneration)
                        Edge.DrawEdges(minimalSpanningTree.FinalEdgeSet, minimalSpanningTree.LineColor);
                    break;
                }
                minimalSpanningTree.Update();
                break;
            case GenerationState.FINALPATHS:
                if (firstTime)
                {
                    if (!canContinue)
                        break;
                    canContinue = !animate && !doStep;
                    DetermineFinalPaths();
                    if (!optimizedGeneration)
                        Edge.DrawEdges(finalPaths, finalPathColor);                                  
                    stage = GenerationState.TILEMETADATA;
                }
                break;
            case GenerationState.TILEMETADATA:
                if (firstTime)
                {
                    if (!canContinue)
                        break;
                    if (!optimizedGeneration)
                        Edge.StopDrawEdges(finalPaths);
                    canContinue = !animate && !doStep;
                    firstTime = false;

                    List<Cell> roomsAsCells = new List<Cell>();
                    for (int i = 0; i < rooms.Count; ++i)
                    {
                        roomsAsCells.Add(rooms[i].ParentCell);
                    }
                    CalculateMapBoundingBox(roomsAsCells);
                    gridMetaData.Init(mapBoundingBoxMin, mapBoundingBoxMax, rooms, !optimizedGeneration);
                    corridorDigger.Init(gridMetaData, finalPaths, allCells, !optimizedGeneration);                   
                }
                else if(corridorDigger.IsDone)
                {
                    gridMetaData.SetUpRoomConnections();
                    firstTime = true;
                    stage = GenerationState.STARTENDROOMDECIDER;
                    break;
                }
                corridorDigger.Update();
                if (!optimizedGeneration)
                    gridMetaData.Draw();
                break;
            case GenerationState.STARTENDROOMDECIDER:
                if (firstTime)
                {
                    if (!canContinue)
                        break;

                    if (!optimizedGeneration)
                    {
                        for (int i = 0; i < allCells.Count; ++i)
                        {
                            allCells[i].StopDraw();
                        }
                    }
                    allCells.Clear();
                   
                    firstTime = false;
                    int minimumDistance = minDistBetweenStartEndRooms;
                    while (!startEndRoomDecider.DetermineStartAndEndRooms(gridMetaData.Rooms, minimumDistance))
                    {
                        if (minimumDistance == 1)
                        {
                            Debug.Log("RegenerateMap");
                            GenerateMap();
                            return;
                        }
                        minimumDistance--;
                    }
                    if (!optimizedGeneration)
                    {
                        gridMetaData.Draw();
                        Edge.DrawEdges(finalPaths);
                    }
                        
                    stage = GenerationState.BUILDINGGRID;
                    firstTime = true;
                }
                break;
            case GenerationState.BUILDINGGRID:
                if (firstTime)
                {
                    if (!canContinue)
                        break;

                    firstTime = false;

                    Grid grid = new Grid();
                    grid.CreateMapFromMetaData(gridMetaData);

                    //TEMPORARY
                    if (callback != null)
                        callback(grid);
                }                
                break;
        }

        canContinue = (!animate && !doStep) || optimizedGeneration;
        if (animate && !canContinue)
        {
            animateTimer += Time.deltaTime;
            if (animateTimer > animateTime)
            {
                canContinue = true;
                animateTimer = 0;
            }
        }
        if (doStep && Input.GetMouseButtonDown(0))
        {
            canContinue = true;
        }

    }

    public void GenerateMap()
    {
        Reset();
        GenerateCells();
        stage = GenerationState.CELLSGENERATED;
        firstTime = true;
        canContinue = !animate && !doStep;
    }

    public void GenerateMap(System.Action<Grid> OnMapGenerated)
    {
        Reset();
        GenerateCells();
        stage = GenerationState.CELLSGENERATED;
        firstTime = true;
        canContinue = !animate && !doStep;
        callback = OnMapGenerated;
    }
    //Generates random sized nodes at random positions. Size is weighted towards smaller size.
    private void GenerateCells()
    {
        int roomsLeft = (int)Random.Range(roomAmount.x, roomAmount.y);
        if (startingCells < roomsLeft)
        {
            startingCells = roomsLeft;
        }
        float roomSpawnRate = (float)roomsLeft / startingCells;
        bool spawnRoom = false;
        for (int i = 0; i < startingCells; i++)
        {
           
                        
            spawnRoom = Random.Range(0f, 1f) < Mathf.Max(roomsLeft / (startingCells - i), roomSpawnRate);
            if (roomsLeft == 0)
            {
                spawnRoom = false;
            }

            int xSize=0, ySize = 0;
            if (spawnRoom)
            {
                xSize = (int)Random.Range(roomSizeX.x, roomSizeX.y);
                ySize = (int)Random.Range(roomSizeY.x, roomSizeY.y);
                roomsLeft--;
            }
            else
            {
                xSize = (int)Random.Range(cellSizeX.x, cellSizeX.y);
                ySize = (int)Random.Range(cellSizeY.x, cellSizeY.y);
            }
            

            if (xSize % 2 == 0) { xSize += 1; }
            if (ySize % 2 == 0) { ySize += 1; }

            Cell cell = new Cell(GetRandomCellPosition(), new Vector2(xSize, ySize));

            allCells.Add(cell);
        }       
    }
    private void InitializeCells()
    {
        for (int i = 0; i < allCells.Count; i++)
        {
            if (optimizedGeneration)
            {
                allCells[i].Init(allCells);
            }
            else
            {
                allCells[i].Init(allCells);
                allCells[i].Draw();
                allCells[i].SetColor(new Color(Random.Range(cellColorRanges.x, cellColorRanges.y),
                                               Random.Range(cellColorRanges.x, cellColorRanges.y),
                                               Random.Range(cellColorRanges.x, cellColorRanges.y)));
            }
            
        }
    }
    //returns true if all nodes are still
    private bool CellsStill()
    {
        for (int i = 0; i < allCells.Count; ++i)
        {
            if (allCells[i].HasStopped == false)
            {
                return false;
            }
        }
        return true;
    }
    //Calculates map's boundingBox
    private void CalculateMapBoundingBox(List<Cell> cellsInBox)
    {
        mapBoundingBoxMax = new Vector2(-Mathf.Infinity, -Mathf.Infinity);
        mapBoundingBoxMin = new Vector2(Mathf.Infinity, Mathf.Infinity);
        Vector2 objectSize = Vector2.zero;
        for (int i = 0; i< cellsInBox.Count; ++i)
        {
            objectSize.x = Mathf.Floor(cellsInBox[i].Size.x / 2);
            objectSize.y = Mathf.Floor(cellsInBox[i].Size.y / 2);

            //Min boundaries
            if (cellsInBox[i].Position.x - objectSize.x < mapBoundingBoxMin.x)
            {
                mapBoundingBoxMin.x = cellsInBox[i].Position.x - objectSize.x;
            }
            if (cellsInBox[i].Position.y - objectSize.y < mapBoundingBoxMin.y)
            {
                mapBoundingBoxMin.y = cellsInBox[i].Position.y - objectSize.y;
            }

            //Max boundaries
            if (cellsInBox[i].Position.x + objectSize.x>mapBoundingBoxMax.x)
            {
                mapBoundingBoxMax.x = cellsInBox[i].Position.x + objectSize.x;
            }
            if (cellsInBox[i].Position.y + objectSize.y > mapBoundingBoxMax.y)
            {
                mapBoundingBoxMax.y = cellsInBox[i].Position.y + objectSize.y;
            }            
        }

    }
    //Determine Rooms by cell sizes
    private void DetermineRooms()
    {
        for (int i = 0; i < allCells.Count; ++i)
        {
            if (allCells[i].Size.x >= roomSizeX.x || allCells[i].Size.y >= roomSizeY.x)
            {
                rooms.Add(new VertexNode(allCells[i].Position.x, allCells[i].Position.y, allCells[i]));
                if (!optimizedGeneration)
                {
                    allCells[i].SetColor( new Color(0.8f, 0.8f, 0.8f));
                }                
            }
        }
    }
    //Generates 1x1 node to all empty spaces.
    private void DetermineFinalPaths()
    {
        
        List<Edge> spanningTree = minimalSpanningTree.FinalEdgeSet;
        //Add all of the minimal spanning tree paths to final paths
        for (int i = 0; i < spanningTree.Count; ++i)
        {
            finalPaths.Add(spanningTree[i]);
        }
        List<Edge> additionalPaths = delTriangulation.FinalTriangulation;
        //Determine additional paths by removing all minimal spanning tree paths from delunay tringulation paths
        for (int i = 0; i < additionalPaths.Count; ++i)
        {
            if (finalPaths.Contains(additionalPaths[i]))
            {
                additionalPaths.RemoveAt(i);
                --i;
            }
        }

        //Choose random additional paths to the final paths
        int additionalPathAmount = Mathf.FloorToInt(additionalPaths.Count * percentOfDTPaths+0.99f);
        for (int i = 0; i < additionalPathAmount; ++i)
        {
            int random = Random.Range(0, additionalPaths.Count);
            finalPaths.Add(additionalPaths[random]);
            additionalPaths.RemoveAt(random);
        }
        
    }
    //Destroy old Game objects and resets values
    public void Reset()
    {
        Edge.StopDrawEdges(dtEdges);
        Edge.StopDrawEdges(minimalSpanningTree.FinalEdgeSet);
        Edge.StopDrawEdges(finalPaths);
        for (int i = 0; i < allCells.Count; ++i)
        {
            allCells[i].StopDraw();
        }
        allCells.Clear();
        rooms.Clear();
        dtEdges.Clear();
        finalPaths.Clear();
        stage = GenerationState.INACTIVE;
        delTriangulation.Reset();
        minimalSpanningTree.Reset();
        gridMetaData.Reset();
    }
    public Vector2 GetRandomCellPosition()
    {
        int xPos = Random.Range(-cellPosX, cellPosX);
        int yPos = Random.Range(-cellPosY, cellPosY);

        return new Vector3(xPos, yPos);
    }

}
