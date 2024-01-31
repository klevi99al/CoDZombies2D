using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{   [Header("References")]
    public GameObject playersHolder;
    public GameObject zombiesHolder;
    public GameObject effectsHolder;
    public GameObject powerupsHolder;
    public List<GameObject> weapons;
    public GameObject[] players;
    public AudioSource source;
    public static List<int> zoneNumbers = new();

    private Audios audios;

    private void Start()
    {
        audios = GetComponent<Audios>();
        zoneNumbers.Add(0);
        StaticVariables.zombiesNumber = TotalZombiePerRound();
    }

    // a random formula that i thought of, which will be used to count how many zombies can spawn in that specific round
    public static int TotalZombiePerRound()
    {
        //Debug.Log((int)((roundNumber * 6) + Mathf.Round((float)(0.3 * roundNumber))));
        return (int)((StaticVariables.roundNumber * 6) + Mathf.Round((float)(0.3 * StaticVariables.roundNumber)));
    }

    public void MakeRoundSwitchAudio()
    {
        StartCoroutine(PlayRoundSounds());
    }


    private IEnumerator PlayRoundSounds()
    {  
        AudioClip roundEndSound = audios.roundEnd[Random.Range(0, audios.roundEnd.Count)];
        AudioClip roundStartSound = audios.roundStart[Random.Range(0, audios.roundStart.Count)];

        Debug.Log(roundStartSound + " some space " + roundEndSound);

        float soundLength = roundEndSound.length;
        source.PlayOneShot(roundEndSound);
        yield return new WaitForSeconds(soundLength);
        source.PlayOneShot(roundStartSound);

        ResetVariables();
    }

    private void ResetVariables()
    {
        StaticVariables.totalPowerupsSpawnedThisRound = 0;
        StaticVariables.totaldoublePointsSpawned = 0;
        StaticVariables.totalInstakillSpawned = 0;
        StaticVariables.totalMaxAmmosSpawned = 0;
        StaticVariables.totalNukesSpawned = 0;
        StaticVariables.totalFiresalesSpawned = 0;
    }

    public void GameOver()
    {
        return;
    }
}
