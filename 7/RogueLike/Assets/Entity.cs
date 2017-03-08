using UnityEngine;
using System.Collections;

public class Entity  {
    protected Tile tile; // current tile
    public virtual Tile CurrentTile { get{ return tile; } set { tile = value; } }
    public bool moveable; // if true other entities can enter to same tile
    public GameObject go;

    public Entity(Tile _tile, GameObject _go)
    {
        tile = _tile;
        go = _go;
    }

    // interactions for when entity tries to enter same tile with other entiity but can't
    public virtual void OnCollide(Entity other)
    {

    }
    // interactions for when entity enters to same tile with other entity
    public virtual void OnEnterSameTile(Entity other)
    {

    }
    //public virtual void TakeHit(Attack attack) // interactions for when attack, spell or other effect happens to tile where the entity is.
}
