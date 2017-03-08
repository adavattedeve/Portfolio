using UnityEngine;
using System.Collections;

public class FloorTile : Tile
{
    public override bool Moveable { get { return true; } }

    public FloorTile(int _x, int _y, GameObject _go) : base(_x, _y, _go) {
        blockVision = false;
    }
}
