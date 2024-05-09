using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileCategory tileType;
    private void OnMouseDown()
    {
        if (tileType == TileCategory.Wood) 
        {
            Debug.LogError("Wooden Tile Has Been Selected");
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


