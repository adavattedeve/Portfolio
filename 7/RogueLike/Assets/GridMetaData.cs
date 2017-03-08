using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class GridMetaData {
    private List<RoomMetaData> rooms;
    public List<RoomMetaData> Rooms { get{ return rooms; } }
    private Vector2 gridSWCorner;
    public Vector2 GridSWCorner {get{ return gridSWCorner; } }
    private TileMetaData[,] grid;
    public TileMetaData[,] Grid { get{ return grid; } }
    public TileMetaData GetMetaTile(int x, int y, bool fromWorldCoords = true)
    {
        if (fromWorldCoords)
        {
            x -= (int)gridSWCorner.x;
            y -= (int)gridSWCorner.y;
        }
        if (CoordsValid(x, y))
            return grid[x, y];
        return null;
    }
    [Header("Just for visualization")]
    [SerializeField]
    private GameObject tile;
    [SerializeField]
    private Color wall;
    [SerializeField]
    private Color floor;
    [SerializeField]
    private Color door;

    private bool visualizationEnabled;

    public void Init(Vector2 boundingBoxMin, Vector2 boundingBoxMax, List<VertexNode> roomsAsNodes, bool _visualizationEnabled)
    {
        visualizationEnabled = _visualizationEnabled;
        gridSWCorner = new Vector2(boundingBoxMin.x, boundingBoxMin.y);
        grid = new TileMetaData[(int)(boundingBoxMax.x - boundingBoxMin.x) + 1, (int)(boundingBoxMax.y - boundingBoxMin.y) + 1];
        for (int y = 0; y < grid.GetLength(1); ++y)
        {
            for (int x = 0; x < grid.GetLength(0); ++x)
            {
                grid[x, y] = new TileMetaData(x + (int)gridSWCorner.x, y + (int)gridSWCorner.y, MetaTileType.UNDEFINED);
            }
        }
        // Make rooms
        rooms = new List<RoomMetaData>();
        for (int i = 0; i < roomsAsNodes.Count; ++i)
        {
            rooms.Add(new RoomMetaData(roomsAsNodes[i], this));
            if (visualizationEnabled)
                roomsAsNodes[i].ParentCell.StopDraw();
        }
        if (visualizationEnabled)
            Draw();
    }

    public void SetUpRoomConnections()
    {
        for (int i = 0; i < rooms.Count; ++i)
        {
            rooms[i].ConstructNeighboursList();
        }
    }
    public void SurroundCorridorsWithWalls()
    {
        // if neighbour tile is floor in order of north, east, south, west
        bool[] isFloor = new bool[8];
        for (int y = 0; y < grid.GetLength(1); ++y)
        {
            for (int x = 0; x < grid.GetLength(0); ++x)
            {
                if (grid[x, y].type != MetaTileType.UNDEFINED)
                {
                    continue;
                }

                int index = 0;
                for (int neighbourY = -1; neighbourY < 2; ++neighbourY)
                {
                    for (int neighbourX = -1; neighbourX < 2; ++neighbourX)
                    {
                        if (neighbourX == 0 && neighbourY == 0)
                            continue;
                        isFloor[index] = CoordsValid(neighbourX + x, neighbourY + y) ? grid[neighbourX + x, neighbourY + y].type == MetaTileType.FLOOR : false;
                        ++index;
                    }
                }

                for (int i = 0; i < isFloor.Length; ++i)
                {
                    if (isFloor[i])
                    {
                        grid[x, y].type = MetaTileType.WALL;
                        break;
                    }
                }
            }
        }
        //TurnLonelyWallsToUnwalkableTiles();
        //TurnLonelyWallsToUnwalkableTiles();
    }

    private void TurnLonelyWallsToUnwalkableTiles()
    {
        for (int y = 0; y < grid.GetLength(1); ++y)
        {
            for (int x = 0; x < grid.GetLength(0); ++x)
            {
                if (grid[x, y].type != MetaTileType.WALL)
                    continue;

                bool[] neighbourNoWall = NeihgbourTilesAre(x, y, MetaTileType.WALL, false);
                bool[] neighbourNotUndefined = NeihgbourTilesAre(x, y, MetaTileType.UNDEFINED, false);

                bool[] neighbourNotWallOrUndefined = new bool[4];
                int floorCount = 0;
                for (int i = 0; i < neighbourNotWallOrUndefined.Length; ++i)
                {
                    neighbourNotWallOrUndefined[i] = neighbourNoWall[i] && neighbourNotUndefined[i];
                    if (neighbourNotWallOrUndefined[i])
                        floorCount++;
                }
                if (floorCount > 2 || (neighbourNotWallOrUndefined[0] && neighbourNotWallOrUndefined[2]) || (neighbourNotWallOrUndefined[1] && neighbourNotWallOrUndefined[3]))
                {
                    grid[x, y].type = MetaTileType.UNWALKABLE;
                }
            }
        }
    }

    public void Reset()
    {
        StopDraw();
    }
    private bool CoordsValid(int _x, int _y)
    {
        return _x >= 0 && _y >= 0 && _x < grid.GetLength(0) && _y < grid.GetLength(1);
    }

    //returns array of bools , true if same as given type in order of n e s w
    public bool[] NeihgbourTilesAre(int x, int y, MetaTileType type, bool tileIs=true)
    {
        bool[] neighbourIsType = new bool[4];
        int newX, newY;

        newX = x;
        newY = y + 1;
        neighbourIsType[0] =
            (newX >= 0 && newX < grid.GetLength(0) &&
             newY >= 0 && newY < grid.GetLength(1) &&
             grid[newX, newY].type == type);

        newX = x + 1;
        newY = y;
        neighbourIsType[1] =
           (newX >= 0 && newX < grid.GetLength(0) &&
             newY >= 0 && newY < grid.GetLength(1) &&
             grid[newX, newY].type == type);

        newX = x;
        newY = y - 1;
        neighbourIsType[2] =
            (newX >= 0 && newX < grid.GetLength(0) &&
             newY >= 0 && newY < grid.GetLength(1) &&
             grid[newX, newY].type == type);

        newX = x - 1;
        newY = y;
        neighbourIsType[3] =
            (newX >= 0 && newX < grid.GetLength(0) &&
             newY >= 0 && newY < grid.GetLength(1) &&
             grid[newX, newY].type == type);
        if (!tileIs)
        {
            for (int i = 0; i < neighbourIsType.Length; ++i)
            {
                neighbourIsType[i] = !neighbourIsType[i];
            }
        }
        return neighbourIsType;
    }

    //returns array of bools , true if same as given type in order of ne se sw nw
    public bool[] CornerNeihgbourTilesAre(int x, int y, MetaTileType type, bool tileIs = true)
    {
        bool[] neighbourIsType = new bool[4];
        int newX, newY;

        newX = x + 1;
        newY = y + 1;
        neighbourIsType[0] =
            (newX >= 0 && newX < grid.GetLength(0) &&
             newY >= 0 && newY < grid.GetLength(1) &&
             grid[newX, newY].type == type);

        newX = x + 1;
        newY = y - 1;
        neighbourIsType[1] =
           (newX >= 0 && newX < grid.GetLength(0) &&
             newY >= 0 && newY < grid.GetLength(1) &&
             grid[newX, newY].type == type);

        newX = x - 1;
        newY = y - 1;
        neighbourIsType[2] =
            (newX >= 0 && newX < grid.GetLength(0) &&
             newY >= 0 && newY < grid.GetLength(1) &&
             grid[newX, newY].type == type);

        newX = x - 1;
        newY = y + 1;
        neighbourIsType[3] =
            (newX >= 0 && newX < grid.GetLength(0) &&
             newY >= 0 && newY < grid.GetLength(1) &&
             grid[newX, newY].type == type);
        if (!tileIs)
        {
            for (int i = 0; i < neighbourIsType.Length; ++i)
            {
                neighbourIsType[i] = !neighbourIsType[i];
            }
        }
        return neighbourIsType;
    }

    public void Draw()
    {
        for (int y = 0; y < grid.GetLength(1); ++y)
        {
            for (int x = 0; x < grid.GetLength(0); ++x)
            {
                
                Color color = new Color();
                switch (grid[x, y].type)
                {
                    case MetaTileType.WALL:
                        color = wall;
                        break;
                    case MetaTileType.FLOOR:
                        if (grid[x, y].partOf != null && grid[x, y].partOf.Type == RoomType.STARTINGROOM)
                        {
                            color = new Color(0.6f, 0, 0);
                        }
                        else if (grid[x, y].partOf != null && grid[x, y].partOf.Type == RoomType.ENDINGROOM)
                        {
                            color = new Color(0, 0, 0.6f);
                        }
                        else
                        {
                            color = floor;
                        }                       
                        break;
                    case MetaTileType.DOOR:
                        color = door;
                        break;
                    case MetaTileType.UNDEFINED:
                        continue;
                }
                
                grid[x, y].Draw(color, tile);
            }
        }

    }
    private void StopDraw()
    {
        if (grid==null)
            return;
        for (int y = 0; y < grid.GetLength(1); ++y)
        {
            for (int x = 0; x < grid.GetLength(0); ++x)
            {

                grid[x, y].StopDraw();
            }
        }
    }

    public static bool IsNeighbours(TileMetaData tile1, TileMetaData tile2)
    {
        if (tile1.X == tile2.X)
        {
            return tile1.Y + 1 == tile2.Y || tile1.Y - 1 == tile2.Y;
        }
        else if (tile1.Y == tile2.Y)
        {
            return tile1.X + 1 == tile2.X || tile1.X - 1 == tile2.X;

        }
        return false;
    }
}
