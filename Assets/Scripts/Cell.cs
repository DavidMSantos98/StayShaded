using System;
using UnityEngine;

public class Cell
{
    public bool isWalkable;
    public enum CellType
    {
        floor,
        wall,
        na
    }

    public CellType cellType;
    public GameObject cellObject; // Reference to the GameObject representing this cell
    public Vector2 gridPosition; // Position of the cell in the grid

    public void AssingSortingLayer()
    {
        switch(cellType)
        {
            case CellType.floor:
                cellObject.GetComponent<SpriteRenderer>().sortingLayerName = "FloorTiles";
                break;
            case CellType.wall:
                cellObject.GetComponent<SpriteRenderer>().sortingLayerName = "WallTiles";
                break;
            case CellType.na:
                cellObject.GetComponent<SpriteRenderer>().sortingLayerName = "FloorTiles";
                break;
        }
    }
}
