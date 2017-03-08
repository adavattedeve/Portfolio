using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class ForestGenerator {
    public float forestHeight = 0f;
    [Header("index 0: size=1, etc")]
    public float[] treeSizeChances;
    public TreeGenerator treeGen;
    public void GenerateForests(WorldGrid grid, List<IntVector2> owners)
    {
        
        float totalChances = 0;
        for (int i = 0; i < treeSizeChances.Length; ++i)
        {
            totalChances += treeSizeChances[i];
        }
        bool[,] treeSpawned = new bool[grid.GridSize.x, grid.GridSize.y];
        for (int y = 0; y < grid.GridSize.y; ++y)
        {
            for (int x = 0; x < grid.GridSize.x; ++x)
            {
                treeSpawned[x, y] = false;

            }
        }
                IntVector2 treeSize = new IntVector2(0,0);
        for (int y = 0; y < grid.GridSize.y; ++y)
        {
            for (int x = 0; x < grid.GridSize.x; ++x)
            {
                NodeInWorld node = grid.GetGridAsWorldNodes[x, y];
                if (node.type==TerrainType.FOREST && owners.Contains(node.owner) && !treeSpawned[x, y])
                {
                    GameObject tree=null;
                    float random = Random.Range(0, totalChances);
                    IntVector2 coords = new IntVector2(x, y);

                    for (int i = 0; i < treeSizeChances.Length; ++i)
                    {
                        if (random <= treeSizeChances[i])
                        {
                            treeSize.x = i + 1;
                            treeSize.y = i + 1;
                            break;
                        }
                        random -= treeSizeChances[i];
                    }
                    while (tree == null)
                    {
                        if (treeSize == IntVector2.zero)
                        {
                            Debug.Log("couldn't find good tree");
                            return;
                        }
                        if (AbleToSpawnTree(coords, treeSize, ref grid, ref treeSpawned, ref owners))
                        {
                            tree = treeGen.GetTree(treeSize);
                            if (tree != null)
                            {
                                break;
                            }
                        }

                        treeSize -= IntVector2.one;
                    }
                    for (int yOff = 0; yOff < treeSize.y; ++yOff)
                    {
                        for (int xOff = 0; xOff < treeSize.x; ++xOff)
                        {
                            treeSpawned[x + xOff,y + yOff] = true;
                        }
                    }
                    tree.transform.position = new Vector3(x*grid.NodeDiameter + (treeSize.x-1)*(grid.NodeDiameter/2), forestHeight, y*grid.NodeDiameter + (treeSize.y - 1) * (grid.NodeDiameter / 2));
                    tree.transform.rotation = Quaternion.AngleAxis(Random.Range(-180,180), Vector3.up);
                }
            }
        }
    }
    private bool AbleToSpawnTree(IntVector2 coords, IntVector2 size, ref WorldGrid grid, ref bool[,] treeSpawned, ref List<IntVector2> owners)
    {
        for (int y = coords.y; y < coords.y + size.y; ++y)
        {
            for (int x = coords.x; x < coords.x + size.x; ++x)
            {
                if (!(grid.GridSize.x > x && grid.GridSize.y > y))
                {
                    return false;
                }
                if (!(grid.GetGridAsWorldNodes[x, y].type == TerrainType.FOREST &&
                    owners.Contains(grid.GetGridAsWorldNodes[x, y].owner) && 
                    !treeSpawned[x, y]))
                {
                    return false;
                }
            }
        }
        return true;
    }
}
