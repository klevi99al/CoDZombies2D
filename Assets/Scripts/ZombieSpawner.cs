using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public int zoneNumber;
    public GameObject zombie;
    private GameObject zombiesHolder;

    private void Start()
    {
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
        while (true)
        {
            if (StaticVariables.levelInBetweenRounds == false)
            {
                if (StaticVariables.zombiesSpawnedThisRound < LevelManager.TotalZombiePerRound())
                {
                    if (zombiesHolder.transform.childCount < 25)
                    {
                        Instantiate(zombie, transform.position, Quaternion.identity, zombiesHolder.transform);
                        StaticVariables.zombiesSpawnedThisRound++;
                    }
                }
                yield return new WaitForSeconds(Random.Range(1, StaticVariables.timeToWaitForNextZombieSpawn + 1));
            }
            else
            {
                yield return new WaitUntil(() => StaticVariables.levelInBetweenRounds == false);
            }
        }
    }
}
