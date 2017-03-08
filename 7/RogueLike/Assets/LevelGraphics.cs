using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LevelGraphics  {

    //Wall graphics
    #region
    [SerializeField]
    private Texture2D wallSpriteSheet;
    private int wallsInSheetX = 8;
    private int wallsInSheetY = 6;
    //F=floor, FC = corner floor
    [SerializeField]
    private Sprite wallF1000;
    [SerializeField]
    private Sprite wallF0100;
    [SerializeField]
    private Sprite wallF0010;
    [SerializeField]
    private Sprite wallF0001;

    [SerializeField]
    private Sprite wallF1111;

    [SerializeField]
    private Sprite wallF1000FC0100;
    [SerializeField]
    private Sprite wallF1000FC0010;
    [SerializeField]
    private Sprite wallF1000FC0110;

    [SerializeField]
    private Sprite wallF0100FC0010;
    [SerializeField]
    private Sprite wallF0100FC0001;
    [SerializeField]
    private Sprite wallF0100FC0011;

    [SerializeField]
    private Sprite wallF0010FC0001;
    [SerializeField]
    private Sprite wallF0010FC1000;
    [SerializeField]
    private Sprite wallF0010FC1001;

    [SerializeField]
    private Sprite wallF0001FC1000;
    [SerializeField]
    private Sprite wallF0001FC0100;
    [SerializeField]
    private Sprite wallF0001FC1100;

    //corners
    [SerializeField]
    private Sprite wallF1100;
    [SerializeField]
    private Sprite wallF0110;
    [SerializeField]
    private Sprite wallF0011;
    [SerializeField]
    private Sprite wallF1001;

    [SerializeField]
    private Sprite wallF1010;
    [SerializeField]
    private Sprite wallF0101;

    [SerializeField]
    private Sprite wallF0111;
    [SerializeField]
    private Sprite wallF1011;
    [SerializeField]
    private Sprite wallF1101;
    [SerializeField]
    private Sprite wallF1110;

    [SerializeField]
    private Sprite wallFC1000;
    [SerializeField]
    private Sprite wallFC0100;
    [SerializeField]
    private Sprite wallFC0010;
    [SerializeField]
    private Sprite wallFC0001;

    [SerializeField]
    private Sprite wallFC1100;
    [SerializeField]
    private Sprite wallFC0110;
    [SerializeField]
    private Sprite wallFC0011;
    [SerializeField]
    private Sprite wallFC1001;
    [SerializeField]
    private Sprite wallFC1010;
    [SerializeField]
    private Sprite wallFC0101;

    [SerializeField]
    private Sprite wallFC0111;
    [SerializeField]
    private Sprite wallFC1011;
    [SerializeField]
    private Sprite wallFC1101;
    [SerializeField]
    private Sprite wallFC1110;
    [SerializeField]
    private Sprite wallFC1111;


    [SerializeField]
    private Sprite wallF1100FC0010;
    [SerializeField]
    private Sprite wallF0110FC0001;
    [SerializeField]
    private Sprite wallF0011FC1000;
    [SerializeField]
    private Sprite wallF1001FC0100;
    #endregion


    //DoorGraphics
    [SerializeField]
    private Sprite door1010;
    [SerializeField]
    private Sprite door0101;

    //Floor graphics
    [SerializeField]
    private Sprite[] floorSprites;

    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    private GameObject doorPrefab;

    public void Init()
    {
        InitWallGraphics();
    }
    private void InitWallGraphics()
    {
        float h = wallSpriteSheet.height;
        float w = wallSpriteSheet.width;
        float imgSize = w / wallsInSheetX;
        Sprite[] walls = new Sprite[wallsInSheetX * wallsInSheetY];
        int x, y;
        for (int i = 0; i < walls.Length; ++i)
        {
            x = i % wallsInSheetX;
            y = i / wallsInSheetX;
            Rect rect = new Rect(x * imgSize, h - imgSize - y * imgSize, imgSize, imgSize);
            walls[i] = Sprite.Create(wallSpriteSheet, rect , new Vector2(0.5f, 0.5f), 64f);
        }
        wallF1000 = walls[0];
        wallF0100 = walls[1];
        wallF0010 = walls[2];
        wallF0001 = walls[3];

        wallF0111 = walls[4];
        wallF1011 = walls[5];
        wallF1101 = walls[6];
        wallF1110 = walls[7];

        wallF1010 = walls[8];
        wallF0101 = walls[9];
        wallF1100 = walls[10];
        wallF0110 = walls[11];
        wallF0011 = walls[12];
        wallF1001 = walls[13];

        wallFC1000 = walls[14];
        wallFC0100 = walls[15];
        wallFC0010 = walls[16];
        wallFC0001 = walls[17];

        wallFC1100 = walls[18];
        wallFC0110 = walls[19];
        wallFC0011 = walls[20];
        wallFC1001 = walls[21];
        wallFC1010 = walls[22];
        wallFC0101 = walls[23];

        wallFC0111 = walls[24];
        wallFC1011 = walls[25];
        wallFC1101 = walls[26];
        wallFC1110 = walls[27];
        wallFC1111 = walls[28];

        wallF1000FC0100 = walls[29];
        wallF1000FC0010 = walls[30];
        wallF1000FC0110 = walls[31];

        wallF0100FC0010 = walls[32];
        wallF0100FC0001 = walls[33];
        wallF0100FC0011 = walls[34];

        wallF0010FC0001 = walls[35];
        wallF0010FC1000 = walls[36];
        wallF0010FC1001 = walls[37];

        wallF0001FC1000 = walls[38];
        wallF0001FC0100 = walls[39];
        wallF0001FC1100 = walls[40];

        wallF1100FC0010 = walls[41];
        wallF0110FC0001 = walls[42];
        wallF0011FC1000 = walls[43];
        wallF1001FC0100 = walls[44];

        wallF1111 = walls[45];
}
    public GameObject GetWallTileGO(bool[] neighbourIsNotWallOrUndefined, bool[] neighbourIsWall,  bool[] CornerNeighbourIsFloor)
    {
        int floorCount = 0;
        int wallCount = 0;
        int cornerFloorCount = 0;
        GameObject newWall = MonoBehaviour.Instantiate(tilePrefab) as GameObject;

        for (int i = 0; i < neighbourIsNotWallOrUndefined.Length; ++i)
        {
            if (neighbourIsNotWallOrUndefined[i])
                ++floorCount;
            if (neighbourIsWall[i])
                ++wallCount;
            if (CornerNeighbourIsFloor[i])
                ++cornerFloorCount;
        }

        Sprite wall = null;

        switch (floorCount)
        {
            case 0:
                switch (cornerFloorCount)
                {
                    case 1:
                        if (CornerNeighbourIsFloor[0])
                            wall = wallFC1000;
                        else if (CornerNeighbourIsFloor[1])
                            wall = wallFC0100;
                        else if (CornerNeighbourIsFloor[2])
                            wall = wallFC0010;
                        else if (CornerNeighbourIsFloor[3])
                            wall = wallFC0001;
                        break;
                    case 2:
                        if (CornerNeighbourIsFloor[0] && CornerNeighbourIsFloor[1])
                            wall = wallFC1100;
                        else if (CornerNeighbourIsFloor[1] && CornerNeighbourIsFloor[2])
                            wall = wallFC0110;
                        else if (CornerNeighbourIsFloor[2] && CornerNeighbourIsFloor[3])
                            wall = wallFC0011;
                        else if (CornerNeighbourIsFloor[3] && CornerNeighbourIsFloor[0])
                            wall = wallFC1001;
                        else if (CornerNeighbourIsFloor[0] && CornerNeighbourIsFloor[2])
                            wall = wallFC1010;
                        else if (CornerNeighbourIsFloor[1] && CornerNeighbourIsFloor[3])
                            wall = wallFC0101;
                        break;
                    case 3:
                        if (!CornerNeighbourIsFloor[0])
                            wall = wallFC0111;
                        else if (!CornerNeighbourIsFloor[1])
                            wall = wallFC1011;
                        else if (!CornerNeighbourIsFloor[2])
                            wall = wallFC1101;
                        else if (!CornerNeighbourIsFloor[3])
                            wall = wallFC1110;
                        break;
                    case 4:
                        wall = wallFC1111;
                        break;

                }
                break;

            case 1:

                if (neighbourIsNotWallOrUndefined[0])
                {
                    if (CornerNeighbourIsFloor[1] && CornerNeighbourIsFloor[2])
                        wall = wallF1000FC0110;
                    else if (CornerNeighbourIsFloor[1])
                        wall = wallF1000FC0100;
                    else if (CornerNeighbourIsFloor[2])
                        wall = wallF1000FC0010;
                    else
                        wall = wallF1000;
                }
                else if (neighbourIsNotWallOrUndefined[1])
                {
                    if (CornerNeighbourIsFloor[2] && CornerNeighbourIsFloor[3])
                        wall = wallF0100FC0011;
                    else if (CornerNeighbourIsFloor[2])
                        wall = wallF0100FC0010;
                    else if (CornerNeighbourIsFloor[3])
                        wall = wallF0100FC0001;
                    else
                        wall = wallF0100;
                }
                else if (neighbourIsNotWallOrUndefined[2])
                {
                    if (CornerNeighbourIsFloor[3] && CornerNeighbourIsFloor[0])
                        wall = wallF0010FC1001;
                    else if (CornerNeighbourIsFloor[3])
                        wall = wallF0010FC0001;
                    else if (CornerNeighbourIsFloor[0])
                        wall = wallF0010FC1000;
                    else
                        wall = wallF0010;
                }
                else
                {
                    if (CornerNeighbourIsFloor[0] && CornerNeighbourIsFloor[1])
                        wall = wallF0001FC1100;
                    else if (CornerNeighbourIsFloor[0])
                        wall = wallF0001FC1000;
                    else if (CornerNeighbourIsFloor[1])
                        wall = wallF0001FC0100;
                    else
                        wall = wallF0001;
                }
              
                break;

            case 2:

                if (neighbourIsNotWallOrUndefined[0] && neighbourIsNotWallOrUndefined[2])
                    wall = wallF1010;
                else if (neighbourIsNotWallOrUndefined[1] && neighbourIsNotWallOrUndefined[3])
                    wall = wallF0101;
                else if (neighbourIsNotWallOrUndefined[0] && neighbourIsNotWallOrUndefined[1])
                {
                    if (CornerNeighbourIsFloor[2])
                        wall = wallF1100FC0010;
                    else
                        wall = wallF1100;
                }
                else if (neighbourIsNotWallOrUndefined[1] && neighbourIsNotWallOrUndefined[2])
                {
                    if (CornerNeighbourIsFloor[3])
                        wall = wallF0110FC0001;
                    else
                        wall = wallF0110;
                }
                else if (neighbourIsNotWallOrUndefined[2] && neighbourIsNotWallOrUndefined[3])
                {
                    if (CornerNeighbourIsFloor[0])
                        wall = wallF0011FC1000;
                    else
                        wall = wallF0011;
                }
                else if (neighbourIsNotWallOrUndefined[3] && neighbourIsNotWallOrUndefined[0])
                {
                    if (CornerNeighbourIsFloor[1])
                        wall = wallF1001FC0100;
                    else
                        wall = wallF1001;
                }

                break;
            case 3:
                if (!neighbourIsNotWallOrUndefined[0])
                    wall = wallF0111;
                else if (!neighbourIsNotWallOrUndefined[1])
                    wall = wallF1011;
                else if (!neighbourIsNotWallOrUndefined[2])
                    wall = wallF1101;
                else if (!neighbourIsNotWallOrUndefined[3])
                    wall = wallF1110;
                break;
            case 4:
                wall = wallF1111;
                break;
        }
        if (wall == null)
        {
            Debug.Log("No valid sprite found");
            for (int i = 0; i < neighbourIsNotWallOrUndefined.Length; ++i)
            {
                Debug.Log(neighbourIsNotWallOrUndefined[i] + "  " + CornerNeighbourIsFloor[i]);
            }
        }
        else
            newWall.GetComponent<SpriteRenderer>().sprite = wall;
        return newWall;
    }

    public GameObject GetFloorTileGO(bool[] neighbourIsWall)
    {
        GameObject newFloor = MonoBehaviour.Instantiate(tilePrefab) as GameObject;
        newFloor.GetComponent<SpriteRenderer>().sprite = floorSprites[Random.Range(0, floorSprites.Length)];

        return newFloor;
    }

    public GameObject GetDoorGO(bool[] neighbourIsFloor)
    {
        Sprite doorSprite = null;
        if (neighbourIsFloor[1] && neighbourIsFloor[3])
            doorSprite = door1010;
        else if (neighbourIsFloor[0] && neighbourIsFloor[2])
            doorSprite = door0101;

        GameObject door = MonoBehaviour.Instantiate(doorPrefab);
        SpriteRenderer renderer = door.GetComponent<SpriteRenderer>();
        renderer.sprite = doorSprite;
        return door;
    }
}
