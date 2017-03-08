using UnityEngine;
using System.Collections.Generic;

public class Triangle  {
    private List<Edge> edges = new List<Edge>();
    public List<Edge> Edges { get { return edges;  } }

    public Color DrawColor
    {
        set
        {
            for (int i = 0; i < edges.Count; ++i)
            {
                edges[i].DrawColor = value;
            }
        }
    }

    public Triangle(Edge edg0, Edge edg1, Edge edg2)
    {
        edges.Add(edg0);
        edges.Add(edg1);
        edges.Add(edg2);
        DrawColor = new Color(255, 0, 0, 1);
    }

    public void DrawTriangle()
    {
        for (int i = 0; i < edges.Count; ++i)
        { 
            edges[i].Draw();
        }
    }

    public void StopDraw()
    {
        for (int i = 0; i < edges.Count; ++i)
        {
            edges[i].StopDraw();
        }
    }


    //Find if this triangle contains a vert
    public bool ContainsVertex(VertexNode vert)
    {
        for (int i = 0; i < edges.Count; ++i)
        {
            if (edges[i].Node0 == vert || edges[i].Node1 == vert)
            {
                return true;
            }
        }

        return false;
    }

    public bool CheckTriangleShareEdge(Triangle tri)
    {
        for (int i = 0; i < tri.Edges.Count; ++i)
        {
            for (int j = 0; j < edges.Count; ++j)
            {
                if (edges[j].IsEqual(tri.Edges[i]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool ContainsEdge(Edge edge)
    {
        for (int i = 0; i < edges.Count; ++i)
        {
            if (edges[i].IsEqual(edge))
            {
                return true;
            }
        }
            
        return false;
    }

    public void SetEdges(Edge edge0, Edge edge1, Edge edge2)
    {
        for (int i = 0; i < edges.Count; ++i)
        {
            edges[i].StopDraw();
        }

        edges[0] = edge0;
        edges[1] = edge1;
        edges[2] = edge2;
    }
}
