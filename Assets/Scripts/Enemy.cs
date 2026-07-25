using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public GameObject enemyObject;
    public string pathDirections;
    public Queue<Cell> path=new Queue<Cell>();
    public Vector2 enemyPosition;

    public void MoveNext()
    {
        if (path.Count > 0)
        {
            Cell nextCell = path.Dequeue();
            enemyObject.transform.position = nextCell.cellObject.transform.position;
            enemyPosition = nextCell.gridPosition;
            path.Enqueue(nextCell); // Add the cell back to the end of the queue for looping
        }
        else
        {
                       Debug.Log("Enemy has no more moves in its path.");
        }
    }
}
