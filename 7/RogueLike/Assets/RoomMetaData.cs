using UnityEngine;
using System.Collections.Generic;
public enum RoomType { UNDEFINED, STARTINGROOM, ENDINGROOM }
public class RoomMetaData  {
   
    //x and y are in world coordinates
    private int x;
    public int X { get{ return x; } }
    private int y;
    public int Y { get { return y; } }
    private RoomType type;
    public RoomType Type { get { return type; } set { type = value; } }
    private TileMetaData[,] tiles;
    private List<RoomMetaData> neighbours;
    public List<RoomMetaData> Neighbours { get{ return neighbours; } }

    public int HalfWidth { get{return Mathf.FloorToInt(tiles.GetLength(0)/2); } }
    public int HalfHeight { get { return Mathf.FloorToInt(tiles.GetLength(1) / 2); } }

    private GridMetaData grid;
    // public TileMetaData[,] Tiles { get{ return tiles; } }
    public TileMetaData GetMetaTile(int _x, int _y, bool fromWorldCoords = true)
    {
        if (fromWorldCoords)
        {
            _x -= x;
            _y -= y;
        }

        if (_x >= 0 && _y >= 0 && _x < tiles.GetLength(0) && _y < tiles.GetLength(1))
            return tiles[_x,_y];
        return null;
    }
    public RoomMetaData(VertexNode roomAsNode, GridMetaData _grid)
    {
        type = RoomType.UNDEFINED;
        grid = _grid;
        //Make rooms one tile smaller in every direction so they don't touch each other
        roomAsNode.ParentCell.Size = new Vector2(roomAsNode.ParentCell.Size.x-2, roomAsNode.ParentCell.Size.y-2);
        x = (int)roomAsNode.VertexPos.x - Mathf.FloorToInt(roomAsNode.ParentCell.Size.x/2);
        y = (int)roomAsNode.VertexPos.y - Mathf.FloorToInt(roomAsNode.ParentCell.Size.y/2);
        tiles = new TileMetaData[(int)roomAsNode.ParentCell.Size.x, (int)roomAsNode.ParentCell.Size.y];
        for (int iy = 0; iy < tiles.GetLength(1); ++iy)
        {
            for (int ix = 0; ix < tiles.GetLength(0); ++ix)
            {
                tiles[ix, iy] = grid.GetMetaTile(x + ix, y + iy);
                tiles[ix, iy].partOf = this;
                if (ix == tiles.GetLength(0) - 1 || iy == tiles.GetLength(1) - 1 || ix == 0 || iy == 0)
                {
                    tiles[ix, iy].type = MetaTileType.WALL;
                }
                else
                {
                    tiles[ix, iy].type = MetaTileType.FLOOR;
                }
            }
        }
    }

    //Gathers all unique waypoints the room's tiles have to one list and constructs Neighbours list with that data. 
    public void ConstructNeighboursList()
    {
        List<Vector2> uniqueWaypoints = new List<Vector2>();
        //Find all unique waypoints within the room
        for (int iy = 0; iy < tiles.GetLength(1); ++iy)
        {
            for (int ix = 0; ix < tiles.GetLength(0); ++ix)
            {
                if (tiles[ix, iy].type == MetaTileType.DOOR)
                {
                    for (int i = 0; i < tiles[ix, iy].Waypoints.Count; ++i)
                    {
                        if (!uniqueWaypoints.Contains(tiles[ix, iy].Waypoints[i]))
                        {
                            uniqueWaypoints.Add(tiles[ix, iy].Waypoints[i]);
                        }
                    }
                }

            }
        }
        neighbours = new List<RoomMetaData>();
        //add all the neighbours to neighbours list
        for (int i = 0; i < uniqueWaypoints.Count; ++i)
        {
            neighbours.Add(grid.GetMetaTile((int)uniqueWaypoints[i].x, (int)uniqueWaypoints[i].y).partOf);
        }

        //Add waypoint data to all the tiles inside the room
        for (int iy = 0; iy < tiles.GetLength(1); ++iy)
        {
            for (int ix = 0; ix < tiles.GetLength(0); ++ix)
            {
                if (tiles[ix, iy].type != MetaTileType.WALL)
                {
                    for (int i = 0; i < uniqueWaypoints.Count; ++i)
                    {
                        tiles[ix, iy].AddWaypoint(uniqueWaypoints[i]);
                    }
                }

            }
        }
    }

    //Returns list of door locations at the same edge of room as the point given as parameter.
    //Returned positions are in world coordinates.
    public List<TileMetaData> GetDoorsAtEdgeOfRoom(int _x, int _y)
    {
        List<TileMetaData> doors = new List<TileMetaData>();
        if (!IsCorner(_x, _y))
        {
            // Determine in which wall the coordinates are.
            int ix = 0, iy = 0;
            bool horizontalWall= true;
            if ((_x - x) == 0 || (_x - x) == tiles.GetLength(0) - 1)
            {
                horizontalWall = false;
                ix = (_x - x);
            }
            else if ((_y - y) == 0 || (_y - y) == tiles.GetLength(1) - 1)
            {
                horizontalWall = true;
                iy = (_y - y);
            }

            //Loop through walls and add all the doors to the doorPositions list
            if (horizontalWall)
            {
                for (ix = 0; ix < tiles.GetLength(0); ++ix)
                {
                    if (tiles[ix, iy].type == MetaTileType.DOOR)
                        doors.Add(tiles[ix, iy]);
                }
            }
            else
            {
                for (iy = 0; iy < tiles.GetLength(1); ++iy)
                {
                    if (tiles[ix, iy].type == MetaTileType.DOOR)
                        doors.Add(tiles[ix, iy]);
                }
            }
        }

        return doors;
    }

    //return true if given coordinates are corner of the room
    public bool IsCorner(int _x, int _y)
    {
        return ((_x - x) == 0 || (_x - x) == tiles.GetLength(0)-1) &&
            ((_y - y) == 0 || (_y - y) == tiles.GetLength(1) - 1);
    }
}
