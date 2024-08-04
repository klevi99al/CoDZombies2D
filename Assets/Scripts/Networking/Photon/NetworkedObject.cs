using Photon.Pun;
using UnityEngine;

public class NetworkedObject : MonoBehaviour, IPunInstantiateMagicCallback
{
    // Customize this method for object-specific setup if needed
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if(CompareTag("Zombie"))
        {
            GameObject zombiesHolder = GameObject.FindGameObjectWithTag("ZombiesHolder");
            if (zombiesHolder != null)
            {
                transform.SetParent(zombiesHolder.transform);
                Debug.Log("Parent is set");
            }
            else
            {
                Debug.LogError("ZombiesHolder not found.");
            }
        }
    }
}
