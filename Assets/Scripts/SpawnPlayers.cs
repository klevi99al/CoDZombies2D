using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    [Header("References")]
    public GameObject[] players;
    public GameObject[] playerSpawnPoints;

    private void Start()
    {
        SpawnAllPlayers();
    }

    private void SpawnAllPlayers()
    {
        int playerCount = PhotonNetwork.PlayerList.Length;

        for (int i = 0; i < playerCount; i++)
        {
            // Calculate spawn position for each player
            Vector3 spawnPosition = playerSpawnPoints[i % playerSpawnPoints.Length].transform.position;

            // Determine if this is the local player's spawn
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                // Spawn the local player's player object at index 0
                GameObject playerObject = PhotonNetwork.Instantiate("Photon/"+players[i].name, spawnPosition, Quaternion.identity);
                playerObject.transform.SetParent(transform);
                playerObject.transform.SetSiblingIndex(0);
            }
            else
            {
                // Spawn for other players with appropriate indexing
                GameObject playerObject = PhotonNetwork.Instantiate("Photon/" + players[i].name, spawnPosition, Quaternion.identity);
                playerObject.transform.SetParent(transform);
                playerObject.transform.SetSiblingIndex(i + 1); // Set index based on player number
            }
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        SpawnAllPlayers();
    }
}
