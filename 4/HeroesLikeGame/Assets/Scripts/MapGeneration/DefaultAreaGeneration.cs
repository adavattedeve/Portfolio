using UnityEngine;
using System.Collections;

[System.Serializable]
public class DefaultAreaGeneration : IAreaDataGeneration {

    //params
    public ColorByHeight[] colorRanges;
    public Vector2 defaultAreaHeightRange;
	[Header("MountainParams")]
	public Vector2 thicknesRange;
	public Vector2 heightRange;

	public void GenerateAreaData(ref AreaGenerationData data, int sizeX, int sizeY){
		GenerateHeightMap (ref data, sizeX, sizeY);
		GenerateColorData (ref data, sizeX, sizeY);
	}

    public void GenerateHeightMap(ref AreaGenerationData data, int sizeX, int sizeY)
    {
        Debug.Log("generating heightmap for area");
        float[,] mountainHeightMap = GenerateMountainHeightData(sizeX, sizeY);
        float[,] heightMap = new float[sizeX + 1, sizeY + 1];
        for (int y = 0; y < heightMap.GetLength(1); ++y)
        {
            for (int x = 0; x < heightMap.GetLength(0); ++x)
            {
                if (mountainHeightMap[x, y] != 0)
                {
                    heightMap[x, y] = mountainHeightMap[x, y];
                }
                else {
                    heightMap[x, y] = Random.Range(defaultAreaHeightRange.x, defaultAreaHeightRange.y);
                }
            }
        }
        data.heigthMap = heightMap;
    }

    public void GenerateColorData(ref AreaGenerationData data, int sizeX, int sizeY)
    {
        Debug.Log("generating colordata for area");

        Color[] colorData = new Color[sizeX * sizeY * 2];

        int index = 0;
        float avgHeight = 0;
        for (int y = 0; y < sizeY; ++y)
        {
            for (int x = 0; x < sizeX; ++x)
            {
                index = 2 * x + sizeX * 2 * y;
                //left trist Height
                avgHeight = (data.heigthMap[x, y] + data.heigthMap[x, y + 1] + data.heigthMap[x + 1, y + 1]) / 3;
                for (int i = 0; i < colorRanges.Length; ++i)
                {
                    if (avgHeight.IsInRange(colorRanges[i].range.x, colorRanges[i].range.y))
                    {
                        colorData[index] = colorRanges[i].color;
                    }
                }

                ++index;
                //rigth trist Height
                avgHeight = (data.heigthMap[x, y] + data.heigthMap[x + 1, y + 1] + data.heigthMap[x + 1, y]) / 3;
                for (int i = 0; i < colorRanges.Length; ++i)
                {
                    if (avgHeight.IsInRange(colorRanges[i].range.x, colorRanges[i].range.y))
                    {
                        colorData[index] = colorRanges[i].color;
                    }
                }

            }
        }
        data.triangleColors = colorData;
    }

    private float[,] GenerateMountainHeightData(int sizeX, int sizeY){

		bool[,] isMountain = GenerateMountainAreaData(sizeX, sizeY);
		float [,] mountainHeightMap = new float[sizeX+1,sizeY+1];

		for (int y=0; y<isMountain.GetLength(1); ++y) {
			for (int x=0; x<isMountain.GetLength(0); ++x){
				if (isMountain[x, y]){
                    float mpl = 1;
                    if (!IsAllTrueInNeighbour(x, y, isMountain, 2)) {
                        mpl *= 0.5f;
                        if (!IsAllTrueInNeighbour(x, y, isMountain)) {
                            mpl *= 0.5f;
                        }
                    }
					mountainHeightMap[x, y] = Random.Range(heightRange.x, heightRange.y)*mpl;
				}else {
					mountainHeightMap[x, y] = 0;
				}
			}
		}

		return mountainHeightMap;
	}

    private bool IsAllTrueInNeighbour(int x, int y, bool[,] array, int range=1)
    {
        for (int yOff=-range; yOff <= range; ++yOff)
        {
            for (int xOff = -range; xOff <= range; ++xOff)
            {
                if (!(array.GetLength(0) > x + xOff && array.GetLength(1) > y + yOff) || !(x+xOff>=0 && y+yOff>=0)) {
                    continue;
                }
                if (!array[x + xOff, y + yOff]) {
                    return false;
                }

            }
        }
        return true;
    }

	private bool[,] GenerateMountainAreaData(int sizeX, int sizeY){
		bool [,] isMountain = new bool[sizeX+1,sizeY+1];
		
		int y=0,x=0;
		for (x=0; x<isMountain.GetLength(0); ++x){
			int thicknes = (int)Random.Range(thicknesRange.x, thicknesRange.y);
			for (int i=0; i<thicknes; ++i){
				isMountain[x, y+i] = true;
			}
		}
		y = isMountain.GetLength (1) - 1;
		for (x=0; x<isMountain.GetLength(0); ++x){
			int thicknes = (int)Random.Range(thicknesRange.x, thicknesRange.y);
			for (int i=0; i<thicknes; ++i){
				isMountain[x, y-i] = true;
			}
		}
		x = 0;
		for (y=0; y<isMountain.GetLength(1); ++y){
			int thicknes = (int)Random.Range(thicknesRange.x, thicknesRange.y);
			for (int i=0; i<thicknes; ++i){
				isMountain[x+i, y] = true;
			}
		}
		x = isMountain.GetLength (0) - 1;
		for (y=0; y<isMountain.GetLength(1); ++y){
			int thicknes = (int)Random.Range(thicknesRange.x, thicknesRange.y);
			for (int i=0; i<thicknes; ++i){
				isMountain[x-i, y] = true;
			}
		}
		return isMountain;
	}



    private Color GetColorByHeight(float height) {
        return Color.black;
    }

}
