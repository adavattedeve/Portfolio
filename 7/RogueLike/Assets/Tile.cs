using UnityEngine;
using System.Collections.Generic;

public class Tile {
    // x and y are in world coordinates
    protected int x;
    public int X { get { return x; } }
    protected int y;
    public int Y { get { return y; } }

    //Is visible by the player
    private bool visible = false;
    public bool Visible { get {
            return visible;
        } set {
            visible = value;
            GameManager.instance.Fow.MarkTileDirty(x, y);
        }
    }
    //if the tile blocks vision
    public bool blockVision = true;

    protected GameObject go;
    protected List<Entity> entities;

    public virtual bool Moveable { get{ return true; } }

    public Tile(int _x, int _y, GameObject _go)
    {
        x = _x;
        y = _y;
        go = _go;
        go.transform.position = new Vector3(x, y, 0);
        entities = new List<Entity>();
    }
    public void AddEntity(Entity e)
    {        
        for (int i = 0; i < entities.Count; ++i)
        {
            entities[i].OnEnterSameTile(e);
        }
        entities.Add(e);
        e.CurrentTile = this;
    }

    public void RemoveEntity(Entity e)
    {
        entities.Remove(e);
    }
    public void Collide(Entity e)
    {
        for (int i = 0; i < entities.Count; ++i)
        {
            entities[i].OnCollide(e);
        }
    }
}
