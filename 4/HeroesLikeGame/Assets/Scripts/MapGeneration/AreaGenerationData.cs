using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaGenerationData {
//	public AreaType type;
	public float[,] heigthMap;
	public Color[] triangleColors;
    public AreaGenerationData(int tilesX, int tilesY) {
        heigthMap = new float[tilesX + 1, tilesY+1];
        triangleColors = new Color[tilesX*tilesY*2];
    }
}