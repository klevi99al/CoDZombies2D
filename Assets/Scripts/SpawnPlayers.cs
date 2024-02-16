using Unity.Netcode;
using UnityEngine;

public class SpawnPlayers : NetworkBehaviour
{
    [Header("Player Spawning variables")]
    public GameObject[] players;
    public Transform playersHolder;
    public float minX, maxX, minY, maxY;

    [Header("Testing Network")]
    public GameObject richtofen;
    public GameObject nikolai;
    public GameObject takeo;
    public Transform[] playerSpawnPoints = new Transform[4];

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

        foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList) 
        {
            HandleClientConnected(client.ClientId);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;

            
        }
    }

    private void HandleClientConnected(ulong clientId)
    {

    }

    private void HandleClientDisconnected(ulong clientId)
    {

    }
}
