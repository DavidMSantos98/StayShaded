using UnityEngine;

public class Movement : MonoBehaviour
{

    [HideInInspector] public bool isMovinginGrid;
    [HideInInspector] public GameObject movingObject;
    private float movSpeed;
    private Vector2 destination;
    [SerializeField] private float arrivalThreshold; // Threshold to consider the object has arrived at the destination

    private GameObject gameManager;

    void Start()
    {
        movingObject = null;
        isMovinginGrid = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMovinginGrid)
        {
            movingObject.transform.position = Vector2.MoveTowards(movingObject.transform.position,
            destination, movSpeed * Time.deltaTime);

            if (HasArrived())
            {
                Debug.Log(movingObject.name+" can stop now");
                isMovinginGrid = false;

            }
            else
            {
                gameManager.GetComponent<TurnBaseSystem>().agentMoving = false;
            }
        }      
    }

    public void StartMoving(GameObject obj, Cell destination, float movSpeed, GameObject gameManager)
    {
        movingObject = obj;
        isMovinginGrid = true;
        this.movSpeed = movSpeed;
        this.destination = destination.cellObject.transform.position;
        this.gameManager = gameManager;
        this.gameManager.GetComponent<TurnBaseSystem>().agentMoving = true;
        Debug.Log($"Start moving {obj.name} to {destination.cellObject.transform.position}");
    }

    private bool HasArrived()
    {
        if (movingObject.transform.position.x>= destination.x - arrivalThreshold &&
            movingObject.transform.position.x<= destination.x + arrivalThreshold)
        {
            if(movingObject.transform.position.y>= destination.y - arrivalThreshold &&
                movingObject.transform.position.y<= destination.y + arrivalThreshold)
            {
                 return true;
            }
            else {return false; }
        }
        else { return false; }
            
    }
}
