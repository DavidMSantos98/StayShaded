using UnityEngine;

public class Player : MonoBehaviour
{
    public bool hitEnemy = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
       if(collision.gameObject.CompareTag("EnemyVision"))
       {
            Debug.Log("Player collided with Enemy");
            hitEnemy = true;
       }
    }


}
