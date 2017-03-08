using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class TemplateAreaDataArray
{
    public TemplateAreaData[] areaArray;
}
 [System.Serializable]
public class RowOfAreas {
    public TemplateAreaDataArray[] rowOfAreas;
}

[System.Serializable]
public class TemplateMapData {

    [Header("perlin noise params")]
    public float amplitude;
    public float frequency;
    public float startingHeight = 0.4f;
    public Vector2 perlinSeedRange;

    public float randomVariation;

    public RowOfAreas[] areaTemplates;
    [System.NonSerialized]public List<AreaTypeData[]> areaTypeDatas;
    public BorderDataGenerator borderDataGen;

    public void GenerateData(ref MapGenerationData generationData, Dictionary<AreaType, AreaTypeData> areaTypeDataDict) {
        areaTypeDatas = new List<AreaTypeData[]>();
        int additionalRowCounter = 0;
        for (int y = 0; y < areaTemplates.Length; ++y)
        {
            for (int rowY = 0; rowY < areaTemplates[y].rowOfAreas.Length; ++rowY)
            {
                if (rowY>0)
                {
                    additionalRowCounter++;
                }
                areaTypeDatas.Add(new AreaTypeData[areaTemplates[y].rowOfAreas[rowY].areaArray.Length]);
                for (int x = 0; x < areaTypeDatas[y+ additionalRowCounter].Length; ++x)
                {
                    int areaTypesAmount = System.Enum.GetNames(typeof(AreaType)).Length-1;
                    if (areaTemplates[y].rowOfAreas[rowY].areaArray[x].areaType == AreaType.RANDOM) {
                        areaTemplates[y].rowOfAreas[rowY].areaArray[x].areaType = (AreaType)Random.Range(0, areaTypesAmount);
                    }
                    areaTypeDatas[y+ additionalRowCounter][x] = areaTypeDataDict[areaTemplates[y].rowOfAreas[rowY].areaArray[x].areaType];
                    areaTypeDatas[y + additionalRowCounter][x].AddOwner(new IntVector2(x, y + additionalRowCounter));
                }
            }
        }


        borderDataGen.GenerateData(ref generationData, this);

        //Generate height and color data using generationData.VertexOwner and  generationData.vertexTerrainType
        //ForTesting purposes make heightData 0 and colordata based on vertex owner or vertexTerrain type
        float perlinSeed = Random.Range(perlinSeedRange.x, perlinSeedRange.y);
        for (int y = 0; y < generationData.mapHeightMap.GetLength(1); ++y)
        {
            for (int x = 0; x < generationData.mapHeightMap.GetLength(0); ++x)
            {
               
                switch (generationData.vertexTerrainType[x, y])
                {
                    case TerrainType.DEFAULT:

                        generationData.mapHeightMap[x, y] = startingHeight + Mathf.PerlinNoise(x * frequency+ perlinSeed, y * frequency+ perlinSeed) * amplitude + Random.Range(-randomVariation, randomVariation);
                        break;
                    case TerrainType.FOREST:
                        generationData.mapHeightMap[x, y] = startingHeight + Mathf.PerlinNoise(x * frequency+ perlinSeed, y * frequency+ perlinSeed) * amplitude + Random.Range(-randomVariation, randomVariation)*2;
                        break;
                    case TerrainType.MOUNTAIN:
                        break;
                    case TerrainType.WATER:
                       // generationData.mapHeightMap[x, y] = -3;
                        break;
                }

            }
        }
        foreach (AreaTypeData areaType in areaTypeDataDict.Values)
        {
            areaType.GenerateMountains(ref generationData);
            areaType.GenerateRiverHeightMaps(ref generationData);
        }

        for (int y = 0; y < generationData.mapHeightMap.GetLength(1)-1; ++y)
        {
            for (int x = 0; x < generationData.mapHeightMap.GetLength(0)-1; ++x)
            {
                IntVector2 trisOwner = generationData.vertexAreaOwner[x, y];
                if (generationData.vertexAreaOwner[x, y + 1] == generationData.vertexAreaOwner[x + 1, y + 1])
                {
                    trisOwner = generationData.vertexAreaOwner[x, y + 1];
                }
                TerrainType trisType = TerrainType.DEFAULT;
                if (generationData.vertexTerrainType[x, y] == TerrainType.MOUNTAIN ||
                generationData.vertexTerrainType[x, y + 1] == TerrainType.MOUNTAIN ||
                generationData.vertexTerrainType[x + 1, y + 1] == TerrainType.MOUNTAIN)
                {
                    trisType = TerrainType.MOUNTAIN;
                }
                else if (generationData.vertexTerrainType[x, y] == TerrainType.WATER ||
                     generationData.vertexTerrainType[x, y + 1] == TerrainType.WATER ||
                     generationData.vertexTerrainType[x + 1, y + 1] == TerrainType.WATER)
                {
                    trisType = TerrainType.WATER;
                }
                else if (generationData.vertexTerrainType[x, y] == TerrainType.FOREST ||
                     generationData.vertexTerrainType[x, y + 1] == TerrainType.FOREST ||
                     generationData.vertexTerrainType[x + 1, y + 1] == TerrainType.FOREST)
                {
                    trisType = TerrainType.FOREST;
                }
                float trisHeight = (generationData.mapHeightMap[x, y] + generationData.mapHeightMap[x, y + 1] + generationData.mapHeightMap[x + 1, y + 1])/3;

                generationData.mapTriangleColorData[2 * x, y] = areaTypeDatas[trisOwner.y][trisOwner.x].GetColorByHeight(trisType, trisHeight);


                trisOwner = generationData.vertexAreaOwner[x+1, y];
                if (generationData.vertexAreaOwner[x, y] == generationData.vertexAreaOwner[x + 1, y + 1])
                {
                    trisOwner = generationData.vertexAreaOwner[x, y];
                }


                trisType = TerrainType.DEFAULT;
                if (generationData.vertexTerrainType[x, y] == TerrainType.MOUNTAIN ||
                generationData.vertexTerrainType[x+1, y] == TerrainType.MOUNTAIN ||
                generationData.vertexTerrainType[x + 1, y + 1] == TerrainType.MOUNTAIN)
                {
                    trisType = TerrainType.MOUNTAIN;
                }
                else if (generationData.vertexTerrainType[x, y] == TerrainType.WATER ||
                     generationData.vertexTerrainType[x + 1, y] == TerrainType.WATER ||
                     generationData.vertexTerrainType[x + 1, y + 1] == TerrainType.WATER)
                {
                    trisType = TerrainType.WATER;
                }
                else if (generationData.vertexTerrainType[x, y] == TerrainType.FOREST ||
                     generationData.vertexTerrainType[x + 1, y] == TerrainType.FOREST ||
                     generationData.vertexTerrainType[x + 1, y + 1] == TerrainType.FOREST)
                {
                    trisType = TerrainType.FOREST;
                }
                trisHeight = (generationData.mapHeightMap[x, y] + generationData.mapHeightMap[x + 1, y] + generationData.mapHeightMap[x + 1, y + 1]) / 3;

                generationData.mapTriangleColorData[2 * x+1, y] = areaTypeDatas[trisOwner.y][trisOwner.x].GetColorByHeight(trisType, trisHeight);

            }
        }
        //Generate Rivers and trees here !!!
        generationData.SplitMapDataToAreaDatas();
    }
}
