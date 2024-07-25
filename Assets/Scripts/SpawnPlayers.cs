using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPlayers : NetworkBehaviour
{
    [Header("References")]
    public GameObject[] players;
    public GameObject[] playerSpawnPoints;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }


    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsIds.Count; i++)
        {
            ulong clientId = NetworkManager.Singleton.ConnectedClientsIds[i];
            GameObject player = Instantiate(players[i], playerSpawnPoints[i].transform.position, Quaternion.Euler(Vector3.zero), transform);
            player.transform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            player.GetComponent<NetworkObject>().TrySetParent(transform);
        }
    }
}
