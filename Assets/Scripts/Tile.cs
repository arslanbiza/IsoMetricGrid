using UnityEngine;

public class Tile : MonoBehaviour
{
    public int TileType { get; set; }
    public bool HasTable { get; set; }


    public TileCategory tileType;
    public bool canPlaceTable;
    private void OnMouseDown()
    {
        if (tileType == TileCategory.Wood) 
        {
            
        }
    }

    public void SetTileTypeToWood() 
    {
        tileType = TileCategory.Wood;
    }
}

public enum TileCategory
{
    Dirt,
    Grass,
    Stone,
    Wood
}


