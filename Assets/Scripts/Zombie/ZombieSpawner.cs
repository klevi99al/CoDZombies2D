using Photon.Pun;
using System.Collections;
using UnityEngine;

public class ZombieSpawner : MonoBehaviourPunCallbacks, IPunObservable
{
    public int zoneNumber;
    public GameObject zombie;
    public readonly string zombieNetwork = "Photon/ZombieNetwork";
    private GameObject zombiesHolder;
    public bool shouldSpawn = false;

    private void Start()
    {
        // Ensure zombiesHolder is consistently instantiated across all clients
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            // Only instantiate zombiesHolder if it does not already exist
            if (GameObject.FindGameObjectWithTag("ZombiesHolder") == null)
            {
                GameObject holder = new GameObject("ZombiesHolder");
                holder.tag = "ZombiesHolder";
                holder.AddComponent<PhotonView>();
                DontDestroyOnLoad(holder);
                PhotonNetwork.AllocateViewID(holder.GetComponent<PhotonView>()); // Allocate a view ID for the zombiesHolder
            }
        }

        zombiesHolder = GameObject.FindGameObjectWithTag("ZombiesHolder");
        if (zoneNumber == 0)
        {
            StartCoroutine(ZombieSpawnLogic());
        }
    }

    public void PrepareSpawningLogic()
    {
        StartCoroutine(ZombieSpawnLogic());
    }

    public IEnumerator ZombieSpawnLogic()
    {
        yield return new WaitForSeconds(2f);
        while (shouldSpawn)
        {
            if (!StaticVariables.levelInBetweenRounds)
            {
                if (StaticVariables.zombiesSpawnedThisRound < LevelManager.TotalZombiePerRound())
                {
                    if (zombiesHolder.transform.childCount < 25)
                    {
                        if (PhotonNetwork.IsMasterClient) // Only Master Client spawns zombies
                        {
                            GameObject zombieInstance;
                            if (StaticVariables.isSoloGame)
                            {
                                zombieInstance = Instantiate(zombie, transform.position, Quaternion.identity, zombiesHolder.transform);
                            }
                            else
                            {
                                zombieInstance = PhotonNetwork.Instantiate(zombieNetwork, transform.position, Quaternion.identity);
                            }
                            StaticVariables.zombiesSpawnedThisRound++;
                        }
                    }
                }
                yield return new WaitForSeconds(Random.Range(1, StaticVariables.timeToWaitForNextZombieSpawn + 1));
            }
            else
            {
                yield return new WaitUntil(() => !StaticVariables.levelInBetweenRounds);
            }
        }
    }

    // Implement OnPhotonSerializeView to synchronize shouldSpawn across the network
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(shouldSpawn);
        }
        else
        {
            shouldSpawn = (bool)stream.ReceiveNext();
        }
    }
}
