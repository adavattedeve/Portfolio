using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class RiverGenerator {
    [Range(0.1f, 1f)]
    public float heightMpl;
    public Vector2 heightRange;

    public float waterHeight = -0.25f;
    public Color waterColor;
    public Material waterMaterial;

    public void GenerateRiverHeightData(ref MapGenerationData data, List<IntVector2> owners)
    {

        for (int y = 0; y < data.vertexTerrainType.GetLength(1); ++y)
        {
            for (int x = 0; x < data.vertexTerrainType.GetLength(0); ++x)
            {
                if (data.vertexTerrainType[x, y] == TerrainType.WATER && owners.Contains(data.vertexAreaOwner[x, y]))
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
    public void GenerateRiverMesh(WorldGrid grid, List<IntVector2> owners)
    {
        //verts trists, trisColors, uvs, normals
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        List<int> tris = new List<int>();
        List<Color> vertColors = new List<Color>();
        int vertexCounter = 0;
        for (int y = 0; y < grid.GridSize.y; ++y)
        {
            for (int x = 0; x < grid.GridSize.x; ++x)
            {
                if (grid.GetGridAsWorldNodes[x, y].type == TerrainType.WATER && owners.Contains(grid.GetGridAsWorldNodes[x, y].owner))
                {
                    //firstTris
                    verts.Add(new Vector3(x*grid.NodeDiameter,waterHeight , y * grid.NodeDiameter));
                    normals.Add(Vector3.up);
                    //uvs.Add();
                    vertColors.Add(waterColor);
                    tris.Add(vertexCounter);
                    ++vertexCounter;

                    verts.Add(new Vector3(x * grid.NodeDiameter, waterHeight, (y + 1) * grid.NodeDiameter));
                    normals.Add(Vector3.up);
                    //uvs.Add();
                    vertColors.Add(waterColor);
                    tris.Add(vertexCounter);
                    ++vertexCounter;

                    verts.Add(new Vector3((x + 1) * grid.NodeDiameter, waterHeight, (y + 1) * grid.NodeDiameter));
                    normals.Add(Vector3.up);
                    //uvs.Add();
                    vertColors.Add(waterColor);
                    tris.Add(vertexCounter);
                    ++vertexCounter;

                    //Second tris
                    verts.Add(new Vector3(x * grid.NodeDiameter, waterHeight, y * grid.NodeDiameter));
                    normals.Add(Vector3.up);
                    //uvs.Add();
                    vertColors.Add(waterColor);
                    tris.Add(vertexCounter);
                    ++vertexCounter;

                    verts.Add(new Vector3((x + 1) * grid.NodeDiameter, waterHeight, (y + 1) * grid.NodeDiameter));
                    normals.Add(Vector3.up);
                    //uvs.Add();
                    vertColors.Add(waterColor);
                    tris.Add(vertexCounter);
                    ++vertexCounter;

                    verts.Add(new Vector3((x + 1) * grid.NodeDiameter, waterHeight, y * grid.NodeDiameter));
                    normals.Add(Vector3.up);
                    //uvs.Add();
                    vertColors.Add(waterColor);
                    tris.Add(vertexCounter);
                    ++vertexCounter;

                }

            }
        }

        //New mesh with created data
        GameObject water = new GameObject();
        water.transform.position = Vector3.zero;
        MeshRenderer meshRenderer = water.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = waterMaterial;
        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.colors = vertColors.ToArray();

        MeshFilter filter = water.AddComponent<MeshFilter>();
        filter.mesh = mesh;
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
                if (!(terrainTypes[x + xOff, y + yOff] == TerrainType.WATER))
                {
                    return false;
                }

            }
        }
        return true;
    }

}
