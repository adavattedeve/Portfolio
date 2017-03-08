using UnityEngine;
using System.Collections.Generic;

public enum MetaTileType { UNDEFINED, WALL, FLOOR, DOOR, UNWALKABLE}
public class TileMetaData
{
    // x and y are in world coordinates
    private int x;
    public int X { get { return x; } }
    private int y;
    public int Y { get { return y; } }

    public MetaTileType type;
    public RoomMetaData partOf;

    private List<Vector2> waypoints; //Closest rooms you can get from this tile. Used by wandering enemies for pathfinding targets.
    public List<Vector2> Waypoints {
        get { return waypoints; } }
    public void AddWaypoint(Vector2 point)
    {
        if (!waypoints.Contains(point) && ((int)point.x != x || (int)point.y != y))
        {
            waypoints.Add(point);
        }
            
    }
    private GameObject visualizationGO;
    public TileMetaData(int _x, int _y, MetaTileType _type)
    {
        x = _x;
        y = _y;
        type = _type;
        waypoints = new List<Vector2>();
    }

    public void Draw(Color color, GameObject prefab)
    {
        if (!visualizationGO)
        {
            visualizationGO = MonoBehaviour.Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
        }
        visualizationGO.GetComponent<Renderer>().material.color = color;

    }
    public void StopDraw()
    {
        if (!visualizationGO)
        {
            return;
        }
        MonoBehaviour.Destroy(visualizationGO);
    }
}