using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReferences : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public HandleFireAction playerActions;
    public PlayerHealth playerHealth;
    public PlayerInventory playerInventory;
    public PlayerSoundsAndExtras playerExtras;
    public PlayerLaststand playerLaststand;

    public bool IsPlayerFocused(bool ignoreMovement = false)
    {
        if(playerActions.isReloading)
        {
            return false;
        }

        if(!playerMovement.IsGrounded())
        {
            return false;
        }

        if(playerMovement.isSliding)
        {
            return false;
        }

        if (playerMovement.isDrinking)
        {
            return false;
        }

        if (playerActions.isKnifing)
        {
            return false;
        }

        if (playerActions.isThrowingGrenade)
        {
            return false;
        }

        if (playerHealth.playerHealth <= 0)
        {
            return false;
        }

        if (playerLaststand.playerInReviveTrigger)
        {
            return false;
        }

        if (!ignoreMovement)
        {
            if (playerMovement.moveValue != 0)
            {
                return false;
            }
        }

        return true;
    }
}
