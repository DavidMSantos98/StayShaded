using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class TurnBaseSystem : MonoBehaviour
{
    [SerializeField] private float timeForTurn;
    private float turnTime;
    [SerializeField] private float timeForEnemyTurn;
    [SerializeField] private Vector2 playerStartPosition;
    private Vector2 nextPlayerPos;
    private bool playerMovedThisTurn;

    private Vector2 playerCurrentPosition;
    [SerializeField] private Vector2 goalPosition;
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject[] enemyPrefabArray;
    [SerializeField] private GameObject enemyManager;
    [SerializeField] private float enemyMoveSpeed;

    [Header("Path: Up=0 / Right=1 / Down=2  / Left=3 / Stay=4")] 
    [SerializeField] private Vector3[] enemyPositions;

    private Enemy[] enemyList;

    private Movement movementScript;
    private Movement playerMovement;
    public bool agentMoving;

    Queue<GameObject> enemyQueue;
    Queue<GameObject> temporaryEnemyQueue;
    public enum gameState
    {
        START, PLAYERTURN, ENEMYTURN, ENEMYACTION, PLAYERACTION, WON, LOSS
    }

    public gameState currentGameState;

    void Start()
    {
        enemyQueue= new Queue<GameObject>();
        temporaryEnemyQueue= new Queue<GameObject>();
        agentMoving = false;
        playerMovement = player.GetComponent<Movement>();
        movementScript =transform.GetComponent<Movement>();
        playerMovedThisTurn = false;
        currentGameState = gameState.START;
        SnapToCell(grid.GetCell(playerStartPosition));
        playerCurrentPosition = playerStartPosition;
        enemyList = new Enemy[enemyPositions.Length];
        GenerateEnemies();
    }

    void GenerateEnemies()
    {

        int index = 0;
        foreach (Vector3 enemyCord in enemyPositions)
        {
            if (grid.isCellReal((Vector2)enemyPositions[index]) && grid.GetCell((Vector2)enemyPositions[index]).isWalkable)
            {
                enemyList[index] = new Enemy();
                enemyList[index].enemyPosition = enemyCord;

                //currently only using 1 enemy prefab, but can be expanded to use more
                GameObject enemyObject = Instantiate(enemyPrefabArray[0],
                                        grid.GetCell(enemyCord).cellObject.transform.position, Quaternion.identity);
                enemyObject.name = $"Enemy {index}";
                enemyObject.transform.SetParent(enemyManager.transform);

                enemyList[index].enemyObject = enemyObject;
                enemyList[index].pathDirections= enemyPositions[index].z.ToString();
                enemyQueue.Enqueue(enemyObject);
            }
            else
            {
                Debug.Log($"Enemy {index} in {enemyPositions[index]} is in an illegal position");
            }
            index++;
        }

        CreateEnemyPath();
    }

    void CreateEnemyPath()
    {

        Vector2 currentPos;
        Vector2 nextPos;
        int index = 0;

        foreach (Enemy enemy in enemyList)
        {
            currentPos = enemyList[index].enemyPosition;

            foreach (char direction in enemy.pathDirections)
            {
                nextPos = currentPos;
                switch (direction)
                {
                    case '0': // Up
                        nextPos.y -= 1;
                        break;
                    case '1': // Right
                        nextPos.x += 1;
                        break;
                    case '2': // Down
                        nextPos.y += 1;
                        break;
                    case '3': // Left
                        nextPos.x -= 1;
                        break;
                    case '4': // Stay
                        break;
                }
                if (grid.isCellReal(nextPos) && grid.GetCell(nextPos).isWalkable)
                {
                    enemy.path.Enqueue(grid.GetCell(nextPos));
                    currentPos = nextPos; // Update the enemy's position for the next move
                }
                else
                {
                    Debug.Log($"Enemy at {enemy.enemyPosition} cannot move to {nextPos}. It's either out of bounds or not walkable.");
                }
            }
            index++;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!agentMoving)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                currentGameState = gameState.PLAYERTURN;
                turnTime = 0;
            }

            if (currentGameState == gameState.PLAYERTURN)
            {
                checkPlayerAction();

                if (currentGameState == gameState.PLAYERTURN)
                {
                    turnTime += Time.deltaTime;

                    if (turnTime >= timeForTurn)
                    {
                        changeToEnemyTurn();
                    }
                }

            }
        }

        if(currentGameState == gameState.ENEMYTURN)
        {
            foreach (Enemy enemy in enemyList)
            {
                
            }
        }


        if(currentGameState == gameState.ENEMYACTION)
        {
            foreach (Enemy enemy in enemyList)
            {

                
                    if (enemy.canMove())
                    {

                        //movementScript.StartMoving(enemy.enemyObject, enemy.path.Peek(), enemyMoveSpeed);
                        //movementScript.isMovinginGrid = true;
                        Movement enemyMovement = enemy.enemyObject.GetComponent<Movement>();
                        enemyMovement.StartMoving(enemy.enemyObject, enemy.path.Peek(), enemyMoveSpeed, gameObject);
                        agentMoving = true;
                        StartCoroutine(MovingAgent(enemyMovement));
                        enemy.MoveNext();
                    }                  
    
            }

            if (!agentMoving)
            {

                if (playerMovedThisTurn)
                {
                    currentGameState = gameState.PLAYERACTION;
                }
                else
                {
                    CheckWinLoseCon();
                    if (currentGameState != gameState.LOSS || currentGameState != gameState.WON)
                    {
                        currentGameState = gameState.PLAYERTURN;
                    }

                }
            }
            
        }


        if(currentGameState == gameState.PLAYERACTION)
        {
            if (!agentMoving)
            {
                //SnapToCell(grid.GetCell(nextPlayerPos));
                //movementScript.StartMoving(player, grid.GetCell(nextPlayerPos), enemyMoveSpeed);
                playerMovement.StartMoving(player, grid.GetCell(nextPlayerPos), enemyMoveSpeed, gameObject);
                agentMoving=true;

                StartCoroutine(MovingAgent(playerMovement));

                playerCurrentPosition = nextPlayerPos;
                playerMovedThisTurn = false;
            }

            if (!agentMoving)
            {
                CheckWinLoseCon();
                if (currentGameState != gameState.LOSS || currentGameState != gameState.WON)
                {
                    currentGameState = gameState.ENEMYACTION;
                }
            }
        }
    }

    void CheckWinLoseCon()
    {
        if (CheckLose())
        {
            currentGameState = gameState.LOSS;
            Debug.Log("Player Lost");
        }
        else
        {
            if (CheckWin())
            {
                currentGameState = gameState.WON;
                Debug.Log("Player Won");
            }
        }
    }

    void checkPlayerAction()
    {
        nextPlayerPos = playerCurrentPosition;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            playerMovedThisTurn = true;
            nextPlayerPos.x -= 1;
            Debug.Log("Moving Left");
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            playerMovedThisTurn = true;
            nextPlayerPos.x += 1;
            Debug.Log("Moving Right");
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            playerMovedThisTurn = true;
            nextPlayerPos.y -= 1;
            Debug.Log("Moving Up");
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            playerMovedThisTurn = true;
            nextPlayerPos.y += 1;
            Debug.Log("Moving Down");
        }

        
            if (nextPlayerPos != playerCurrentPosition)
            {
                if (playerMovedThisTurn)
                {
                    Debug.Log($"Next Player Position: {nextPlayerPos}");
                    if (grid.isCellReal(nextPlayerPos) && grid.GetCell(nextPlayerPos).isWalkable)
                    {
                        changeToEnemyTurn();
                    }
                    else
                    {
                        playerMovedThisTurn = false;
                        Debug.Log("Illegal Move");
                    }
                }
            }

    }

    void changeToEnemyTurn()
    {
        currentGameState = gameState.ENEMYACTION;
        turnTime = 0;
        temporaryEnemyQueue=enemyQueue;
        //Debug.Log("Enemy Turn");
    }

    private bool CheckLose()
    {
        if (player.GetComponent<Player>().hitEnemy)
        {
            return true;
        }
        else { return false; }
    }

    private bool CheckWin()
    {
        if (playerCurrentPosition == goalPosition)
        {
            return true;
        }
        else { return false; }
    }

    private void SnapToCell(Cell cell)
    {
        Vector2 playerPos = cell.cellObject.transform.position;
        playerPos.y-= player.transform.GetComponent<SpriteRenderer>().bounds.size.y/2; // Adjust the player's position to be slightly above the cell
        player.transform.position = playerPos;
        playerCurrentPosition=cell.gridPosition;

    }

    /*IEnumerator MovingAgent(GameObject agent,Cell destination, float movSpeed)
    {
        movementScript.StartMoving(agent, destination, movSpeed);
        yield return new WaitUntil(()=>!movementScript.isMovinginGrid);
      
    }*/

    IEnumerator MovingAgent(Movement mov)
    {
        yield return new WaitUntil(() => !mov.isMovinginGrid);

    }
}
