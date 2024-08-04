using Photon.Pun;
using UnityEngine;

public class PlayerSoundsAndExtras : MonoBehaviour
{
    [Header("Audios")]
    public AudioSource audioSource;
    public AudioClip knifeFlashSound;
    public HandleFireAction playerActions;
    
    [Header("Variables")]
    public bool canPlayerInteract = true;
    public int playerScore = 500;
    public Transform weaponsHolder;

    private void OnTriggerStay(Collider other)
    {
        if(playerActions.isKnifing == true && other.gameObject.CompareTag("Zombie"))
        {
            audioSource.PlayOneShot(knifeFlashSound);
        }
    }
}
