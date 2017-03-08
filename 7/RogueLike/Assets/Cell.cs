using UnityEngine;
using System.Collections.Generic;

public class Cell{
    [SerializeField]
    private float steeringVelocity = 1;
    [SerializeField]
    private float steeringVelocityCap = 2;
    private List<Cell> allCells = new List<Cell>();

    private Vector2 size;
    public Vector2 Size {get { return size; } set { size = value; } }

    private Vector2 position;
    public Vector2 Position{ get { return position; }}

    private bool initialized=false;

    // Used for noticing if cell is moving back and forward
    private Vector2 oldPosition = new Vector2(Mathf.Infinity, Mathf.Infinity); // last frame
    private Vector2 olderPosition = new Vector2(Mathf.Infinity, Mathf.Infinity); // position at frame before last frame


    private bool hasStopped = false;
    public bool HasStopped { get { return hasStopped && initialized; } }

    private static GameObject visualizationPrefab;
    private GameObject visualizationGO;

    public Cell(Vector2 _position, Vector2 _size)
    {
        position = _position;
        size = _size;
    }

	public void Init (List<Cell> _allCells) {
        
        allCells = _allCells;
        initialized = true;
    }


    public void Update () {

        if (initialized)
        {
            //Search all cells which this cell overlapps with and add for each of those movement to delta variables.
            float deltaX=0;
            float deltaY=0;
            for (int i = 0; i < allCells.Count; i++)
            {

                if (allCells[i] != this)
                {
                    if (OverlapsWith(allCells[i]))
                    {
                        Vector2 direction = position - allCells[i].Position;
                        if (direction == Vector2.zero)
                        {
                            direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));                            
                        }
                        direction.Normalize();

                        deltaX += steeringVelocity * direction.x;
                        deltaY += steeringVelocity * direction.y;

                    }
                }
            }
            //Clamp deltas to velocity cap
            if (deltaX != 0)
            {
                deltaX = (deltaX / Mathf.Abs(deltaX)) * Mathf.Min(Mathf.Abs(deltaX), steeringVelocityCap);                
            }
            if (deltaY != 0)
            {
                deltaY = (deltaY / Mathf.Abs(deltaY)) * Mathf.Min(Mathf.Abs(deltaY), steeringVelocityCap);
            }

                
            position = new Vector2(Mathf.Round(position.x + deltaX), Mathf.Round(position.y + deltaY));
            //Reposition if cell is moving back and forth.
            if (position.x == olderPosition.x && position.y == olderPosition.y && (position.x != oldPosition.x || position.y != oldPosition.y))
            {
                Reposition();
            }
            //Cell has stopped if position and old position are equal
            if (position.x == oldPosition.x && position.y == oldPosition.y)
            {
                hasStopped = true;
            }
            else {
                hasStopped = false;
            }

            if (visualizationGO != null)
            {
                visualizationGO.transform.position = new Vector3(position.x, position.y, 1);
            }

            olderPosition = oldPosition;
            oldPosition = new Vector2(position.x, position.y);           
        }
    }

    //returns true if this gameobject overlaps with go
    private bool OverlapsWith(Cell otherCell)
    {
        Vector2 otherSize = otherCell.Size;

        if (position.x - size.x / 2 < otherCell.Position.x + otherSize.x / 2 &&
        position.x + size.x / 2 > otherCell.Position.x - otherSize.x / 2 &&
        position.y - size.y / 2 < otherCell.Position.y + otherSize.y / 2 &&
        position.y + size.y / 2 > otherCell.Position.y - otherSize.y / 2)
        {
            return true;
        }

        return false;
    }

    private void Reposition()
    {
        position = MapGenerator.instance.GetRandomCellPosition();
    }

    //Sets visualization game object's color
    public void SetColor(Color color)
    {
        if (visualizationGO != null)
        {
            visualizationGO.GetComponent<Renderer>().material.color = color;
        }
    }

    public void Draw()
    {
        if (visualizationPrefab == null)
        {
            visualizationPrefab = Resources.Load<GameObject>("Cell");
        }
        visualizationGO = MonoBehaviour.Instantiate(visualizationPrefab, new Vector3(position.x, position.y, 1), Quaternion.identity) as GameObject;
        visualizationGO.transform.localScale = new Vector3(size.x, size.y, 1);
    }

    public void StopDraw()
    {
        if (visualizationGO != null)
        {
            MonoBehaviour.Destroy(visualizationGO);
        }
    }

    public bool PointIntersects(Vector2 point)
    {
        if (position.x - size.x / 2 < point.x &&
        position.x + size.x / 2 > point.x &&
        position.y - size.y / 2 < point.y &&
        position.y + size.y / 2 > point.y)
        {
            return true;
        }

        return false;
    }
}
