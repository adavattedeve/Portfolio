using UnityEngine;
using System.Collections.Generic;

public class FogOfWar : MonoBehaviour {

    //params
    public Material fowMaterial;
    public int fogResolution = 2;
    private float tileSize = 1f;
    public float fogHeight = 5f;
    [Range(0f, 1f)] public float exploredAlpha = 0.5f;


    private Grid grid;
    private bool[,] explored;
    private bool[] dirtyVerts;
    private MeshFilter fowMeshFilter;
    private Mesh fowMesh;

    public void InitFogOfWar(Grid _grid) {
        grid = _grid;

        int tileAmountX = grid.mapSizeX * fogResolution;
        int tileAmountY = grid.mapSizeY * fogResolution;

        dirtyVerts = new bool[((tileAmountX*fogResolution) + 1) * ((tileAmountY * fogResolution) + 1)];

        explored = new bool[tileAmountX, tileAmountY];
        for (int y = 0; y< tileAmountY; ++y) {
            for (int x = 0; x < tileAmountX; ++x) {
                explored[x, y] = false;
            }
        }


        //Create Mesh
        int numTiles = tileAmountX  * tileAmountY;
        int numTris = numTiles * 2;

        int vSizeX = tileAmountX + 1;
        int vSizeY = tileAmountY + 1;
        int numVerts = vSizeX * vSizeY;

        // MeshData
        Vector3[] vertices = new Vector3[numVerts];
        Vector3[] normals = new Vector3[numVerts];
        Vector2[] uv = new Vector2[numVerts];

        int[] triangles = new int[numTris * 3];
        int index = 0;
        for (int y = 0; y < vSizeY; ++y) {
            for (int x = 0; x < vSizeX; ++x) {
                vertices[index] = new Vector3(x * tileSize / fogResolution - 0.5f, y * tileSize / fogResolution - 0.5f, fogHeight);
                normals[index] = Vector3.up;
                uv[index] = new Vector2(x / (vSizeX - 1), y / (vSizeY - 1));

                dirtyVerts[index] = false;
                ++index;
            }
        }


        int squareIndex;
        int triOffset;
        for (int y = 0; y < tileAmountY; ++y) {
            for (int x = 0; x < tileAmountX; ++x) {

                squareIndex = tileAmountX * y + x;
                triOffset = squareIndex * 6;
                //triangle#1
                triangles[triOffset + 0] = y * (vSizeX) + x + 0;
                triangles[triOffset + 1] = y * (vSizeX) + x + vSizeX + 0;
                triangles[triOffset + 2] = y * (vSizeX) + x + vSizeX + 1;

                Vector3 leftTrisNormal = Vector3.Cross(
                                         vertices[triangles[triOffset + 2]] - vertices[triangles[triOffset + 1]],
                                         vertices[triangles[triOffset + 0]] - vertices[triangles[triOffset + 1]]).normalized;

                normals[triangles[triOffset + 0]] = leftTrisNormal;
                normals[triangles[triOffset + 1]] = leftTrisNormal;
                normals[triangles[triOffset + 2]] = leftTrisNormal;

                //triangle#2
                triangles[triOffset + 3] = y * (vSizeX) + x + 0;
                triangles[triOffset + 4] = y * (vSizeX) + x + vSizeX + 1;
                triangles[triOffset + 5] = y * (vSizeX) + x + 1;

                Vector3 rightTrisNormal = Vector3.Cross(
                                          vertices[triangles[triOffset + 3]] - vertices[triangles[triOffset + 5]],
                                          vertices[triangles[triOffset + 4]] - vertices[triangles[triOffset + 5]]).normalized;

                normals[triangles[triOffset + 3]] = rightTrisNormal;
                normals[triangles[triOffset + 4]] = rightTrisNormal;
                normals[triangles[triOffset + 5]] = rightTrisNormal;

            }
        }



        // Triangle colors
        Color[] vertexColors = new Color[numVerts];
        Color defaultColor = new Color(0, 0, 0, 1f);
        for (int i = 0; i < vSizeX*vSizeY - 1; ++i) {
            vertexColors[i] = defaultColor;
        }

        //New mesh with created data
        GameObject fogOfWar = new GameObject();
        fogOfWar.name = "FogOfWar";
        MeshRenderer meshRenderer = fogOfWar.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = fowMaterial;
        fowMesh = new Mesh();
        fowMesh.vertices = vertices;
        fowMesh.normals = normals;
        fowMesh.uv = uv;
        fowMesh.triangles = triangles;
        fowMesh.colors = vertexColors;

        fowMeshFilter = fogOfWar.AddComponent<MeshFilter>();
        fowMeshFilter.mesh = fowMesh;
    }

    private void CreateFowSegment(int tilesX, int tilesY, int indexOffset, Transform parent) {

        int tileAmountX = tilesX * fogResolution;
        int tileAmountY = tilesY * fogResolution;

        dirtyVerts = new bool[((tileAmountX * fogResolution) + 1) * ((tileAmountY * fogResolution) + 1)];

        //Create Mesh
        int numTiles = tileAmountX * tileAmountY;
        int numTris = numTiles * 2;

        int vSizeX = tileAmountX + 1;
        int vSizeY = tileAmountY + 1;
        int numVerts = vSizeX * vSizeY;

        // MeshData
        Vector3[] vertices = new Vector3[numVerts];
        Vector3[] normals = new Vector3[numVerts];
        Vector2[] uv = new Vector2[numVerts];

        int[] triangles = new int[numTris * 3];
        int index = 0;
        for (int y = 0; y < vSizeY; ++y)
        {
            for (int x = 0; x < vSizeX; ++x)
            {
                vertices[index] = new Vector3(x * tileSize / fogResolution - 0.5f, y * tileSize / fogResolution - 0.5f + transform.position.y, transform.position.z);
                normals[index] = Vector3.up;
                uv[index] = new Vector2(x / (vSizeX - 1), y / (vSizeY - 1));

                dirtyVerts[index] = false;
                ++index;
            }
        }


        int squareIndex;
        int triOffset;
        for (int y = 0; y < tileAmountY; ++y)
        {
            for (int x = 0; x < tileAmountX; ++x)
            {

                squareIndex = tileAmountX * y + x;
                triOffset = squareIndex * 6;
                //triangle#1
                triangles[triOffset + 0] = y * (vSizeX) + x + 0;
                triangles[triOffset + 1] = y * (vSizeX) + x + vSizeX + 0;
                triangles[triOffset + 2] = y * (vSizeX) + x + vSizeX + 1;

                Vector3 leftTrisNormal = Vector3.Cross(
                                         vertices[triangles[triOffset + 2]] - vertices[triangles[triOffset + 1]],
                                         vertices[triangles[triOffset + 0]] - vertices[triangles[triOffset + 1]]).normalized;

                normals[triangles[triOffset + 0]] = leftTrisNormal;
                normals[triangles[triOffset + 1]] = leftTrisNormal;
                normals[triangles[triOffset + 2]] = leftTrisNormal;

                //triangle#2
                triangles[triOffset + 3] = y * (vSizeX) + x + 0;
                triangles[triOffset + 4] = y * (vSizeX) + x + vSizeX + 1;
                triangles[triOffset + 5] = y * (vSizeX) + x + 1;

                Vector3 rightTrisNormal = Vector3.Cross(
                                          vertices[triangles[triOffset + 3]] - vertices[triangles[triOffset + 5]],
                                          vertices[triangles[triOffset + 4]] - vertices[triangles[triOffset + 5]]).normalized;

                normals[triangles[triOffset + 3]] = rightTrisNormal;
                normals[triangles[triOffset + 4]] = rightTrisNormal;
                normals[triangles[triOffset + 5]] = rightTrisNormal;

            }
        }



        // Triangle colors
        Color[] vertexColors = new Color[numVerts];
        Color defaultColor = new Color(0, 0, 0, 1f);
        for (int i = 0; i < vSizeX * vSizeY - 1; ++i)
        {
            vertexColors[i] = defaultColor;
        }

        //New mesh with created data
        GameObject fogOfWar = new GameObject();
        fogOfWar.name = "FogOfWarSegment";
        fogOfWar.transform.SetParent(parent);
        MeshRenderer meshRenderer = fogOfWar.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = fowMaterial;
        fowMesh = new Mesh();
        fowMesh.vertices = vertices;
        fowMesh.normals = normals;
        fowMesh.uv = uv;
        fowMesh.triangles = triangles;
        fowMesh.colors = vertexColors;

        fowMeshFilter = fogOfWar.AddComponent<MeshFilter>();
        fowMeshFilter.mesh = fowMesh;
    }


    public void UpdateFow() {
        Color[] vertexColors = fowMesh.colors;
        
        for (int i=0; i<dirtyVerts.Length; ++i) {
            if (dirtyVerts[i]) {
                Color color = vertexColors[i];
                color.a = GetUpdatedAlpha(i);
                vertexColors[i] = color;
                dirtyVerts[i] = false;
            }
                
        }
        fowMesh.colors = vertexColors;
        fowMeshFilter.mesh = fowMesh;
    }

    //Marks all the verts dirty surrounding tile at (_x, _y) coords
    public void MarkTileDirty(int _x, int _y) {

        if (grid.GetTile(_x, _y).Visible)
            explored[_x, _y] = true;
        int vSizeX = (grid.mapSizeX * fogResolution + 1);
        int vertIndex = _x * fogResolution + (_y * fogResolution) * vSizeX + vSizeX +1;
        
        for (int y= -fogResolution; y<= fogResolution; ++y) {
            for (int x = -fogResolution; x <= fogResolution; ++x) {
                if (vertIndex + x + y * vSizeX >= 0 && vertIndex + x + y * vSizeX < dirtyVerts.Length)
                    dirtyVerts[vertIndex + x + y * vSizeX] = true;
            }
        }
    }
    //Returns updated alpha value of vert
    private float GetUpdatedAlpha(int vertIndex) {

        int vertIndexX = vertIndex % ((grid.mapSizeX * fogResolution) + 1);
        int vertIndexY = vertIndex / ((grid.mapSizeX * fogResolution) + 1);

        int tileX = vertIndexX / fogResolution;
        int tileY = vertIndexY / fogResolution;

        List<Tile> currentTiles = new List<Tile>();

        if (grid.GetTile(tileX, tileY) != null)
            currentTiles.Add(grid.GetTile(tileX, tileY)); 

        bool crossX = vertIndexX % fogResolution == 0;
        bool crossY = vertIndexY % fogResolution == 0;

        if (crossX && grid.GetTile(tileX - 1, tileY) != null)
            currentTiles.Add(grid.GetTile(tileX - 1, tileY));

        if (crossY && grid.GetTile(tileX, tileY - 1) != null)
            currentTiles.Add(grid.GetTile(tileX, tileY - 1));

        if (crossX && crossY && grid.GetTile(tileX - 1, tileY - 1) != null)
            currentTiles.Add(grid.GetTile(tileX - 1, tileY - 1));

        for (int i=0; i< currentTiles.Count; ++i) {
            if (currentTiles[i].Visible)
                return 0f;
        }

        for (int i = 0; i < currentTiles.Count; ++i)
        {
            if (explored[currentTiles[i].X, currentTiles[i].Y])
                return exploredAlpha;
        }

        if (crossX && crossY) {
            Debug.Log("Cross section not visible");
            return 1f;
        }
            
        currentTiles.Clear();

        if (crossX) {
            if (grid.GetTile(tileX - 1, tileY + 1) != null)
                currentTiles.Add(grid.GetTile(tileX - 1, tileY + 1));

            if (grid.GetTile(tileX - 1, tileY - 1) != null)
                currentTiles.Add(grid.GetTile(tileX - 1, tileY - 1));

            if (grid.GetTile(tileX, tileY + 1) != null)
                currentTiles.Add(grid.GetTile(tileX, tileY + 1));

            if (grid.GetTile(tileX, tileY - 1) != null)
                currentTiles.Add(grid.GetTile(tileX, tileY - 1));
        }
        else if (crossY) {
            if (grid.GetTile(tileX + 1, tileY - 1) != null)
                currentTiles.Add(grid.GetTile(tileX + 1, tileY - 1));

            if (grid.GetTile(tileX - 1, tileY - 1) != null)
                currentTiles.Add(grid.GetTile(tileX - 1, tileY - 1));

            if (grid.GetTile(tileX + 1, tileY) != null)
                currentTiles.Add(grid.GetTile(tileX + 1, tileY));

            if (grid.GetTile(tileX - 1, tileY) != null)
                currentTiles.Add(grid.GetTile(tileX - 1, tileY));
        }
        else {
            currentTiles = grid.GetNeighbourTiles(tileX, tileY);
        }

        for (int i = 0; i < currentTiles.Count; ++i)
        {
            if (currentTiles[i].Visible || explored[currentTiles[i].X, currentTiles[i].Y])
                return exploredAlpha;
        }
        return 0;
    }
}
