using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSoundsAndExtras : MonoBehaviour
{
    [Header("Audios")]
    public AudioSource audioSource;
    public AudioClip knifeFlashSound;
    public HandleFireAction playerActions;
    
    [Header("Variables")]
    public bool canPlayerInteract = true;
    public int playerScore = 500;

    private void OnTriggerStay(Collider other)
    {
        if(playerActions.isKnifing == true && other.gameObject.CompareTag("Zombie"))
        {
            audioSource.PlayOneShot(knifeFlashSound);
        }
    }
}
