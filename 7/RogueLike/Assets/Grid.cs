using UnityEngine;
using System.Collections.Generic;

public class Grid
{
    private Tile[,] map;

    public Tile levelStart = null;

    public Tile GetTile(int _x, int _y)
    {
        if (_x >= 0 && _y >= 0 && _x<map.GetLength(0) && _y < map.GetLength(1))
            return map[_x, _y];
        return null;
    }
    public int mapSizeX { get{ return map.GetLength(0); } }
    public int mapSizeY { get { return map.GetLength(1); } }

    public void CreateMapFromMetaData(GridMetaData metaGrid)
    {
        
        map = new Tile[metaGrid.Grid.GetLength(0), metaGrid.Grid.GetLength(1)];
        if (DataBase.instance == null)
            return;
        LevelGraphics graphics = DataBase.instance.GetLevelGraphics();
        for (int y = 0; y < map.GetLength(1); ++y)
        {
            for (int x = 0; x < map.GetLength(0); ++x)
            {
                if (metaGrid.Grid[x, y].type == MetaTileType.UNDEFINED)
                {
                    map[x, y] = null;
                    continue;
                }

                Tile newTile = null;
                int newX = metaGrid.Grid[x, y].X - (int)metaGrid.GridSWCorner.x;
                int newY = metaGrid.Grid[x, y].Y - (int)metaGrid.GridSWCorner.y;
                switch (metaGrid.Grid[x, y].type)
                {
                    case MetaTileType.FLOOR:
                        newTile = new FloorTile(newX, newY, graphics.GetFloorTileGO(metaGrid.NeihgbourTilesAre(x, y, MetaTileType.WALL)));
                        if (levelStart == null)
                            levelStart = newTile;
                        break;
                    case MetaTileType.DOOR:
                        newTile = new FloorTile(newX, newY, graphics.GetFloorTileGO(metaGrid.NeihgbourTilesAre(x, y, MetaTileType.WALL)));
                        newTile.AddEntity(new Door(newTile, graphics.GetDoorGO(metaGrid.NeihgbourTilesAre(x, y, MetaTileType.FLOOR))));
                        break;

                    case MetaTileType.WALL:
                        bool[] neighbourIsNotWall = metaGrid.NeihgbourTilesAre(x, y, MetaTileType.WALL, false);
                        bool[] neighbourIsUndefined = metaGrid.NeihgbourTilesAre(x, y, MetaTileType.UNDEFINED);

                        for (int i = 0; i < neighbourIsNotWall.Length; ++i)
                        {
                            if (neighbourIsUndefined[i])
                                neighbourIsNotWall[i] = !neighbourIsNotWall[i];
                        }
                        
                        newTile = new WallTile(newX, newY, graphics.GetWallTileGO(
                            neighbourIsNotWall, 
                            metaGrid.NeihgbourTilesAre(x, y, MetaTileType.WALL),
                            metaGrid.CornerNeihgbourTilesAre(x, y, MetaTileType.FLOOR)));
                        break;
                    
                }

                map[x, y] = newTile;
            }
        }
    }

    //Used for getNeighbour tiles so there is no need to create new list int every call
    private List<Tile> neighbourTiles = new List<Tile>();
    public List<Tile> GetNeighbourTiles(int _x, int _y) {
        neighbourTiles.Clear();
        for (int y=-1; y <= 1; ++y) {
            for (int x = -1; x <= 1; ++x) {
                if (x == 0 && y == 0)
                    continue;
                Tile neighbour = GetTile(_x + x, _y + y);
                if (neighbour != null)
                    neighbourTiles.Add(neighbour);
            }
        }
        return neighbourTiles;
    }
}
