using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class CorridorDigger {
    //Animation && step by step processing params for CorridorDigger algoritm
    #region
    [SerializeField]
    private bool doStep = true;
    private bool canContinue;

    [SerializeField]
    private bool animate = true;

    [SerializeField]
    private float animateTime = 0.5f;
    private float animateTimer = 0;
    #endregion
    //Animation && step by step processing params for single corridors
    #region 
    [SerializeField]
    private bool corridorDoStep = true;
    private bool corridorCanContinue;

    [SerializeField]
    private bool corridorAnimate = true;

    [SerializeField]
    private float corridorAnimateTime = 0.5f;
    private float corridorAnimateTimer = 0;
    #endregion

    private bool visualizationEnabled;

    [SerializeField]
    private CornerValueBoundaries[] cornerValueBoundaries;
    [SerializeField]
    [Range(0f, 1f)]
    [Header("how much corner lengths can be offset from average length")]
    private float offsetFromAverage;
    [SerializeField]
    private int maxDoorsPerEdge = 1;
    [SerializeField]
    private int maxStepsPerUpdate = 100;

    private GridMetaData grid;
    private List<Edge> paths;

    private int currentPathIndex;

    //current position of the digger
    private int posX;
    private int posY;

    //current path's target position
    private int targetX;
    private int targetY;

    //Current paths starting and ending point, needed for adding waypoint data to tiles for enemies wandering behaviour.

    private Vector2 startingPoint;
    private Vector2 endingPoint;

    //Delta position to target from current position
    private int DeltaX { get { return targetX - posX; } }
    private int DeltaY { get { return targetY - posY; } }

    //How many corners there is remaining within current path
    private int corners;

    //How much there is left x and y corner movement
    private int cornerX;
    private int cornerY;

    //true if currently moving x direction. Used in corner movement.
    private bool xMovement;

    private List<Cell> allCells;

    private int currentStepsPerUpdate = 0;
    

    private bool isDone;
    public bool IsDone { get{ return isDone; } }
    public void Init(GridMetaData _grid, List<Edge> _paths, List<Cell> _allCells, bool _visualizationEnabled)
    {
        visualizationEnabled = _visualizationEnabled;
        grid = _grid;
        paths = _paths;
        allCells = _allCells;
        corners = 0;
        cornerX = 0;
        cornerY = 0;
        currentPathIndex = -1;
        isDone = false;
    }

    public void Update()
    {
        if (isDone)
            return;

        if (!animate && !doStep)
        {
            if (!corridorAnimate && !corridorDoStep)
            {
                while (!isDone)
                {
                    ProgresCorridorDigger();
                    ++currentStepsPerUpdate;
                    if (currentStepsPerUpdate >= maxStepsPerUpdate)
                    {
                        currentStepsPerUpdate = 0;
                        break;
                    }
                }
            }
            else if (corridorCanContinue)
            {
                corridorAnimateTimer = 0;
                ProgresCorridorDigger();
            }
            corridorCanContinue = !corridorAnimate && !corridorDoStep;
            if (corridorAnimate && !corridorCanContinue)
            {
                corridorAnimateTimer += Time.deltaTime;
                if (corridorAnimateTimer > corridorAnimateTime)
                {
                    corridorCanContinue = true;
                    corridorAnimateTimer = 0;
                }
            }
            if (corridorDoStep && Input.GetMouseButtonDown(0))
            {
                corridorCanContinue = true;
            }
        }
        else if (canContinue)
        {

            animateTimer = 0;
            if (corners == 0 && cornerX == 0 && cornerY == 0)
            {
                ProgresCorridorDigger();
            }
            if (!corridorAnimate && !corridorDoStep)
            {
                ProgresCorridorDigger();
                while (corners != 0 || cornerX != 0 || cornerY != 0)
                {
                    ProgresCorridorDigger();
                }
            }
            else if (corridorCanContinue)
            {
                corridorAnimateTimer = 0;
                ProgresCorridorDigger();
            }
            corridorCanContinue = !corridorAnimate && !corridorDoStep;
            if (corridorAnimate && !corridorCanContinue)
            {
                corridorAnimateTimer += Time.deltaTime;
                if (corridorAnimateTimer > corridorAnimateTime)
                {
                    corridorCanContinue = true;
                    corridorAnimateTimer = 0;
                }
            }
            if (corridorDoStep && Input.GetMouseButtonDown(0))
            {
                corridorCanContinue = true;
            }

        }
        canContinue = (!animate && !doStep) || (corners != 0 || cornerX != 0 || cornerY != 0);
        if (animate && !canContinue)
        {
            animateTimer += Time.deltaTime;
            if (animateTimer > animateTime)
            {
                canContinue = true;
                animateTimer = 0;
            }
        }
        if (doStep && Input.GetMouseButtonDown(0))
        {
            canContinue = true;
        }
    }
    private void ProgresCorridorDigger()
    {
        if (isDone)
            return;

        if (cornerX == 0 && cornerY == 0)
        {
            if (corners == 0)
            {
                StartNewPath();
                return;
            }
            else
            {
                StartNewCorner();
                return;
            }
        }

        ProgresCornerMovement();
    }
    private void StartNewPath()
    {
        if (currentPathIndex>=0 && visualizationEnabled)
            paths[currentPathIndex].StopDraw();

        currentPathIndex++;
        if (currentPathIndex == paths.Count)
        {
            Finished();
            return;
        }
        if (visualizationEnabled)
            paths[currentPathIndex].Draw();
        corners = 0;
        cornerX = 0;
        cornerY = 0;

        CalculatePathPositions(paths[currentPathIndex].Node0, paths[currentPathIndex].Node1);
        // Dig at the path starting tile
        Dig(posX, posY);
        // set up corner amount
        int distance = Mathf.Abs(DeltaX) + Mathf.Abs(DeltaY);
        for (int i = 0; i < cornerValueBoundaries.Length; ++i)
        {
            if (distance < cornerValueBoundaries[i].maxDistance)
            {
                corners = cornerValueBoundaries[i].CornerAmount;
                break;
            }
        }
        // If corner value boundaries are not set then assing some value to corners so algoritm still works.
        if (corners == 0)
        {
            corners = 2;
        }
    }
    private void StartNewCorner()
    {
        // calc average lengths
        int averageX = Mathf.Abs(DeltaX) / corners;
        int averageY = Mathf.Abs(DeltaY) / corners;
        //set up cornerX, cornerY
        cornerX = (int)Random.Range(averageX - averageX * offsetFromAverage, averageX + averageX * offsetFromAverage);
        cornerY = (int)Random.Range(averageY - averageY * offsetFromAverage, averageY + averageY * offsetFromAverage);
        // clamp corner x & y
        cornerX = Mathf.Clamp(cornerX, 0, Mathf.Abs(DeltaX));
        cornerY = Mathf.Clamp(cornerY, 0, Mathf.Abs(DeltaY));
        //set up movementX
        xMovement = Random.Range(0f, 1f) > 0.5f;
        //If corners are exhausted and target isn't reached --> add counter to corners variable as long as the target isn't reached
        if ((posX + cornerX != targetX || posY + cornerY != targetY) && corners == 1)
        {
            ++corners;
        }
        --corners;
    }
    private void ProgresCornerMovement()
    {
        //Movement in this step
        int movementX = 0;
        int movementY = 0;

        //determine direction
        if (xMovement)
        {
            if (cornerX == 0)
            {
                xMovement = false;
                movementY++;
            }
            else
            {
                movementX++;
            }                
        }
        else if (!xMovement)
        {
            if (cornerY == 0)
            {
                xMovement = true;
                movementX++;
            }
            else
            {
                movementY++;               
            }            
        }

        int signX = 1;
        if (DeltaX != 0)
            signX = DeltaX / (Mathf.Abs(DeltaX));

        int signY = 1;
        if (DeltaY != 0)
            signY = DeltaY / (Mathf.Abs(DeltaY));

        int tempPosX = posX + signX * movementX;
        int tempPosY = posY + signY * movementY;

        if (grid.GetMetaTile(tempPosX, tempPosY).type == MetaTileType.WALL)
        {
            if (grid.GetMetaTile(tempPosX, tempPosY).partOf.IsCorner(tempPosX, tempPosY) ||
                grid.GetMetaTile(posX, posY).type == MetaTileType.WALL || 
                grid.GetMetaTile(posX, posY).type == MetaTileType.DOOR)
            {
                movementX = movementX == 0 ? 1 : 0;
                movementY = movementY == 0 ? 1 : 0;
            }
        }

        posX += signX * movementX;
        posY += signY * movementY;

        Dig(posX , posY);
        cornerX = cornerX - movementX < 0 ? 0 : cornerX - movementX;
        cornerY = cornerY - movementY < 0 ? 0 : cornerY - movementY;
    }

    private void Dig(int _x, int _y)
    {
       
        TileMetaData tile = grid.GetMetaTile(_x, _y);
        // Check if there is node, empty or room
        // if empty --> type to floor
        if (tile.type == MetaTileType.UNDEFINED)
        {
            tile.AddWaypoint(startingPoint);
            tile.AddWaypoint(endingPoint);
            tile.type = MetaTileType.FLOOR;
        }
        //else if tile is part of a room 
        else if (tile.partOf != null)
        {
            if (tile.type == MetaTileType.FLOOR)
            {
                return;
            }
            //type is wall --> check if in that room there is door in neighbour tile.
            //If door is found change current dig position to that and if not, create door.
            else if (tile.type == MetaTileType.WALL)
            {
                TileMetaData neighbourTile = tile.partOf.GetMetaTile(_x + 1, _y);
                if (neighbourTile!=null && neighbourTile.type == MetaTileType.DOOR)
                {
                    posX = _x+1;
                    posY = _y;
                    return;
                }
                neighbourTile = tile.partOf.GetMetaTile(_x - 1, _y);
                if (neighbourTile != null && neighbourTile.type == MetaTileType.DOOR)
                {
                    posX = _x-1;
                    posY = _y;
                    return;
                }
                neighbourTile = tile.partOf.GetMetaTile(_x, _y + 1);
                if (neighbourTile != null && neighbourTile.type == MetaTileType.DOOR)
                {
                    posX = _x;
                    posY = _y+1;
                    return;
                }
                neighbourTile = tile.partOf.GetMetaTile(_x, _y - 1);
                if (neighbourTile != null && neighbourTile.type == MetaTileType.DOOR)
                {
                    posX = _x;
                    posY = _y-1;
                    return;
                }
                tile.AddWaypoint(startingPoint);
                tile.AddWaypoint(endingPoint);
                tile.type = MetaTileType.DOOR;
                return;
            }
        }

        //RaycastHit[] cells = Physics.BoxCastAll(new Vector3(_x, _y, 0), new Vector3(0.4f, 0.4f, 1f), new Vector3(0, 0, -1));

        //Intersecting cells
        Cell intersectingCell = GetIntersectingCell(new Vector2(_x, _y));
        for (int i=0; i< allCells.Count; ++i)
        //If cell, change all cell tiles to floor, 
        if (intersectingCell != null)
        {         

            int xSize = (int)intersectingCell.Size.x;
            int ySize = (int)intersectingCell.Size.y;

            int cellPosX = (int)intersectingCell.Position.x - Mathf.FloorToInt(xSize / 2);
            int cellPosY = (int)intersectingCell.Position.y - Mathf.FloorToInt(ySize / 2);

            for (int iy = 0; iy < ySize; ++iy)
            {
                for (int ix = 0; ix < xSize; ++ix)
                {
                    TileMetaData cellTile = grid.GetMetaTile(cellPosX + ix, cellPosY + iy);
                        if (cellTile != null && cellTile.type == MetaTileType.UNDEFINED)
                        {
                            cellTile.AddWaypoint(startingPoint);
                            cellTile.AddWaypoint(new Vector2(targetX, targetY));
                            cellTile.type = MetaTileType.FLOOR;
                        }
                        
                }
            }

        }
        
    }
    
    private void Finished()
    {
        grid.SurroundCorridorsWithWalls();
        isDone = true;
    }
    //Calculates door and pathing positions from room to room
    private void CalculatePathPositions(VertexNode from, VertexNode to)
    {
        int roomPositionX = (int)from.VertexPos.x;
        int roomPositionY = (int)from.VertexPos.y;

        int targetRoomPositionX = (int)to.VertexPos.x;
        int targetRoomPositionY = (int)to.VertexPos.y;

        
        int startingRoomHalfWidth = grid.GetMetaTile((int)from.VertexPos.x, (int)from.VertexPos.y).partOf.HalfWidth;
        int startingRoomHalfHeight = grid.GetMetaTile((int)from.VertexPos.x, (int)from.VertexPos.y).partOf.HalfHeight;

        int targetRoomHalfWidth = grid.GetMetaTile((int)to.VertexPos.x, (int)to.VertexPos.y).partOf.HalfWidth;
        int targetRoomHalfHeight = grid.GetMetaTile((int)to.VertexPos.x, (int)to.VertexPos.y).partOf.HalfHeight;

        float k = Mathf.Abs((targetRoomPositionY - roomPositionY) / (float)(targetRoomPositionX - roomPositionX));

        float minimumKForStartingRoom = startingRoomHalfHeight / (float)startingRoomHalfWidth;
        float minimumKForTargetRoom = targetRoomHalfHeight / (float)targetRoomHalfWidth;

        //Start room
        int pathStartingDoorX = 0;
        int pathStartingDoorY = 0;
        TileMetaData startingDoor;

        //door in vertical wall
        if (k < minimumKForStartingRoom)
        {
            int sign = targetRoomPositionX - roomPositionX > 0 ?  1 :  -1;
            
            pathStartingDoorX = roomPositionX + sign * startingRoomHalfWidth;
            pathStartingDoorY = roomPositionY + Random.Range(-startingRoomHalfHeight + 1, startingRoomHalfHeight - 1);
            startingDoor = GetValidDoor(pathStartingDoorX, pathStartingDoorY);

            posX = startingDoor.X + sign * 1;
            posY = startingDoor.Y;
        }
        // Door in horizontal wall
        else
        {
            int sign = targetRoomPositionY - roomPositionY > 0 ? 1 : -1;

            pathStartingDoorX = roomPositionX + Random.Range(-startingRoomHalfWidth + 1, startingRoomHalfWidth - 1); ;
            pathStartingDoorY = roomPositionY + sign * startingRoomHalfHeight;
            startingDoor = GetValidDoor(pathStartingDoorX, pathStartingDoorY);

            posX = startingDoor.X;
            posY = startingDoor.Y + sign * 1;

        }

        //TARGET
        int pathTargetDoorX = 0;
        int pathTargetDoorY = 0;
        TileMetaData targetDoor;

        //door in vertical wall
        if (k < minimumKForTargetRoom)
        {
            int sign = targetRoomPositionX - roomPositionX <= 0 ? 1 : -1;

            pathTargetDoorX = targetRoomPositionX + sign * targetRoomHalfWidth;
            pathTargetDoorY = targetRoomPositionY + Random.Range(-targetRoomHalfHeight + 1, targetRoomHalfHeight - 1);
            targetDoor = GetValidDoor(pathTargetDoorX, pathTargetDoorY);

            targetX = targetDoor.X + sign * 1;
            targetY = targetDoor.Y;
        }
        // Door in horizontal wall
        else
        {
            int sign = targetRoomPositionY - roomPositionY <= 0 ? sign = 1 : sign = -1;

            pathTargetDoorX = targetRoomPositionX + Random.Range(-targetRoomHalfWidth + 1, targetRoomHalfWidth - 1);
            pathTargetDoorY = targetRoomPositionY + sign * targetRoomHalfHeight;
            targetDoor = GetValidDoor(pathTargetDoorX, pathTargetDoorY);

            targetX = targetDoor.X;
            targetY = targetDoor.Y + sign * 1;
        }
        startingPoint = new Vector2(pathStartingDoorX, pathStartingDoorY);
        endingPoint = new Vector2(pathTargetDoorX, pathTargetDoorY);

        startingDoor.AddWaypoint(endingPoint);
        startingDoor.type = MetaTileType.DOOR;

        targetDoor.AddWaypoint(startingPoint);
        targetDoor.type = MetaTileType.DOOR;
    }

    private TileMetaData GetValidDoor(int _x, int _y)
    {
        TileMetaData door = grid.GetMetaTile(_x, _y);
        List<TileMetaData> doorsAtEdgeOfRoom = door.partOf.GetDoorsAtEdgeOfRoom(door.X, door.Y);
        if (doorsAtEdgeOfRoom.Count > 0)
        {
            if (doorsAtEdgeOfRoom.Count >= maxDoorsPerEdge)
            {
                door = doorsAtEdgeOfRoom[Random.Range(0, doorsAtEdgeOfRoom.Count)];
            }
            else if (GridMetaData.IsNeighbours(door, doorsAtEdgeOfRoom[0]))
            {
                door = doorsAtEdgeOfRoom[0];
            }
        }
        return door;
    }
    private Cell GetIntersectingCell(Vector2 point)
    {
        for (int i = 0; i < allCells.Count; ++i)
        {
            if (allCells[i].PointIntersects(point))
                return allCells[i];
        }
        return null;
    }
    [System.Serializable]
    private class CornerValueBoundaries
    {

        public int maxDistance;
        [SerializeField]
        private int minCorners;
        [SerializeField]
        private int maxCorners;
        public int CornerAmount { get{ return Random.Range(minCorners, maxCorners); } }
    }
}
