using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum AreaType{RANDOM=-1, GRASS=0, SNOW=1, DEAD=2}
[System.Serializable]
public class ColorByHeight
{
    public Color color;
    public Vector2 range;
}
[System.Serializable]
public class ColorRange
{
    public TerrainType terrainType;
    public ColorByHeight[] colors;
}

[System.Serializable]
public class AreaTypeData {
    public AreaType type;
    public ColorRange[] colorRanges;
    public MountainGenerator mountainGen;
    public RiverGenerator riverGen;
    public ForestGenerator forestGen;
    private List<IntVector2> owners = new List<IntVector2>();
    public Color GetColorByHeight(TerrainType terrainType, float height){
        for (int i = 0; i < colorRanges.Length; ++i)
        {
            if (colorRanges[i].terrainType == terrainType)
            {
                for (int colorIndex = 0; colorIndex < colorRanges[i].colors.Length; ++colorIndex)
                {
                    if (height >= colorRanges[i].colors[colorIndex].range.x && height <= colorRanges[i].colors[colorIndex].range.y)
                    {
                        return colorRanges[i].colors[colorIndex].color;
                    }
                }
            }
        }
        Debug.Log("Color Not Found. Type: " + terrainType + " height: " + height);
        return Color.red;
    }
    public void AddOwner(IntVector2 owner) {
        owners.Add(owner);
    }
    public void GenerateMountains(ref MapGenerationData data) {
        mountainGen.GenerateMountainHeightData(ref data, owners);
    }
    public void GenerateRiverHeightMaps(ref MapGenerationData data)
    {
        riverGen.GenerateRiverHeightData(ref data, owners);
    }
    public void GenerateRiverMesh(WorldGrid grid)
    {
        riverGen.GenerateRiverMesh(grid, owners);
    }
    public void GenerateForests(WorldGrid grid)
    {
        forestGen.GenerateForests(grid, owners);
    }
}
           
