using UnityEngine;
using System.Collections;

public class NodeInWorld: Node {
    public TerrainType type;
    public IntVector2 owner;
    private bool partOfWalkableArea;
    public override bool walkable
    {
        get {
            return occupied && partOfWalkableArea;
        }
       
    }
    public NodeInWorld(Vector3 _worldPos, IntVector2 _gridIndex, TerrainType _type, IntVector2 _owner) : base(_worldPos, _gridIndex)
    {
        owner = _owner;
        type = _type;
        partOfWalkableArea = type == TerrainType.DEFAULT;
    }
}
