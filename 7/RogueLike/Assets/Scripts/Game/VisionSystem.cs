using UnityEngine;
using System.Collections.Generic;

public class VisionSystem {

    //return all currently seen tiles.
    //x,y: position in grid
    //sight: length of the vision
    public static List<Tile> GetVisibleTiles(int x, int y, int sight) {
        Grid grid = GameManager.Grid;
        int fieldOfVisionWidth = sight * 2 + 1;
        int testingPoints = sight * 8;

        List<Tile> visibleTiles = new List<Tile>();
        Tile currentTile = grid.GetTile(x, y);
        if (currentTile != null)
            visibleTiles.Add(currentTile);

        Vector2 currentPos = new Vector2();
        Vector2 testingPoint = new Vector2();

        for (int i=0; i< testingPoints; ++i) {
            currentPos.x = x;
            currentPos.y = y;

            //calculate next testing point
            if (i ==0) {
                testingPoint.x = currentPos.x - sight;
                testingPoint.y = currentPos.y - sight;
            }
            else if (i / (fieldOfVisionWidth * 2) < 1) {                
                if (i == fieldOfVisionWidth) {
                    testingPoint.y += fieldOfVisionWidth - 1;
                    testingPoint.x -= fieldOfVisionWidth - 1;
                } else
                    testingPoint.x += 1;

            }
            else {
                if (i % 2 == 0){
                    testingPoint.x -= fieldOfVisionWidth - 1;
                    testingPoint.y--;
                }
                else
                    testingPoint.x += fieldOfVisionWidth - 1;

            }

            Vector2 direction = testingPoint - currentPos;
            direction.Normalize();
            for (int i2=0; i2< sight; ++i2) {
                currentPos += direction;

                Tile tile = grid.GetTile(Mathf.RoundToInt(currentPos.x), Mathf.RoundToInt(currentPos.y));

                if (tile == null)
                    break;
                if (!visibleTiles.Contains(tile))
                    visibleTiles.Add(tile);
                if (tile.blockVision)
                    break;
            }


        }
        
        return visibleTiles;
    }
}
