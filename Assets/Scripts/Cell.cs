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
}
