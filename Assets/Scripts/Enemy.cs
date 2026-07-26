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
            
            Cell newCell = path.Dequeue();
            //enemyObject.transform.position = nextCell.cellObject.transform.position;
            enemyPosition = newCell.gridPosition;
            path.Enqueue(newCell); // Add the cell back to the end of the queue for looping
 
    }

    public bool canMove()
    {
        if (path.Count > 0)
        { return true; }
        else {
            Debug.Log("Enemy has no more moves in its path.");
            return false; }
    }
}
