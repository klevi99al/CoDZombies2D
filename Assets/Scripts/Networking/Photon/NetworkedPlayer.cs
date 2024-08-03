using Photon.Pun;
using UnityEngine;

public class NetworkedPlayer : MonoBehaviour, IPunInstantiateMagicCallback
{
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        LevelManager levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        transform.SetParent(levelManager.playersHolder.transform);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            transform.SetSiblingIndex(0);
        }
    }
}
