using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DelunayTriangulation  {

    private bool isDone = false;
    public bool IsDone {get{ return isDone; } }

    //All the triangles in the triangulation
    private List<Triangle> triangles = new List<Triangle>();

    //Verticies that still need to be added to the triangulations
    private List<VertexNode> toAddList = new List<VertexNode>();

    //the current verticie that is being added to the triangulation
    private VertexNode nextNode = null;

    //Edges that have become possibly illegal due to the insertion of another verticie
    private List<Edge> dirtyEdges = new List<Edge>();

    //controls if click control should be allowed
    [SerializeField]
    private bool doStep = true;
    //controls if the algorithum should animate the process step by step
    [SerializeField]
    private bool animate = true;
    //time between steps
    [SerializeField]
    private float animateTime = 0.5f;
    private float animateTimer = 0;

    private bool canContinue = false;

    private bool visualizationEnabled = true;
    private bool doInStages { get { return (animate || doStep) && visualizationEnabled; } }
    //current stage the algorithum is at (used for step and animate control)
    int stage = 0;

    //the omega triangle created at start of triangulation
    private Triangle rootTriangle;

    //the triangle the "nextNode" is inside of
    private Triangle inTriangle;

    private List<Edge> finalTriangulation = new List<Edge>();
    public List<Edge> FinalTriangulation { get{ return finalTriangulation; } }

    public void Init(List<VertexNode> roomNodes, Vector2 boundingBoxMin, Vector2 boundingBoxMax, bool _visualizationEnabled)
    {
        visualizationEnabled = _visualizationEnabled;
        //puts all verticies into the toDo list
        for (int i = 0; i < roomNodes.Count; ++i)
        {
            toAddList.Add(roomNodes[i]);
        }

        float h = Mathf.Abs(boundingBoxMax.y - boundingBoxMin.y);   //height of the bounding box
        float w = Mathf.Abs(boundingBoxMax.x - boundingBoxMin.x);   //width of the bounding box

        //creates three artificial verticies for the root triangle
        VertexNode node0 = new VertexNode(boundingBoxMin.x + w / 2, boundingBoxMin.y + 2 * h, null);

        VertexNode node1 = new VertexNode(boundingBoxMin.x - w / 2, boundingBoxMin.y - 1, null);

        VertexNode node2 = new VertexNode(boundingBoxMin.x + w * 1.5f, boundingBoxMin.y - 1, null);

        rootTriangle = new Triangle(new Edge(node0, node1), new Edge(node0, node2), new Edge(node1, node2));

        if (visualizationEnabled)
            rootTriangle.DrawTriangle();

        //adds the root triangle to the triangle list
        triangles.Add(rootTriangle);
    }

    public void Reset()
    {
        for (int i = 0; i < triangles.Count; ++i)
        {
            triangles[i].StopDraw();
            triangles[i] = null;
        }
        triangles.Clear();
        for (int i = 0; i < finalTriangulation.Count; ++i)
        {
            finalTriangulation[i].StopDraw();
            finalTriangulation[i] = null;
        }
        finalTriangulation.Clear();
        toAddList.Clear();
        isDone = false;
    }
    // Get's called once per frame
    public void Update()
    {
        if (visualizationEnabled)
            DrawTriangles();

        if (!doInStages)
        {
            while (toAddList.Count > 0)
            {
                AddVertexToTriangulation();
            }

            ConstructFinal();
        }
        else if (canContinue)
        {

            animateTimer = 0;
            if (toAddList.Count > 0)
            {
                AddVertexToTriangulation();
            }
            else {
                if (stage != 0)
                {
                    AddVertexToTriangulation();
                    ConstructFinal();
                }
            }
        }
        canContinue = !animate && !doStep;
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

    //Adds a verticies to the triangulation
    private void AddVertexToTriangulation()
    {
        //check what mode the triangulation is running in
        if (stage == 0 || !doInStages)
        {


            //Change the color of all other verticies
            if (visualizationEnabled)
            {
                for (int i = 0; i < toAddList.Count; ++i)
                {
                    toAddList[i].ParentCell.SetColor( new Color(0.5f, 0.5f, 0.5f, 1));
                }
            }
            
            //Find a Random verticie from the todo list.
            nextNode = toAddList[Random.Range(0, toAddList.Count)];
            if (visualizationEnabled)
            {
                nextNode.ParentCell.SetColor(new Color(0.8f, 0.8f, 0.8f));
            }
            //remove selected Vertex from todo list
            toAddList.Remove(nextNode);

            if (doInStages)
            {
                stage++;
                return;
            }
        }
        if (stage == 1 || !doInStages)
        {
            //stores triangles created during the loop to be appended to main list after loop
            List<Triangle> tempTriangles = new List<Triangle>();

            //All edges are clean at this point. Remove any that may be left over from previous loop
            dirtyEdges.Clear();

            float count = -1;
            for (int i = 0; i < triangles.Count; ++i)
            {
                List<Edge> triEdges = triangles[i].Edges;
                count++;
                //Find which triangle the current vertex being add is located within
                if (LineIntersector.PointInTriangle(nextNode.VertexPos, triEdges[0].Node0.VertexPos,
                    triEdges[0].Node1.VertexPos, triEdges[1].Node1.VertexPos))
                {

                    //cache the triangle we are in so we can delete it after loop
                    inTriangle = triangles[i];

                    //create three new triangles from each edge of the triangle vertex is in to the new vertex
                    for (int j = 0; j < triangles[i].Edges.Count; ++j)
                    {
                        Triangle tempTriangle = new Triangle(new Edge(nextNode, triangles[i].Edges[j].Node0),
                                        new Edge(nextNode, triangles[i].Edges[j].Node1),
                                        new Edge(triangles[i].Edges[j].Node1, triangles[i].Edges[j].Node0));

                        //cache created triangles so we can add to list after loop
                        tempTriangles.Add(tempTriangle);

                        //mark the edges of the old triangle as dirty
                        dirtyEdges.Add(new Edge(triangles[i].Edges[j].Node0, triangles[i].Edges[j].Node1));
                    }

                    break;
                }

            }

            //add the three new triangles to the triangle list
            for (int i = 0; i < tempTriangles.Count; ++i)
            {
                triangles.Add(tempTriangles[i]);
            }

            //delete the old triangle that the vertex was inside of
            if (inTriangle != null)
            {
                triangles.Remove(inTriangle);
                if (visualizationEnabled)
                {
                    inTriangle.StopDraw();
                }
                inTriangle = null;
            }

            if (doInStages)
            {
                stage++;
                return;
            }
        }
        if (stage == 2 || !doInStages)
        {
            //recursively check the dirty edges to make sure they are not illegal
            CheckEdges(dirtyEdges);
        }
    }

    private void CheckEdges(List<Edge> edges)
    {
        //stores if a flip occured for mode control
        bool didFlip = false;

        //the current dirty edge
        if (edges.Count == 0)
        {
            stage = 0;
            if (animate || doStep)
            {
                if (toAddList.Count > 0)
                {
                    AddVertexToTriangulation();
                }
            }
            return;
        }

        //get the next edge in the dirty list
        Edge currentEdge = edges[0];

        List<Triangle> connectedTris = new List<Triangle>();

        for (int i = 0; i < triangles.Count; ++i)
        {
            if (triangles[i].ContainsEdge(currentEdge))
            {
                connectedTris.Add(triangles[i]);
            }
        }

        //if ConnectedTris.Count == 1 it is root triangle so no fliping needed 
        if (connectedTris.Count == 2)
        {
            //stores the two verticies from both triangles that arnt on the shared edge
            List<VertexNode> uniqueNodes = new List<VertexNode>();

            //loop through the connected triangles and there edges. Checking for a vertex that isnt in the edge
            for (int i = 0; i < connectedTris.Count; i++)
            {
                foreach (Edge edge in connectedTris[i].Edges)
                {
                    if (!currentEdge.ContainsVertex(edge.Node0))
                    {
                        uniqueNodes.Add(edge.Node0);
                        break;
                    }

                    if (!currentEdge.ContainsVertex(edge.Node1))
                    {
                        uniqueNodes.Add(edge.Node1);
                        break;
                    }
                }
            }


            //find the angles of the two unique verticies
            float angle0 = VertexAngle(uniqueNodes[0].VertexPos,
                                                currentEdge.Node0.VertexPos,
                                                currentEdge.Node1.VertexPos);

            float angle1 = VertexAngle(uniqueNodes[1].VertexPos,
                                                currentEdge.Node0.VertexPos,
                                                currentEdge.Node1.VertexPos);

            //Check if the target Edge needs flipping
            if (angle0 + angle1 > 180)
            {
                didFlip = true;

                //create the new edge after flipped
                Edge flippedEdge = new Edge(uniqueNodes[0], uniqueNodes[1]);

                //store the edges of both triangles in the Quad
                Edge[] firstTriEdges = new Edge[3];
                Edge[] secondTriEdges = new Edge[3];

                VertexNode sharedNode0;
                VertexNode sharedNode1;

                //set the shared nodes on the shared edge
                sharedNode0 = currentEdge.Node0;
                sharedNode1 = currentEdge.Node1;

                //construct a new triangle to update old triangle after flip
                firstTriEdges[0] = new Edge(uniqueNodes[0], sharedNode0);
                firstTriEdges[1] = new Edge(sharedNode0, uniqueNodes[1]);
                firstTriEdges[2] = flippedEdge;

                //construct a new triangle to update the other old triangle after flip
                secondTriEdges[0] = new Edge(uniqueNodes[1], sharedNode1);
                secondTriEdges[1] = new Edge(sharedNode1, uniqueNodes[0]);
                secondTriEdges[2] = flippedEdge;

                //update the edges of the triangles involved in the flip
                connectedTris[0].SetEdges(firstTriEdges[0], firstTriEdges[1], firstTriEdges[2]);
                connectedTris[1].SetEdges(secondTriEdges[0], secondTriEdges[1], secondTriEdges[2]);


                //Adds all edges to be potentially dirty
                for (int i = 0; i < connectedTris.Count; ++i)
                {
                    for (int j = 0; j < connectedTris[i].Edges.Count; ++j)
                    {
                        edges.Add(connectedTris[i].Edges[j]);
                    }
                }

                //also add new edge to dirty list
                edges.Add(flippedEdge);
            }
        }

        //remove the current edge from the dirty list
        edges.Remove(currentEdge);

        if (doStep || animate)
        {
            if (!didFlip)
            {
                CheckEdges(edges);
            }
        }
        else {
            CheckEdges(edges);
        }
    }

    private float VertexAngle(Vector2 target, Vector2 shared0, Vector2 shared1)
    {
        float length0 = Vector2.Distance(target, shared0);
        float length1 = Vector2.Distance(shared0, shared1);
        float length2 = Vector2.Distance(shared1, target);

        return Mathf.Acos(((length0 * length0) + (length2 * length2) - (length1 * length1)) / (2 * length0 * length2)) * Mathf.Rad2Deg;
    }

    private void ConstructFinal()
    {
        for (int i = 0; i < triangles.Count; ++i)
        {
            for (int j = 0; j < triangles[i].Edges.Count; ++j)
            {
                if (triangles[i].Edges[j].Node0.ParentCell != null && triangles[i].Edges[j].Node1.ParentCell != null && !Edge.EdgeListContainsEdge(finalTriangulation, triangles[i].Edges[j]))
                {

                    finalTriangulation.Add(triangles[i].Edges[j]);
                }
                if (visualizationEnabled)
                {
                    triangles[i].Edges[j].StopDraw();
                }              
                
            }
            triangles[i] = null;
        }
        triangles.Clear();
        isDone = true;
    }
    
    private void DrawTriangles()
    {
        if (isDone)
        {
            for (int i = 0; i < finalTriangulation.Count; ++i)
            {
                finalTriangulation[i].Draw();
            }
        }
        else
        {
            for (int i = 0; i < triangles.Count; ++i)
            {
                triangles[i].DrawTriangle();
            }
        }
    }
}
