using System.Collections.Generic;
using System.IO;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Rendering;

public class Grid : MonoBehaviour
{
    private int width, height;
    private int tempWidth;
    [SerializeField]private float cellSize;
    [SerializeField] private GameObject defaultTile;
    private Vector2 originPosition;
    [SerializeField] private int originPositionX, originPositionY;
    private Cell[,] gridArray;
    private Dictionary<Vector2, Cell> gridDictionary;

    [SerializeField] private GameObject[] tilesArray;
    //0 = floor
    //1 = wall
    //3 = na

    [SerializeField] char gridSeperator;

    private string levelData;

    void InstatiateValues()
    {
        originPosition = new Vector2(originPositionX, originPositionY);
        gridDictionary = new Dictionary<Vector2, Cell>();
        levelData = File.ReadAllText(Application.dataPath + "\\LevelsData\\Level1.txt");
        EstablishGridDimensions();

    }

    void EstablishGridDimensions()
    {
        tempWidth = 0;
        width = 0;
        foreach (char c in levelData)
        {
            tempWidth++;
            if (c == gridSeperator)
            {
                height++;
                if (tempWidth > width) { width = tempWidth; }
            }
        }
    }

    void Start()
    {
        InstatiateValues();
        GenerateGrid();
    }

    void TranslateLevelFile()
    {
        gridArray = new Cell[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                gridArray[i, j] = new Cell();
            }
        }

        int x = 0;
        int y = 0;
        foreach (char c in levelData)
        {
            if (c == '0') { gridArray[x, y].cellType = Cell.CellType.floor; }
            if (c == '1') { gridArray[x, y].cellType = Cell.CellType.wall; }
            if (c == gridSeperator)
            {
                for(int i=x; i< width; i++)
                {
                    gridArray[x, y].cellType = Cell.CellType.na;
                }

                y += 1;
            }
        }
    }

    void GenerateGrid()
    {
        TranslateLevelFile();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridArray[x, y] = new Cell();

                Vector2 spawnPoint = new Vector2();
                spawnPoint.x = originPosition.x + (cellSize / 2) * x;
                spawnPoint.y = originPosition.y + (cellSize / 2) * y;
                
                GameObject tilePrefab;
                switch (gridArray[x, y].cellType)
                {
                    case Cell.CellType.floor:
                        tilePrefab = tilesArray[0];
                        break;

                    case Cell.CellType.wall:
                        tilePrefab = tilesArray[1];
                        break;
                    default:
                        tilePrefab = tilesArray[2];
                        break;
                }

                GameObject spawnedTile = Instantiate(tilePrefab, spawnPoint,Quaternion.identity, transform);
                spawnedTile.name = $"Tile {x} , {y}";

                


                gridDictionary[new Vector2(x, y)] = gridArray[x, y];

            }
        }
    }

    public Cell GetCell(Vector2 pos)
    {
        if(gridDictionary.TryGetValue(pos,out var cell))
        {
            return cell;
        }

        return null;
    }


}
