using UnityEngine;
using System.Collections;

public class MapGenerationData {
    public TerrainType[,] vertexTerrainType;
    public float[,] mapHeightMap;
    public Color[,] mapTriangleColorData;
    public IntVector2[,] vertexAreaOwner; //contains indexes to areaDatas array
    public AreaGenerationData[,] segmentDatas; //Generation data for segments

    private int segmentSizeX;
    private int segmentSizeY;


    public MapGenerationData(int _segmentSizeX, int _segmentSizeY, int segmentsX, int segmentsY) {
        segmentSizeX = _segmentSizeX;
        segmentSizeY = _segmentSizeY;
        int vertexCountX = segmentsX * segmentSizeX + 1;
        int vertexCountY = segmentsY * segmentSizeY + 1;

        vertexTerrainType = new TerrainType[vertexCountX,vertexCountY];
        mapHeightMap = new float[vertexCountX, vertexCountY];
        vertexAreaOwner = new IntVector2[vertexCountX, vertexCountY];

        mapTriangleColorData = new Color[segmentSizeX * segmentsX * 2, segmentSizeY * segmentsY ];

        segmentDatas = new AreaGenerationData[segmentsX,segmentsY];
        for (int y = 0; y < segmentsY; ++y)
        {
            for (int x = 0; x < segmentsX; ++x)
            {
                segmentDatas[x,y] = new AreaGenerationData(segmentSizeX, segmentSizeY);
            }
        }

    }

    public void SplitMapDataToAreaDatas() {

        for (int y = 0; y < segmentDatas.GetLength(1); ++y)
        {
            for (int x = 0; x < segmentDatas.GetLength(0); ++x)
            {
                MapDataToSegmentData(x,y);
            }
        }

    }

    private void MapDataToSegmentData(int indexX, int indexY) {
        int vertexCountX = segmentSizeX + 1;
        int vertexCountY = segmentSizeY + 1;

        int startingVertexX = indexX * (vertexCountX - 1);
        int startingVertexY = indexY * (vertexCountY - 1);

        for (int y = startingVertexY; y < startingVertexY+vertexCountY; ++y)
        {
            for (int x = startingVertexX; x < startingVertexX+vertexCountX; ++x)
            {
                int localX = x - startingVertexX;
                int localY = y - startingVertexY;
                segmentDatas[indexX, indexY].heigthMap[localX, localY] = mapHeightMap[x,y]; 
            }
        }

        int triangleCountX = segmentSizeX*2;
        int triangleCountY = segmentSizeY;

        int startingTriangleX = indexX * triangleCountX;
        int startingTriangleY = indexY * triangleCountY;

        for (int y = startingTriangleY; y < startingTriangleY + triangleCountY; ++y)
        {
            for (int x = startingTriangleX; x < startingTriangleX + triangleCountX; ++x)
            {
                int localX = x - startingTriangleX;
                int localY = y - startingTriangleY;

                segmentDatas[indexX, indexY].triangleColors[localX + triangleCountX * localY] = mapTriangleColorData[x, y];
            }
        }

    }

}
