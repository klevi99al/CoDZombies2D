using Photon.Pun;
using UnityEngine;

public class NetworkedPlayer : MonoBehaviour, IPunInstantiateMagicCallback
{
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        // Find the LevelManager and set the parent
        LevelManager levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        transform.SetParent(levelManager.playersHolder.transform);

        // Set sibling index based on whether this player is the MasterClient
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            transform.SetSiblingIndex(0); // MasterClient gets index 0
        }
        else
        {
            int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            transform.SetSiblingIndex(playerIndex); // Other players get their ActorNumber index
        }
    }
}
