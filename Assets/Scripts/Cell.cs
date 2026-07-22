using System;
using UnityEngine;

public class Cell
{
    public bool isUsable;
    public enum CellType
    {
        floor,
        wall,
        na
    }

    public CellType cellType;
}
