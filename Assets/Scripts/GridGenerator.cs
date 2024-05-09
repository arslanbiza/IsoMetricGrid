using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GridGenerator : MonoBehaviour
{
    public GameObject tilePrefab;
    public Sprite[] textures; // Array to hold the textures
    public float tileSize;

    public string jsonText; // JSON data as string

    private TerrainGridData terrainGridData;

    void Start()
    {
        // Deserialize JSON data into objects
        terrainGridData = JsonConvert.DeserializeObject<TerrainGridData>(jsonText);

        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int i = 0; i < terrainGridData.TerrainGrid.Count; i++)
        {
            for (int j = 0; j < terrainGridData.TerrainGrid[i].Count; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(tileSize * i, 0, tileSize * j), Quaternion.Euler(90f, 0f, 0f),transform); // Instantiate tile at position
                int tileType = terrainGridData.TerrainGrid[i][j].TileType;
                SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();

                // Assign texture based on tileType
                if (tileType >= 0 && tileType < textures.Length)
                {
                    renderer.sprite = textures[tileType];
                    //renderer.material.mainTexture = textures[tileType];
                }
                else
                {
                    Debug.LogWarning("Invalid TileType: " + tileType);
                }
            }
        }
    }

}

public class TileData
{
    public int TileType { get; set; }
}

public class TerrainGridData
{
    public List<List<TileData>> TerrainGrid { get; set; }
}
