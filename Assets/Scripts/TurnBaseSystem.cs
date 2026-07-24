using UnityEngine;

public class TurnBaseSystem : MonoBehaviour
{
    [SerializeField] private float timeForTurn;
    private float turnTime;
    [SerializeField] private float timeForEnemyTurn;
    [SerializeField] private Vector2 playerStartPosition;
    private Vector2 playerCurrentPosition;
    [SerializeField] private Vector2 goalPosition;
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject player;
    public enum gameState
    {
        START,PLAYERTURN,ENEMYACTION,PLAYERACTION,WON,LOSS
    }

    public gameState currentGameState;

    void Start()
    {
        currentGameState= gameState.START;
        PlayerMoveToCell(grid.GetCell(playerStartPosition));
        playerCurrentPosition = playerStartPosition;

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            currentGameState = gameState.PLAYERTURN;
            turnTime = 0;
        }

        if(currentGameState == gameState.PLAYERTURN)
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

        if(currentGameState == gameState.ENEMYACTION)
        {
           //Debug.Log("Enemy is acting");
           currentGameState=gameState.PLAYERACTION;
        }

        if(currentGameState == gameState.PLAYERACTION)
        {
            //Debug.Log("Player is acting");
            if(checkForWinCondition())
            {
                currentGameState = gameState.WON;
                //Debug.Log("Player Won");
            }
            else
            {
                currentGameState = gameState.PLAYERTURN;
            }
        }
    }

    void checkPlayerAction()
    {
        Vector2 nextPlayerPos = playerCurrentPosition;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            nextPlayerPos.x -= 1;
            Debug.Log("Moving Left");
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            nextPlayerPos.x += 1;
            Debug.Log("Moving Right");
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            nextPlayerPos.y -= 1;
            Debug.Log("Moving Up");
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            nextPlayerPos.y += 1;
            Debug.Log("Moving Down");
        }

        if(nextPlayerPos != playerCurrentPosition)
        {
            Debug.Log($"Next Player Position: {nextPlayerPos}");
            if (grid.isCellReal(nextPlayerPos) && grid.GetCell(nextPlayerPos).isWalkable)
            {
                PlayerMoveToCell(grid.GetCell(nextPlayerPos));
                playerCurrentPosition = nextPlayerPos;
                changeToEnemyTurn();
            }
            else
            {
                Debug.Log("Illegal Move");
            }
        }   
    }

    void changeToEnemyTurn()
    {
        currentGameState = gameState.ENEMYACTION;
        turnTime = 0;
        //Debug.Log("Enemy Turn");
    }

    private bool checkForWinCondition()
    {
        return false;
    }

    private void PlayerMoveToCell(Cell cell)
    {
        Vector2 playerPos = cell.cellObject.transform.position;
        playerPos.y-= player.transform.GetComponent<SpriteRenderer>().bounds.size.y/2; // Adjust the player's position to be slightly above the cell
        player.transform.position = playerPos;
        Debug.Log($"Player moved to cell at position {cell.cellObject.transform.position}");
    }
}
