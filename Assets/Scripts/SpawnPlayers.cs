using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    [Header("Player Spawning variables")]
    public GameObject[] players;
    public Transform playersHolder;
    public float minX, maxX, minY, maxY;

    private void Start()
    {
        Vector2 randomPosition = new(Random.Range(minX, maxX), Random.Range(minY, maxY));
        GameObject spawnedPlayer = PhotonNetwork.Instantiate(players[Random.Range(0, players.Length)].name, randomPosition, Quaternion.identity);
        spawnedPlayer.transform.SetParent(playersHolder);
        spawnedPlayer.SetActive(true);
        spawnedPlayer.transform.SetSiblingIndex(0);
    }
}
