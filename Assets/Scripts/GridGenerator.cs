using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GridGenerator : MonoBehaviour
{
    public GameObject verticalTablePrefab;
    public GameObject horizontalTablePrefab;
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
                Tile tile = Instantiate(tilePrefab, new Vector3(tileSize * i, 0, tileSize * j), Quaternion.Euler(90f, 0f, 0f),transform).GetComponent<Tile>(); // Instantiate tile at position
                int tileType = terrainGridData.TerrainGrid[i][j].TileType;
                SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();

                // Assign texture based on tileType
                if (tileType >= 0 && tileType < textures.Length)
                {
                    renderer.sprite = textures[tileType];
                    if (tileType == 3) 
                    {
                        tile.SetTileTypeToWood();
                    }
                    //renderer.material.mainTexture = textures[tileType];
                }
                else
                {
                    Debug.LogWarning("Invalid TileType: " + tileType);
                }
            }
        }
    }


    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 position = hit.point;
                int x = Mathf.RoundToInt(position.x);
                int z = Mathf.RoundToInt(position.z);

                // Check if the clicked position is a valid location for the table
                if (IsValidTableLocation(x, z))
                {
                    // Spawn the appropriate table at the clicked position
                    if (IsValidVerticalTableLocation(x, z))
                    {
                        GameObject table = Instantiate(verticalTablePrefab, new Vector3(x, 0.073f, z+ 1.047f),Quaternion.EulerRotation(0,45,0) );
                        MarkTilesAsTable(x, z);
                    }
                    else if (IsValidHorizontalTableLocation(x, z))
                    {
                        GameObject table = Instantiate(horizontalTablePrefab, new Vector3(x, 0.073f, z+1.047f), Quaternion.EulerRotation(0, 45, 0));
                        MarkTilesAsTable(x, z);
                    }
                }
            }
        }
    }

    bool IsValidTableLocation(int x, int z)
    {
        // Check if the clicked position is within the bounds of the grid
        if (x >= 0 && x < terrainGridData.TerrainGrid.Count &&
            z >= 0 && z < terrainGridData.TerrainGrid[x].Count)
        {
            // Check if the tile at the clicked position is not occupied by a table
            if (!terrainGridData.TerrainGrid[x][z].HasTable)
            {
                return true;
            }
        }
        return false;
    }

    bool IsValidVerticalTableLocation(int x, int z)
    {
        // Check if there is space for a vertical table (requires two tiles)
        return IsValidTableLocation(x, z) && IsValidTableLocation(x, z + 1);
    }

    bool IsValidHorizontalTableLocation(int x, int z)
    {
        // Check if there is space for a horizontal table (requires two tiles)
        return IsValidTableLocation(x, z) && IsValidTableLocation(x + 1, z);
    }

    void MarkTilesAsTable(int x, int z)
    {
        // Mark the tiles at the clicked position and its adjacent position as having a table
        terrainGridData.TerrainGrid[x][z].HasTable = true;
        if (IsValidTableLocation(x, z + 1))
        {
            terrainGridData.TerrainGrid[x][z + 1].HasTable = true;
        }
        if (IsValidTableLocation(x + 1, z))
        {
            terrainGridData.TerrainGrid[x + 1][z].HasTable = true;
        }
    }

}


public class TerrainGridData
{
    public List<List<Tile>> TerrainGrid { get; set; }
}
