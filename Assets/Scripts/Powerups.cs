using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Powerups : MonoBehaviour
{
    [Header("Powerups")]
    public GameObject nuke;
    public GameObject instaKill;
    public GameObject doublePoints;
    public GameObject maxAmmo;
    public GameObject firesale;

    [Header("References")]
    public GameObject playersHolder;
    public GameObject zombiesHolder;
    public GameObject levelManager;
    public GameObject mysteryBoxHolder;
    public GameObject hudIconsHolder;
    public GameObject looper;
    public List<GameObject> screenSlots = new(5);

    [Header("HUD Icons")]
    public Sprite instaKillSprite;
    public Sprite doublePointsSprite;
    public Sprite deathMachineSprite;
    public Sprite firesaleSprite;
    public Sprite zombieBloodSprite;

    [Header("FX")]
    public GameObject powerupSpawnEffect;
    public GameObject powerupGrabEffect;

    [Header("Powerup Sounds")]
    public AudioClip powerupLoop;
    public AudioClip powerupGrab;
    public AudioClip powerupSpawn;
    public AudioClip doubePointsEnd;
    public AudioClip doubePointsLoop;
    public AudioClip instaKillEnd;
    public AudioClip instaKillLoop;
    public AudioClip maxAmmoGrab;
    public AudioClip nukeFlash;
    public AudioClip nukeSoul;
    
    [Header("Announcer")]
    public AudioClip instakillSound;
    public AudioClip firesaleSound;
    public AudioClip maxAmmoSound;
    public AudioClip nukeSound;
    public AudioClip doublePointsSound;
    public AudioSource announcer;

    [HideInInspector] public List<GameObject> powerupList;

    private void Start()
    {
        for(int i = 0; i < hudIconsHolder.transform.childCount; i++)
        {
            hudIconsHolder.transform.GetChild(i).GetComponent<Image>().sprite = null;
        }
        powerupList.Add(nuke);
        powerupList.Add(instaKill);
        powerupList.Add(doublePoints);
        powerupList.Add(maxAmmo);
        powerupList.Add(firesale);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            SpawnPowerup(doublePoints, playersHolder.transform.GetChild(0).position - Vector3.one);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnPowerup(firesale, playersHolder.transform.GetChild(0).position - Vector3.one);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            SpawnPowerup(instaKill, playersHolder.transform.GetChild(0).position - Vector3.one);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnPowerup(nuke, playersHolder.transform.GetChild(0).position - Vector3.one);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            SpawnPowerup(maxAmmo, playersHolder.transform.GetChild(0).position - Vector3.one);
        }
    }

    public void SpawnPowerup(GameObject powerup, Vector3 position)
    {
        StartCoroutine(CompletePowerupSpawn(powerup, position));
    }

    private IEnumerator CompletePowerupSpawn(GameObject powerup, Vector3 position)
    {
        GameObject spawnEffect = Instantiate(powerupSpawnEffect, position, Quaternion.identity, transform);
        yield return new WaitForSeconds(powerupSpawnEffect.GetComponent<ParticleSystem>().main.duration);
        Destroy(spawnEffect);

        powerup = Instantiate(powerup, position, Quaternion.identity, transform);
        AudioSource selfSource = powerup.GetComponent<AudioSource>();
        AudioClip spawnAudio = powerupSpawn;
        selfSource.PlayOneShot(spawnAudio);

        GameObject loop = Instantiate(looper, powerup.transform);
        loop.transform.localPosition = Vector3.zero;
        looper.GetComponent<AudioSource>().Play();
    }

    public void PlayAnnouncerPowerupVocals(AudioClip audio)
    {
        if (!StaticVariables.isAnnouncerSpeaking)
        {
            StaticVariables.isAnnouncerSpeaking = true;
            StartCoroutine(PlayVocalSound(audio));
        }
    }

    private IEnumerator PlayVocalSound(AudioClip audio)
    {
        announcer.PlayOneShot(audio);
        yield return new WaitForSeconds(audio.length);
        StaticVariables.isAnnouncerSpeaking = false;
    }

    public void EnablePowerupDrops(float time = 15)
    {
        StartCoroutine(EnablePowerups(time));
    }

    private IEnumerator EnablePowerups(float time)
    {
        yield return new WaitForSeconds(time);
        StaticVariables.canPowerupsDrop = true;
    }
}
