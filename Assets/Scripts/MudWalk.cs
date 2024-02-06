using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudWalk : MonoBehaviour
{
    private float zombieTempSpeed;
    private float playerTempSpeed;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            playerTempSpeed = player.GetComponent<PlayerMovement>().movementSpeed;
            player.GetComponent<PlayerMovement>().movementSpeed /= 2;
        }
        else if (collision.gameObject.CompareTag("Zombie"))
        {
            GameObject zombie = collision.gameObject;
            zombieTempSpeed = zombie.GetComponent<ZombieMovements>().zombieSpeed;
            zombie.GetComponent<ZombieMovements>().zombieSpeed /= 2;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            player.GetComponent<PlayerMovement>().movementSpeed = playerTempSpeed;
        }
        else if (collision.gameObject.CompareTag("Zombie"))
        {
            GameObject zombie = collision.gameObject;
            zombie.GetComponent<ZombieMovements>().zombieSpeed = zombieTempSpeed;
        }
    }
}
