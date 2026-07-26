using System.Collections.Generic;
using UnityEngine;

public class EnemyOrder : MonoBehaviour
{
    int numberOfEnemies;
    private Queue<Enemy> enemyQueue;
    public bool isIterating;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isIterating = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
