using UnityEngine;
using System.Collections;

public class Door : Entity {

    public Door(Tile _tile, GameObject _go) : base( _tile,  _go)
    {
        go.transform.position = new Vector3(tile.X, tile.Y, 0);
    }
}
