using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class MountainGenerator {
    [Range(0.1f, 1f)]public float heightMpl;
    public Vector2 heightRange;

    public void GenerateMountainHeightData(ref MapGenerationData data,  List<IntVector2> owners)
    {

        for (int y = 0; y < data.vertexTerrainType.GetLength(1); ++y)
        {
            for (int x = 0; x < data.vertexTerrainType.GetLength(0); ++x)
            {
                if (data.vertexTerrainType[x, y] == TerrainType.MOUNTAIN && owners.Contains(data.vertexAreaOwner[x, y]))
                {
                    float mpl = 1;
                    if (!IsAllTrueInNeighbour(x, y, data.vertexTerrainType, 2))
                    {
                        mpl *= 0.5f;
                        if (!IsAllTrueInNeighbour(x, y, data.vertexTerrainType))
                        {
                            mpl *= 0.5f;
                        }
                    }
                    data.mapHeightMap[x, y] = Random.Range(heightRange.x, heightRange.y) * mpl;
                }
            }
        }
    }

    private bool IsAllTrueInNeighbour(int x, int y, TerrainType[,] terrainTypes, int range = 1)
    {
        for (int yOff = -range; yOff <= range; ++yOff)
        {
            for (int xOff = -range; xOff <= range; ++xOff)
            {
                if (!(terrainTypes.GetLength(0) > x + xOff && terrainTypes.GetLength(1) > y + yOff) || !(x + xOff >= 0 && y + yOff >= 0))
                {
                    continue;
                }
                if (! (terrainTypes[x + xOff, y + yOff]==TerrainType.MOUNTAIN))
                {
                    return false;
                }

            }
        }
        return true;
    }

 
}
