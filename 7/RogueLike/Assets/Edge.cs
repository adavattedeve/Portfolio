using UnityEngine;
using System.Collections.Generic;

public class Edge  {
    private VertexNode node0;
    public VertexNode Node0 { get{ return node0; } }
    private VertexNode node1;
    public VertexNode Node1 { get { return node1; } }

    private Color drawColor = new Color(255, 0, 0, 1);
    public Color DrawColor { set{ drawColor = value; } }

    public float Length { get{ return Vector2.Distance(node0.VertexPos, node1.VertexPos); } }
    private LineRenderer line;

    public Edge(VertexNode _node0, VertexNode _node1)
    {
        node0 = _node0;
        node1 = _node1;
    }

    public bool IsEqual(Edge edge)
    {
        return (node0 == edge.Node0 || node0 == edge.Node1) &&
              (node1 == edge.Node0 || node1 == edge.Node1);
    }

    public bool ContainsVertex(VertexNode vertex)
    {
        return node0 == vertex || node1 == vertex;
    }

    public void Draw()
    {
        if (line == null)
        {
            line = new GameObject().AddComponent<LineRenderer>();
            line.name = "EdgeLine";
            line.material = new Material(Shader.Find("Particles/Additive"));
        }

        line.SetWidth(0.7f, 0.7f);
        line.SetColors(drawColor, drawColor);
        line.SetVertexCount(2);
        line.SetPosition(0, new Vector3(node0.VertexPos.x, node0.VertexPos.y, -3));
        line.SetPosition(1, new Vector3(node1.VertexPos.x, node1.VertexPos.y, -3));
    }

    public void StopDraw()
    {
        if (line != null)
        {
            GameObject.Destroy(line.gameObject);
        }
    }
    public static bool EdgeListContainsEdge(List<Edge> edgeList, Edge edge)
    {
        for (int i = 0; i < edgeList.Count; ++i)
        {
            if (edgeList[i].IsEqual(edge))
            {
                return true;
            }
        }
        return false;
    }
    public static void DrawEdges(List<Edge> edges)
    {
        if (edges == null)
            return;
        for (int i = 0; i < edges.Count; ++i)
        {
            edges[i].Draw();
        }
    }
    public static void DrawEdges(List<Edge> edges, Color color)
    {
        if (edges == null)
            return;
        for (int i = 0; i < edges.Count; ++i)
        {
            edges[i].DrawColor = color;     
            edges[i].Draw();
        }
    }
    public static void StopDrawEdges(List<Edge> edges)
    {
        if (edges == null)
            return;
        for (int i = 0; i < edges.Count; ++i)
        {
            edges[i].StopDraw();
        }
    }
}
