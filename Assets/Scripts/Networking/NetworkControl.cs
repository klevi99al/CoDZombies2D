using Unity.Netcode;
using UnityEngine;

public class NetworkControl : NetworkBehaviour
{
    public GameObject[] playerObjectsToDisableOnStart;

    public override void OnNetworkSpawn()
    {
        for(int i = 0; i  < playerObjectsToDisableOnStart.Length; i++)
        {
            playerObjectsToDisableOnStart[i].SetActive(false);
        }
    }
}
