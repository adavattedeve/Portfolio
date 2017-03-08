using UnityEngine;
using System.Collections;

public interface IAreaDataGeneration {
	void GenerateAreaData(ref AreaGenerationData data, int sizeX, int sizeY);
    void GenerateHeightMap(ref AreaGenerationData data, int sizeX, int sizeY);
    void GenerateColorData(ref AreaGenerationData data, int sizeX, int sizeY);
}
