using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    [Header("References")]
    public GameObject[] soloPlayers;
    public GameObject[] playerPrefabs;      // Array of player prefabs
    public GameObject[] playerSpawnPoints;  // Array of spawn points

    [SerializeField] private CameraScript cameraScript;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(StaticVariables.isSoloGame)
        {
            Instantiate(soloPlayers[StaticVariables.selectedCharacterIndex], playerSpawnPoints[0].transform.position, Quaternion.Euler(0, 90, 0), transform);
            return;
        }
        
        if (scene.buildIndex == 1 && PhotonNetwork.InRoom)
        {
            // Ensure players are spawned when the scene loads
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Vector3 spawnPosition = playerSpawnPoints[playerIndex % playerSpawnPoints.Length].transform.position;
        GameObject playerPrefab = playerPrefabs[playerIndex % playerPrefabs.Length];
        PhotonNetwork.Instantiate("Photon/" + playerPrefab.name, spawnPosition, Quaternion.Euler(0, 90, 0));
    }
}
