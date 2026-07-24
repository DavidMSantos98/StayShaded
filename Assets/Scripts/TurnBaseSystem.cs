using UnityEngine;

public class TurnBaseSystem : MonoBehaviour
{
    [SerializeField] private float timeForTurn;
    private float turnTime;
    [SerializeField] private float timeForEnemyTurn;

    public enum gameState
    {
        START,PLAYERTURN,ENEMYACTION,PLAYERACTION,WON,LOSS
    }

    public gameState currentGameState;

    void Start()
    {
        currentGameState= gameState.START;
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
           Debug.Log("Enemy is acting");
           currentGameState=gameState.PLAYERACTION;
        }

        if(currentGameState == gameState.PLAYERACTION)
        {
            Debug.Log("Player is acting");
            if(checkForWinCondition())
            {
                currentGameState = gameState.WON;
                Debug.Log("Player Won");
            }
            else
            {
                currentGameState = gameState.PLAYERTURN;
            }
        }
    }

    void checkPlayerAction()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Moving Left");
            changeToEnemyTurn();
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Moving Right");
            changeToEnemyTurn();
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Moving Up");
            changeToEnemyTurn();
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Moving Down");
            changeToEnemyTurn();
        }
    }

    void changeToEnemyTurn()
    {
        currentGameState = gameState.ENEMYACTION;
        turnTime = 0;
        Debug.Log("Enemy Turn");
    }

    private bool checkForWinCondition()
    {
        return true;
    }
}
