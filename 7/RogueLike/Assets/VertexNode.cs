using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class VertexNode {
    private List<VertexNode> connectionNodes;
    public List<VertexNode> ConnectionNodes {get { return connectionNodes; } }

    private Cell parentCell;
    public Cell ParentCell  {get { return parentCell; } }

    private Vector2 vertexPos;
    public Vector2 VertexPos {get { return vertexPos; } }

    public VertexNode(float x, float y, Cell _parentCell)
    {
        vertexPos = new Vector2(x, y);
        parentCell = _parentCell;
        connectionNodes = new List<VertexNode>();
    }

    public void SetNodes(VertexNode node0, VertexNode node1)
    {
        connectionNodes.Add(node0);
        connectionNodes.Add(node1);
    }
}
