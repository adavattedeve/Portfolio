using UnityEngine;
using System.Collections.Generic;

public class Player : Entity {
    private List<Tile> visibleTiles = new List<Tile>();

    public override Tile CurrentTile { get { return tile; } set {
            OnEnterTile(tile, value);
            tile = value; } }

    public Player(Tile _tile, GameObject _go) : base(_tile, _go) {
        go.transform.position = new Vector3(tile.X, tile.Y, -0.5f);
    }

    private void OnEnterTile(Tile oldTile, Tile newTile) {

        RefreshVisibleTiles(newTile.X, newTile.Y);
    }

    private void RefreshVisibleTiles (int _x, int _y) {
        List<Tile> newVisibleTiles = VisionSystem.GetVisibleTiles(_x, _y, 2);
        for (int i = 0; i < visibleTiles.Count; ++i) {
            if (!newVisibleTiles.Contains(visibleTiles[i]))
                visibleTiles[i].Visible = false;
        }

        visibleTiles = newVisibleTiles;

        for (int i = 0; i < visibleTiles.Count; ++i) {
            visibleTiles[i].Visible = true;
        }

    }
}
