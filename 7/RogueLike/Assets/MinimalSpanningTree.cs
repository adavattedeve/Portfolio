using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MinimalSpanningTree {
    private List<List<VertexNode>> vertexSets = new List<List<VertexNode>>();
    private List<Color> setColors = new List<Color>();              //Colors for different vertex sets for visualization
    private List<Edge> currentEdges = new List<Edge>();            //Currently existing edges which are not yet chosen to be part of tree
    private List<Edge> finalEdgeSet = new List<Edge>();            //Final edges which constructs the minimal spanning tree
    public List<Edge> FinalEdgeSet { get{ return finalEdgeSet; } }

    private bool isDone = false;
    public bool IsDone { get{ return isDone; } }

    [SerializeField]
    private Color lineColor = new Color(255, 255 ,255);
    public Color LineColor { get{ return lineColor; } }
    [SerializeField]
    private Vector2 setColorRange = new Vector2(0.6f, 1f);

    [SerializeField]
    private bool doStep = false;
    //controls if the algorithum should animate the process step by step
    [SerializeField]
    private bool animate = false;
    //time between steps
    [SerializeField]
    private float animateTime = 0.5f;
    private float animateTimer = 0;
    private bool canContinue = true;

    private bool visualizationEnabled = true;


    public void Init(List<VertexNode> _vertices, List<Edge> _edges, bool _visualizationEnabled)
    {
        visualizationEnabled = _visualizationEnabled;
        // Construct new vertexSet for each of the vertices
        for (int i = 0; i < _vertices.Count; ++i)
        {
            List<VertexNode> newSet = new List<VertexNode>();
            newSet.Add(_vertices[i]);
            if (visualizationEnabled)
            {
                Color setColor = new Color(Random.Range(setColorRange.x, setColorRange.y),
                                       Random.Range(setColorRange.x, setColorRange.y),
                                       Random.Range(setColorRange.x, setColorRange.y));
                _vertices[i].ParentCell.SetColor( setColor);

                setColors.Add(setColor);
            }
            
            vertexSets.Add(newSet);
        }
        currentEdges = new List<Edge>();
        //Reformat edges from shortest to longest
        int shortestIndex = 0;
        float shortest = Mathf.Infinity;
        while (currentEdges.Count != _edges.Count)
        {
            for (int i = 0; i < _edges.Count; ++i)
            {
                if (!Edge.EdgeListContainsEdge(currentEdges, _edges[i]) && _edges[i].Length < shortest)
                {
                    shortestIndex = i;
                    shortest = _edges[i].Length;
                }
            }
            currentEdges.Add(_edges[shortestIndex]);
            shortest = Mathf.Infinity;
        }
    }

    public void Update()
    {
        if (visualizationEnabled)
            Draw();
        //Make whole tree in one frame
        if ((!animate && !doStep) || !visualizationEnabled)
        {
            while (!isDone)
            {
                ProgresOneStep();
            }           
        }
        else if (canContinue)
        {
            
            animateTimer = 0;
            ProgresOneStep();
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

    private void ProgresOneStep()
    {
       
        Edge edge = currentEdges[0];
        currentEdges.RemoveAt(0);
        
        int setIndex0 = 0;  //set index for first vertex
        int setIndex1 = 0;  //set index for second vertex
        for (int i = 0; i < vertexSets.Count; ++i)
        {
            if (vertexSets[i].Contains(edge.Node0))
            {
                setIndex0 = i;
            }
            if (vertexSets[i].Contains(edge.Node1))
            {
                setIndex1 = i;
            }
        }
        if (setIndex0 != setIndex1)
        {
            CombineVertexSets(setIndex0, setIndex1);
            finalEdgeSet.Add(edge);
        }
        if (vertexSets.Count == 1)
        {
            Finished();
        }
    }

    private void CombineVertexSets(int setIndex0, int setIndex1)
    {
        int combineTo = setIndex0;
        int toBeCombined= setIndex1;        
        //Combine smaller set to bigger
        if (vertexSets[setIndex1].Count > vertexSets[setIndex0].Count)
        {
            combineTo = setIndex1;
            toBeCombined = setIndex0;           
        }
        //Add vertices to combineTo set
        for (int i = 0; i < vertexSets[toBeCombined].Count; ++i)
        {
            vertexSets[combineTo].Add(vertexSets[toBeCombined][i]);
            if (visualizationEnabled)
                vertexSets[toBeCombined][i].ParentCell.SetColor( setColors[combineTo]);
        }
        //Remove old and empty set
        vertexSets.RemoveAt(toBeCombined);
        if (visualizationEnabled)
            setColors.RemoveAt(toBeCombined);
    }

    private void Draw()
    {
        for (int i = 0; i < finalEdgeSet.Count; ++i)
        {
            finalEdgeSet[i].DrawColor = lineColor;
            finalEdgeSet[i].Draw();
        }
    }

    private void StopDraw()
    {
        for (int i = 0; i < finalEdgeSet.Count; ++i)
        {
            finalEdgeSet[i].StopDraw();
        }
    }

    public void Reset()
    {
        StopDraw();
        vertexSets.Clear();
        setColors.Clear();
        currentEdges.Clear();
        finalEdgeSet.Clear();
        isDone = false;
    }
    private void Finished()
    {
        
        isDone = true;
        if (visualizationEnabled)
        {
            StopDraw();
            for (int i = 0; i < finalEdgeSet.Count; ++i)
            {
                finalEdgeSet[i].Node0.ParentCell.SetColor(new Color(0.8f, 0.8f, 0.8f));
                finalEdgeSet[i].Node1.ParentCell.SetColor(new Color(0.8f, 0.8f, 0.8f));
            }
        }
            
    }
}
