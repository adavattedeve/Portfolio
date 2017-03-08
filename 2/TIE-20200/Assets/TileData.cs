using UnityEngine;
using System.Collections;
public enum TileID{GRASS, ROUGH}
[System.Serializable]
public class TileData {
	public TileID id;
	public float movementCost=1;
	public GameObject prefab;
	public Texture2D[] normalMaps;
}
