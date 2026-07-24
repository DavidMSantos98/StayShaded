using System.Collections.Generic;
using System.IO;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Rendering;

public class Grid : MonoBehaviour
{
    private int width, height;
    private float cellSize;
    [SerializeField] private GameObject defaultTile;
    private Vector2 originPosition;
    [SerializeField] private int originPositionX, originPositionY;
    private Cell[,] gridArray;
    private Dictionary<Vector2, Cell> gridDictionary;

    [SerializeField] private GameObject[] tilesArray;
    //0 = floor
    //1 = wall
    //3 = na

    private char gridSeperator;

    private string levelData;
    private string[] levelDataArray;

    void InstatiateValues()
    {
        originPosition = new Vector2(originPositionX, originPositionY);
        gridDictionary = new Dictionary<Vector2, Cell>();
        levelData = File.ReadAllText(Application.dataPath + "\\LevelsData\\Level1.txt");
        Debug.Log($"Level data read from file: {levelData}");
        levelDataArray = File.ReadAllLines(Application.dataPath + "\\LevelsData\\Level1.txt");
        //levelDataArray = levelData.Split('\n');

        /*
         * for (int i = 0; i < levelDataArray.Length; i++)
        {
            levelDataArray[i].TrimEnd('\n');
            Debug.Log($"Row {i} is {levelDataArray[i]}");
            Debug.Log($"Row {i} is {levelDataArray[i].Length}"+" size");
        }
        */

        //using first sprite in array as measure for cell size
        cellSize = tilesArray[0].GetComponent<SpriteRenderer>().bounds.size.x;
        EstablishGridDimensions();
    }

    void EstablishGridDimensions()
    {
        width = 0;
        int tempWidth = 0;

        height = levelDataArray.Length;

        for (int i = 0; i < levelDataArray.Length; i++)
        {
            
            foreach (char c in levelDataArray[i])
            {
                tempWidth++;
            }
            if (tempWidth > width) { width = tempWidth; }
            tempWidth = 0;
        }
        //height -= 1;
        //width -= 1;
        //To account for array indexing starting at 0, we subtract 1 from the width and height to get the correct dimensions for the grid array.
        //width -= 1;
        //It is counting the new line as a character, so we subtract 1 from the height to get the correct number of rows in the grid array.
        Debug.Log($"Grid dimensions established: width = {width}, height = {height}");
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


            for (int i = 0; i < levelDataArray.Length; i++)
            {
                x = 0;
                foreach (char c in levelDataArray[i])
                {
                //Debug.Log("Length of row "+i +" is "+(levelDataArray[i].Length-1));
                    //Debug.Log($"Translating char {c} at position {x},{i}");
                    if (c == '0') { gridArray[x, i].cellType = Cell.CellType.floor; }
                    if (c == '1') { gridArray[x, i].cellType = Cell.CellType.wall; }
                    x++;
                }

                if (x < width)
                {
                    for (int j = x; j < width; j++)
                    {
                        gridArray[j, i].cellType = Cell.CellType.na;
                    }
                }
            }
        }

        void GenerateGrid()
        {
            TranslateLevelFile();

            for (int x = 0; x < width; x++)
            {
                for (int y = height-1; y >= 0; y--)
                {
                    Vector2 spawnPoint = new Vector2();
                    spawnPoint.x = originPosition.x + (cellSize) * (x + 1);
                    spawnPoint.y = originPosition.y - (cellSize) * (y + 1);

                    GameObject tilePrefab;
                    //Debug.Log("y is " + y);

                    switch (gridArray[x, y].cellType)
                    {
                        case Cell.CellType.floor:
                            tilePrefab = tilesArray[0];
                            //Debug.Log("Assigning floor tile prefab");
                            break;

                        case Cell.CellType.wall:
                            tilePrefab = tilesArray[1];
                            //Debug.Log("Assigning wall tile prefab");
                            break;
                        default:
                            tilePrefab = tilesArray[2];
                            //Debug.Log("Assigning na tile prefab");
                            break;
                    }

                   // Debug.Log($"Spawning tile with type {gridArray[x, y].cellType}");

                    GameObject spawnedTile = Instantiate(tilePrefab, spawnPoint, Quaternion.identity, transform);
                    spawnedTile.name = $"Tile {x} , {y}";

                //not sure if needed, but adding to dictionary for now
                gridDictionary[new Vector2(x, y)] = gridArray[x, y];

                }
            }
        }
    

    public Cell GetCell(int x,int y)
    {
        /*
        //using dictionary to get cell at position, if it exists, otherwise return null
        if (gridDictionary.TryGetValue(pos,out var cell))
        {
            return cell;
        }
        */

        try
        {
            return gridArray[x, y];
        }
        catch
        {
            Debug.Log($"Cell at position {x}, {y} does not exist in grid array.");
            return null;
        }

        
    }


}
