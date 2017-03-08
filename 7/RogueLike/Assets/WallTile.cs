using UnityEngine;
using System.Collections;

public class WallTile : Tile
{
    public override bool Moveable { get { return false; } }
    public WallTile(int _x, int _y,  GameObject _go) : base(_x, _y, _go) {
        blockVision = true;
    }

}