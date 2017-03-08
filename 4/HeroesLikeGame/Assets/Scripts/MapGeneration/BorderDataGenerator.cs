using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class IntVector2: System.IEquatable<IntVector2> {
    public static IntVector2 zero = new IntVector2(0,0);
    public static IntVector2 one = new IntVector2(1, 1);
    public static IntVector2 rigth = new IntVector2(1, 0);
    public static IntVector2 up = new IntVector2(0, 1);
    public int x;
    public int y;
    public IntVector2(int _x, int _y) {
        x = _x;
        y = _y;
    }

    public static IntVector2 operator +(IntVector2 value1, IntVector2 value2)
    {
        return new IntVector2(value1.x + value2.x, value1.y + value2.y);
    }

    public static IntVector2 operator -(IntVector2 value1, IntVector2 value2)
    {
        return new IntVector2(value1.x - value2.x, value1.y - value2.y);
    }

    public static IntVector2 operator *(IntVector2 value1, int value2)
    {
        return new IntVector2(value1.x * value2, value1.y * value2);
    }

    public static bool operator ==(IntVector2 value1, IntVector2 value2)
    {
        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(value1, value2))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)value1 == null) || ((object)value2 == null))
        {
            return false;
        }

        // Return true if the fields match:
        return value1.x == value2.x && value1.y == value2.y;
    }
    public override string ToString()
    {
        return "x: " + x + "y " + y;
    }
    public static bool operator !=(IntVector2 value1, IntVector2 value2)
    {
        return !(value1 == value2);
    }
    public bool Equals(IntVector2 other)
    {
        if (other == null)
            return false;

        if (x == other.x && y == other.y)
            return true;
        else
            return false;
    }
    public override int GetHashCode()
    {
        return x+y;
    }
    public override bool Equals(System.Object obj)
    {
        if (obj == null)
            return false;

        IntVector2 other = obj as IntVector2;
        if (other == null)
            return false;
        else
            return Equals(other);
    }
}

[System.Serializable]
public class BorderDataGenerator {

    [Range(0f, 0.49f)]public float connectionPointOffset;
    public int pathwayWidth;

    public int maxMountainWidthDiff;
    public Vector2 minMaxMountainWidth;
    public int maxRiverWidthDiff;
    public Vector2 minMaxRiverWidth;
    public int maxForestWidthDiff;
    public Vector2 minMaxForestWidth;

    public TerrainType[] borderType;
    public float[] borderTypeChance;

    private class BorderData{
        public IntVector2 start;
        public IntVector2 end;
        public IntVector2[] path;
        public IntVector2 owner;    //index for area
        public IntVector2 other;    //index for area
        public TemplatePathway pathway;

        public BorderData() {
            pathway = new TemplatePathway();
        }
    }
    private class AreaBorders {
        //borders
        public List<BorderData> n;
        public List<BorderData> e;
        public List<BorderData> s;
        public List<BorderData> w;
        //corner coords
        public IntVector2 nw;
        public IntVector2 ne;
        public IntVector2 sw;
        public IntVector2 se;

        //pathways
        public TemplatePathway[] pathways;
        public AreaBorders() {
            n = new List<BorderData>();
            e = new List<BorderData>();
            s = new List<BorderData>();
            w = new List<BorderData>();
        }
    }

    public void GenerateData(ref MapGenerationData generationData, TemplateMapData predefinedData) {

        //areaSizesAsVertices
        
        int areaY = (generationData.mapHeightMap.GetLength(1) - 1) / predefinedData.areaTemplates.Length;

        //Init areaBorderDatas and set corner points for areas
        List<List<AreaBorders>> areaBorderDatas = new List<List<AreaBorders>>();
        int additionalRowCounter = 0;
        for (int y = 0; y < predefinedData.areaTemplates.Length; ++y) {

            int rowSize = areaY / predefinedData.areaTemplates[y].rowOfAreas.Length;

            for (int row = 0; row < predefinedData.areaTemplates[y].rowOfAreas.Length; ++row)
            {
                if (row >0)
                {
                    additionalRowCounter++;
                }
                areaBorderDatas.Add (new List<AreaBorders>());

                int areaX = (generationData.mapHeightMap.GetLength(0) - 1) / predefinedData.areaTemplates[y].rowOfAreas[row].areaArray.Length;    

                for (int x = 0; x < predefinedData.areaTemplates[y].rowOfAreas[row].areaArray.Length; ++x)
                {
                    AreaBorders areaBorders = new AreaBorders();

                    //nw
                    areaBorders.nw = new IntVector2(x * areaX, y * areaY + rowSize * row + rowSize);
                    //ne
                    areaBorders.ne = new IntVector2(x * areaX + areaX, y * areaY + rowSize * row + rowSize);
                    //sw
                    areaBorders.sw = new IntVector2(x * areaX, y * areaY + rowSize * row);
                    //se
                    areaBorders.se = new IntVector2(x * areaX + areaX, y * areaY + rowSize * row);
                    //Randomisation here ??
                    areaBorders.pathways = predefinedData.areaTemplates[y].rowOfAreas[row].areaArray[x].pathwayTemplates;
                    areaBorderDatas[y + additionalRowCounter].Add(areaBorders);

                }
            }
        }

        //Search for each area other areas connection points and find out interesting koordinates so they can be later examined to generate additional borders.
        //there is also need for informations for where that point was found.
        List<BorderData> borders = new List<BorderData>();
        BorderData newBorder;
        for (int y = 0; y < areaBorderDatas.Count; ++y)
        {
            for (int x = 0; x < areaBorderDatas[y].Count; ++x)
            {
                IntVector2 currentPoint;
                //N
                currentPoint = areaBorderDatas[y][x].nw;
                //Examine nortehrn areas connection points to find out if there are additional borders to add at north
                if (y < areaBorderDatas.Count - 1)
                {
                    int y2 = y + 1;
                    for (int x2 = 0; x2 < areaBorderDatas[y2].Count; ++x2)
                    {
                        //if area2 se corner is between area1 n corners
                        if (areaBorderDatas[y2][x2].se.x > currentPoint.x && areaBorderDatas[y2][x2].se.x < areaBorderDatas[y][x].ne.x)
                        {
                            newBorder = new BorderData();
                            newBorder.owner = new IntVector2(x, y);
                            newBorder.other = new IntVector2(x2, y2);
                            for (int i = 0; i < areaBorderDatas[y][x].pathways.Length; ++i)
                            {
                                if (areaBorderDatas[y][x].pathways[i].to == newBorder.other)
                                {
                                    newBorder.pathway = areaBorderDatas[y][x].pathways[i];
                                    break;
                                }
                            }
                            newBorder.start = currentPoint;
                            newBorder.end = areaBorderDatas[y2][x2].se;
                            newBorder.pathway.unwalkableType = GetRandomBorderType();
                            areaBorderDatas[y][x].n.Add(newBorder);
                            areaBorderDatas[y2][x2].s.Add(newBorder);
                            borders.Add(newBorder);

                            currentPoint = areaBorderDatas[y2][x2].se;

                        }
                        else if (areaBorderDatas[y2][x2].se.x >= areaBorderDatas[y][x].ne.x)
                        {
                            newBorder = new BorderData();
                            newBorder.owner = new IntVector2(x, y);
                            newBorder.other = new IntVector2(x2, y2);
                            for (int i = 0; i < areaBorderDatas[y][x].pathways.Length; ++i)
                            {
                                if (areaBorderDatas[y][x].pathways[i].to == newBorder.other)
                                {
                                    newBorder.pathway = areaBorderDatas[y][x].pathways[i];
                                    break;
                                }
                            }
                            newBorder.start = currentPoint;
                            newBorder.end = areaBorderDatas[y][x].ne;
                            newBorder.pathway.unwalkableType = GetRandomBorderType();
                            areaBorderDatas[y][x].n.Add(newBorder);
                            areaBorderDatas[y2][x2].s.Add(newBorder);
                            borders.Add(newBorder);
                            
                            currentPoint = areaBorderDatas[y][x].ne;
                            break;
                        }
                    }
                }
                else
                {
                    newBorder = new BorderData();
                    newBorder.owner = new IntVector2(x, y);
                    newBorder.other = null;
                    newBorder.start = areaBorderDatas[y][x].nw;
                    newBorder.end = areaBorderDatas[y][x].ne;
                    newBorder.pathway.unwalkableType = GetRandomBorderType();
                    newBorder.pathway.type = ConnectionType.CLOSED;
                    areaBorderDatas[y][x].n.Add(newBorder);
                    borders.Add(newBorder);

                }

                //E
                newBorder = new BorderData();
                newBorder.owner = new IntVector2(x, y);
                newBorder.start = areaBorderDatas[y][x].se;
                newBorder.end = areaBorderDatas[y][x].ne;
                
                if (x == areaBorderDatas[y].Count - 1)
                {
                   
                    newBorder.pathway.type = ConnectionType.CLOSED;
                    newBorder.other = null;
                }
                else
                {
                    newBorder.other = new IntVector2(x+1, y);
                    for (int i = 0; i < areaBorderDatas[y][x].pathways.Length; ++i)
                    {
                        if (areaBorderDatas[y][x].pathways[i].to == newBorder.other)
                        {
                            newBorder.pathway = areaBorderDatas[y][x].pathways[i];
                            break;
                        }
                    }
                }
                newBorder.pathway.unwalkableType = GetRandomBorderType();
                areaBorderDatas[y][x].e.Add(newBorder);
                if (newBorder.other != null)
                {
                    areaBorderDatas[newBorder.other.y][newBorder.other.x].w.Add(newBorder);
                }
                borders.Add(newBorder);

                //S
                if (y == 0)
                {
                    newBorder = new BorderData();
                    newBorder.owner = new IntVector2(x, y);
                    newBorder.other = null;
                    newBorder.start = areaBorderDatas[y][x].sw;
                    newBorder.end = areaBorderDatas[y][x].se;
                    newBorder.pathway.unwalkableType = GetRandomBorderType();
                    newBorder.pathway.type = ConnectionType.CLOSED;
                    areaBorderDatas[y][x].s.Add(newBorder);
                    borders.Add(newBorder);
                }

                //W
                if (x == 0)
                {
                    newBorder = new BorderData();
                    newBorder.owner = new IntVector2(x, y);
                    newBorder.other = null;
                    newBorder.start = areaBorderDatas[y][x].sw;
                    newBorder.end = areaBorderDatas[y][x].nw;
                    newBorder.pathway.unwalkableType = GetRandomBorderType();
                    newBorder.pathway.type = ConnectionType.CLOSED;
                    areaBorderDatas[y][x].w.Add(newBorder);
                    borders.Add(newBorder);
                }
            }
        }

        //Generate paths for Borders (Vector2[])
        for (int i = 0; i < borders.Count; ++i)
        {
            IntVector2 deltaVector = borders[i].end - borders[i].start;
            IntVector2 mainDirection;

            int deltaX = (int)deltaVector.x;
            int deltaY = (int)deltaVector.y;
            if (deltaX >= deltaY)
            {
                mainDirection = new IntVector2(1 , 0); 
                borders[i].path = new IntVector2[deltaX + 1];
            }
            else
            {
                mainDirection = new IntVector2(0 , 1);
                borders[i].path = new IntVector2[deltaY + 1];
            }
            borders[i].path[0] = borders[i].start;
            borders[i].path[borders[i].path.Length - 1] = borders[i].end;

            for (int pathIndex =0; pathIndex < borders[i].path.Length-2; ++pathIndex)
            {
                IntVector2 currentPos = borders[i].path[pathIndex];
                deltaVector = borders[i].end - currentPos;

                IntVector2 nextPos = currentPos + mainDirection;


                if (mainDirection.x == 0)
                {
                    if ((float)(Mathf.Abs(deltaVector.x) + 1) / (borders[i].path.Length - pathIndex - 2) < Random.Range(0f, 1f))
                    {
                        if (Random.Range(0f, 1f) < 0.5)
                        {
                            nextPos.x += 1;
                        }
                        else
                        {
                            nextPos.x -= 1;
                        }

                    }
                    else
                    {
                        if (deltaVector.x > 0)
                        {
                            nextPos.x += 1;
                        }
                        else if (deltaVector.x < 0)
                        {
                            nextPos.x -= 1;
                        }
                    }
                }

               else
                {
                    if ((float)(Mathf.Abs(deltaVector.y) + 1) / (borders[i].path.Length - pathIndex - 2) < Random.Range(0f, 1f))
                    {
                        if (Random.Range(0f, 1f) < 0.5)
                        {
                            nextPos.y += 1;
                        }
                        else
                        {
                            nextPos.y -= 1;
                        }

                    }
                    else
                    {
                        if (deltaVector.y > 0)
                        {
                            nextPos.y += 1;
                        }
                        else if (deltaVector.y < 0)
                        {
                            nextPos.y -= 1;
                        }
                    }
                }
                nextPos.x = Mathf.Clamp(nextPos.x, 0, generationData.mapHeightMap.GetLength(0)-1);
                nextPos.y = Mathf.Clamp(nextPos.y, 0, generationData.mapHeightMap.GetLength(1) - 1);
                borders[i].path[pathIndex + 1] = nextPos;
            }
        }


        // Generate vertex owners.
        for (int y = 0; y < areaBorderDatas.Count; ++y)
        {
            for (int x = 0; x < areaBorderDatas[y].Count; ++x)
            {

                
                int minX = 1000000, minY = 1000000;
                int maxX = -1000000, maxY = -1000000;

                //gen bounding box for area

                //maxY
                for (int borderIndex = 0; borderIndex < areaBorderDatas[y][x].n.Count; ++borderIndex)
                {
                    for (int pathIndex = 0; pathIndex < areaBorderDatas[y][x].n[borderIndex].path.Length; ++pathIndex)
                    {
                        if (areaBorderDatas[y][x].n[borderIndex].path[pathIndex].y > maxY)
                        {
                            maxY = areaBorderDatas[y][x].n[borderIndex].path[pathIndex].y;
                        }
                    }
                }

                //minY
                for (int borderIndex = 0; borderIndex < areaBorderDatas[y][x].s.Count; ++borderIndex)
                {
                    for (int pathIndex = 0; pathIndex < areaBorderDatas[y][x].s[borderIndex].path.Length; ++pathIndex)
                    {
                        if (areaBorderDatas[y][x].s[borderIndex].path[pathIndex].y < minY)
                        {
                            minY = areaBorderDatas[y][x].s[borderIndex].path[pathIndex].y;
                        }
                    }
                }

                //maxX
                for (int borderIndex = 0; borderIndex < areaBorderDatas[y][x].e.Count; ++borderIndex)
                {
                    for (int pathIndex = 0; pathIndex < areaBorderDatas[y][x].e[borderIndex].path.Length; ++pathIndex)
                    {
                        if (areaBorderDatas[y][x].e[borderIndex].path[pathIndex].x > maxX)
                        {
                            maxX = areaBorderDatas[y][x].e[borderIndex].path[pathIndex].x;
                        }
                    }
                }


                //minX
                for (int borderIndex = 0; borderIndex < areaBorderDatas[y][x].w.Count; ++borderIndex)
                {
                    for (int pathIndex = 0; pathIndex < areaBorderDatas[y][x].w[borderIndex].path.Length; ++pathIndex)
                    {
                        if (areaBorderDatas[y][x].w[borderIndex].path[pathIndex].x < minX)
                        {
                            minX = areaBorderDatas[y][x].w[borderIndex].path[pathIndex].x;
                        }
                    }
                }
                //Debug.Log("generating owners for area at index: " + x + " " + y);
                //Debug.Log("x: " + minX + "  " + maxX + "  y: " + minY + " " + maxY);
                // loop every coord inside bounding box
                for (int boxY = minY; boxY < maxY; ++boxY)
                {
                    for (int boxX = minX; boxX < maxX; ++boxX)
                    {
                        if (IsCoordInArea(areaBorderDatas[y][x], new IntVector2(boxX, boxY)))
                        {
                            //Debug.Log("Coord; " + boxX + "  " + boxY + " OWNER:  " + x + "  " + y);
                            generationData.vertexAreaOwner[boxX, boxY] = new IntVector2(x, y);
                        }
                    }
                }
            }
        }
        IntVector2 lastOwner=null;
        for (int y = 0; y < generationData.vertexAreaOwner.GetLength(1); ++y)
        {
            for (int x = 0; x < generationData.vertexAreaOwner.GetLength(0); ++x)
            {
                if (generationData.vertexAreaOwner[x, y] == null)
                {
                    generationData.vertexAreaOwner[x, y] = lastOwner;
                }
                else
                {
                    lastOwner = generationData.vertexAreaOwner[x, y];
                }
            }
        }    
        // Generate whole border with randomised width and pathways depending on predifendData.AreaDatas[].sw.se.nw.ne (templateConnectionData) //generate pathways heret
        // Mark vertex types same time (default, water, mountain, forest)

        for (int i = 0; i < borders.Count; ++i)
        {
            if (borders[i].pathway.type == ConnectionType.OPEN)
            {
                continue;
            }
            
            switch (borders[i].pathway.unwalkableType) {
                case TerrainType.DEFAULT:
                    Debug.Log("BORDER UNWALKABLE TYPE IS DEFAULT!!");
                    break;
                case TerrainType.FOREST:
                    GenerateBorderTypeData(borders[i],minMaxForestWidth,maxForestWidthDiff,ref generationData);
                    break;
                case TerrainType.MOUNTAIN:
                    GenerateBorderTypeData(borders[i], minMaxMountainWidth, maxMountainWidthDiff, ref generationData);
                    break;
                case TerrainType.WATER:
                    GenerateBorderTypeData(borders[i], minMaxRiverWidth, maxRiverWidthDiff, ref generationData);
                    break;
            }

        }
    }

    private bool IsCoordInArea(AreaBorders areaBorders, IntVector2 coord)
    {

        //Check if coord is inside borders

        //N
        for (int borderIndex = 0; borderIndex < areaBorders.n.Count; ++borderIndex)
        {
            for (int pathIndex = 0; pathIndex < areaBorders.n[borderIndex].path.Length; ++pathIndex)
            {
                if (areaBorders.n[borderIndex].path[pathIndex].x == coord.x)
                {
                    if (areaBorders.n[borderIndex].path[pathIndex].y < coord.y)
                    {
                        return false;
                    }
                    break;
                }
            }
        }

        //S
        for (int borderIndex = 0; borderIndex < areaBorders.s.Count; ++borderIndex)
        {
            for (int pathIndex = 0; pathIndex < areaBorders.s[borderIndex].path.Length; ++pathIndex)
            {
                if (areaBorders.s[borderIndex].path[pathIndex].x == coord.x)
                {
                    if (areaBorders.s[borderIndex].path[pathIndex].y > coord.y)
                    {
                        return false;
                    }
                    break;
                }
            }
        }

        //E
        for (int borderIndex = 0; borderIndex < areaBorders.e.Count; ++borderIndex)
        {
            for (int pathIndex = 0; pathIndex < areaBorders.e[borderIndex].path.Length; ++pathIndex)
            {
                if (areaBorders.e[borderIndex].path[pathIndex].y == coord.y)
                {
                    if (areaBorders.e[borderIndex].path[pathIndex].x < coord.x)
                    {
                        return false;
                    }
                    break;
                }
            }
        }
        //W
        for (int borderIndex = 0; borderIndex < areaBorders.w.Count; ++borderIndex)
        {
            for (int pathIndex = 0; pathIndex < areaBorders.w[borderIndex].path.Length; ++pathIndex)
            {
                if (areaBorders.w[borderIndex].path[pathIndex].y == coord.y)
                {
                    if (areaBorders.w[borderIndex].path[pathIndex].x > coord.x)
                    {
                        return false;
                    }
                    break;
                }
            }
        }
        return true;
    }

    private void GenerateBorderTypeData(BorderData border, Vector2 minMaxWidth, int maxWidthDiff,  ref MapGenerationData data)
    {

        IntVector2[] path = border.path;
        IntVector2 delta = path[path.Length - 1] - path[0];
        IntVector2 direction;

        bool pathwayDone = border.pathway.type != ConnectionType.GUARDEDPATH && border.pathway.type != ConnectionType.PATH;
        bool pathwayInProgress = false;

        int pathwayStartIndex = Random.Range(path.Length / 3, 2 * path.Length / 3);

        if (delta.x > delta.y)
        {
            direction = new IntVector2(0, 1);
        }
        else
        {
            direction = new IntVector2(1, 0);
        }

        int lastWidth = (int)((minMaxWidth.y + minMaxWidth.x) / 2);
        for (int i = 0; i < path.Length; ++i)
        {

            if (pathwayStartIndex == i && !pathwayDone)
            {
                pathwayInProgress = true;
            }
            if (pathwayWidth + pathwayStartIndex == i)
            {
                pathwayInProgress = false;
            }

            int width = Random.Range((int)minMaxWidth.x, (int)minMaxWidth.y);
            if (Mathf.Abs(width - lastWidth) > maxWidthDiff)
            {
                width = lastWidth + Mathf.Clamp(width - lastWidth, -maxWidthDiff, maxWidthDiff);
                width = Mathf.Clamp(width, (int)minMaxWidth.x, (int)minMaxWidth.y);
            }
            if (pathwayInProgress)
            {
                width = 0;
            }
            for (int widthOffset = -width; widthOffset <= width; ++widthOffset)
            {
                IntVector2 coord = path[i] + direction * widthOffset;
                coord.x = Mathf.Clamp(coord.x, 0, data.vertexAreaOwner.GetLength(0) - 1);
                coord.y = Mathf.Clamp(coord.y, 0, data.vertexAreaOwner.GetLength(1) - 1);

                data.vertexAreaOwner[coord.x, coord.y] = border.owner;

                if (pathwayInProgress)
                {
                    data.vertexTerrainType[coord.x, coord.y] = TerrainType.DEFAULT;
                }
                else
                {
                    data.vertexTerrainType[coord.x, coord.y] = border.pathway.unwalkableType;
                    
                }
                data.vertexAreaOwner[coord.x, coord.y] = border.owner;
            }
            lastWidth = width;
        }
    }

    private TerrainType GetRandomBorderType()
    {

        float borderTypeChancesSum = 0;
        for (int i = 0; i < borderTypeChance.Length; ++i)
        {
            borderTypeChancesSum += borderTypeChance[i];
        }
        float random = Random.Range(0,borderTypeChancesSum);
        borderTypeChancesSum = 0;
        for (int i = 0; i < borderTypeChance.Length; ++i)
        {
            borderTypeChancesSum += borderTypeChance[i];
            if (random <= borderTypeChancesSum)
            {
                return borderType[i];
            }
        }
        return borderType[0];
    }

}
