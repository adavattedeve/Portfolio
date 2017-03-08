using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGeneration : MonoBehaviour {
	//params
	public int segmentsX=3;
	public int segmentsY= 3;
	public int segmentSizeX=50;
	public int segmentSizeY=50;
	public float tileSize=1f;

    public TemplateMapData genDataForNewMap;

    public AreaTypeData[] areaTypeDataArray;
    private Dictionary<AreaType, AreaTypeData> areaTypeDatas;

	//params from some database
	public Material vertexColorMaterial;

    //vars
    private MapGenerationData mapGenData;
    private WorldGrid grid;

    void Awake(){
        grid = GetComponent<WorldGrid>();
        areaTypeDatas = new Dictionary<AreaType, AreaTypeData>();
        for (int i = 0; i < areaTypeDataArray.Length; ++i) {
            if (!areaTypeDatas.ContainsKey(areaTypeDataArray[i].type))
                areaTypeDatas.Add(areaTypeDataArray[i].type, areaTypeDataArray[i]);
        }
	}

	void Start () {
		GenerateMap ();
	}

	// add param Player 
	public void GenerateMap(){
        if (mapGenData == null) {
            mapGenData = new MapGenerationData(segmentSizeX, segmentSizeY, segmentsX , segmentsY );
            genDataForNewMap.GenerateData(ref mapGenData, areaTypeDatas);
            grid.CreateGrid(mapGenData, tileSize / 2);
        }
		GenerateMapMesh (mapGenData);
        foreach (AreaTypeData areaType in areaTypeDatas.Values)
        {
            areaType.GenerateForests(grid);
            areaType.GenerateRiverMesh(grid);
        }
    }
	//area types, difficulties, starting areas
	//private AreaGenerationData[,] GenerateBaseDataForAreas(){
	//	AreaGenerationData[,] returnObject = new AreaGenerationData[areasAmountX,areasAmountZ];
 //       IAreaDataGeneration[,] dataGenerators = new IAreaDataGeneration[areasAmountX, areasAmountZ];
 //       //Logic for area types, difficulties, starting area flags
 //       for (int z=0; z<returnObject.GetLength(1); ++z)
 //       {
	//		for (int x=0; x<returnObject.GetLength(0); ++x)
 //           {
	//			returnObject[x,z] = new AreaGenerationData();
	//			returnObject[x,z].type = AreaType.DEFAULT;
 //               dataGenerators[x,z] = areaDataGenerators[returnObject[x, z].type][Random.Range(0, areaDataGenerators[returnObject[x, z].type].Length)];

 //           }
	//	}


	//	//Generating heigth data
	//	for (int z=0; z<returnObject.GetLength(1); ++z)
 //       {
	//		for (int x=0; x<returnObject.GetLength(0); ++x)
 //           {
	//			dataGenerators[x,z].GenerateHeightMap(ref returnObject[x,z], areaSizeX, areaSizeZ);
	//		}
	//	}
 //       //Welding HeighData
 //       for (int z = 0; z < returnObject.GetLength(1); ++z)
 //       {
 //           for (int x = 0; x < returnObject.GetLength(0); ++x)
 //           {
 //               dataGenerators[x, z].GenerateHeightMap(ref returnObject[x, z], areaSizeX, areaSizeZ);
 //           }
 //       }
 //       //Generating color data
 //       for (int z = 0; z < returnObject.GetLength(1); ++z)
 //       {
 //           for (int x = 0; x < returnObject.GetLength(0); ++x)
 //           {
 //               dataGenerators[x, z].GenerateColorData(ref returnObject[x, z], areaSizeX, areaSizeZ);
 //           }
 //       }

 //       return returnObject;
	//}
	private void GenerateMapMesh (MapGenerationData mapGenData){


        for (int y = 0; y < mapGenData.segmentDatas.GetLength(1); ++y)
        {
            for (int x = 0; x < mapGenData.segmentDatas.GetLength(0); ++x)
            {
                GenerateAreaMesh(mapGenData.segmentDatas[x, y], new Vector3(x*segmentSizeX*tileSize, 0, y*segmentSizeY*tileSize));
            }
        }

    }
    private GameObject GenerateAreaMesh(AreaGenerationData areaData, Vector3 position){
		int tilesAmountX=segmentSizeX;
		int tilesAmountZ=segmentSizeY;
		int numTiles = tilesAmountZ*tilesAmountX;
		int numTris = numTiles * 2;
		
		int vSizeX = tilesAmountX*3;
		int vSizeZ = tilesAmountZ*2;
		int numVerts = vSizeX * vSizeZ;

		// MeshData
		Vector3[] vertices = new Vector3[numVerts];
		Vector3[] normals = new Vector3[numVerts];
		Vector2[] uv = new Vector2[numVerts];
		
		int[] triangles = new int[numTris*3];
		int x, z;
		int index = 0;
		for (z=0; z<tilesAmountZ; ++z) {
			for (x=0; x<tilesAmountX; ++x) {
				//Bot verts
				index= 2*z*vSizeX+x*3;
				vertices[index] = new Vector3(x*tileSize, areaData.heigthMap[x, z], z*tileSize);
				normals[index] =Vector3.up ;
				uv[index] =new Vector2(0,0);

				++index;
				vertices[index] = new Vector3(x*tileSize, areaData.heigthMap[x, z], z*tileSize);
				normals[index] =Vector3.up ;
				uv[index] =new Vector2(0,0);

				++index;
				vertices[index] = new Vector3((x+1)*tileSize, areaData.heigthMap[x+1, z], z*tileSize);
				normals[index] =Vector3.up ;
				uv[index] =new Vector2(1,0);


				//Top verts
				index=(2*z+1)*vSizeX+x*3;
				vertices[index] = new Vector3(x*tileSize, areaData.heigthMap[x, z+1], (z+1)*tileSize);
				normals[index] =Vector3.up ;
				uv[index] =new Vector2(0,1);

				++index;
				vertices[index] = new Vector3((x+1)*tileSize, areaData.heigthMap[x+1, z+1], (z+1)*tileSize);
				normals[index] =Vector3.up ;
				uv[index] =new Vector2(1,1);

				++index;
				vertices[index] = new Vector3((x+1)*tileSize, areaData.heigthMap[x+1, z+1], (z+1)*tileSize);
				normals[index] =Vector3.up ;
				uv[index] =new Vector2(1,1);



            }
		}

		Debug.Log("Done Verts");
		
		int squareIndex;
		int triOffset;

		for (z=0; z<tilesAmountZ; ++z) {
			for (x=0; x<tilesAmountX; ++x) {
				squareIndex = tilesAmountX*z+x;
				triOffset=squareIndex*6;
				//triangle#1
				triangles[triOffset+0] = z*vSizeX*2+x*3+	   0; 
				triangles[triOffset+1] = z*vSizeX*2+x*3+vSizeX+0;
				triangles[triOffset+2] = z*vSizeX*2+x*3+vSizeX+1;

                Vector3 leftTrisNormal = Vector3.Cross(vertices[triangles[triOffset+2]] - vertices[triangles[triOffset + 1]],
   vertices[triangles[triOffset + 0]] - vertices[triangles[triOffset + 1]]).normalized;
//                Debug.Log(leftTrisNormal.ToString());
                normals[triangles[triOffset + 0]] = leftTrisNormal;
                normals[triangles[triOffset + 1]] = leftTrisNormal;
                normals[triangles[triOffset + 2]] = leftTrisNormal;

                //triangle#2
                triangles[triOffset+3] = z*vSizeX*2+x*3+	   1;
				triangles[triOffset+4] = z*vSizeX*2+x*3+vSizeX+2;
				triangles[triOffset+5] = z*vSizeX*2+x*3+	   2;

                Vector3 rightTrisNormal = Vector3.Cross(vertices[triangles[triOffset + 3]] - vertices[triangles[triOffset + 5]],
   vertices[triangles[triOffset + 4]] - vertices[triangles[triOffset + 5]]).normalized;
 //               Debug.Log(rightTrisNormal.ToString());
                normals[triangles[triOffset + 3]] = rightTrisNormal;
                normals[triangles[triOffset + 4]] = rightTrisNormal;
                normals[triangles[triOffset + 5]] = rightTrisNormal;

            }
		}

		Debug.Log ("Triangles Done");


        // Triangle colors
        Color[] vertexColors = new Color[numVerts];
        Debug.Log(vertexColors.Length);
        Debug.Log(areaData.triangleColors.Length);
		for (int i=0; i< areaData.triangleColors.Length; ++i) {
			vertexColors[triangles[i*3]] = areaData.triangleColors[i];
			vertexColors[triangles[i*3+1]] = areaData.triangleColors[i];
			vertexColors[triangles[i*3+2]] = areaData.triangleColors[i];
		}

		//New mesh with created data
		GameObject newArea = new GameObject ();
        newArea.transform.SetParent(transform);
        newArea.transform.position = position;
		MeshRenderer meshRenderer = newArea.AddComponent <MeshRenderer> ();
		meshRenderer.sharedMaterial = vertexColorMaterial;
		Mesh mesh = new Mesh();
		mesh.vertices=vertices;
		mesh.normals=normals;
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.colors = vertexColors;

		MeshFilter filter = newArea.AddComponent<MeshFilter>();
		filter.mesh = mesh;
		
		Debug.Log ("Mesh done!!");
		return newArea;
	}
}
