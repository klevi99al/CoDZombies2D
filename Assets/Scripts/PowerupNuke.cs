using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupNuke : MonoBehaviour
{
    [HideInInspector] public bool powerupGrabbed = false;   // we keep this to avoid any bugs with double activating of the powerup
    private GameObject playersHolder;
    private AudioSource source;
    private Powerups powerups;
    private Coroutine lifetimeCoroutine;

    private void Start()
    {
        powerups = transform.parent.GetComponent<Powerups>();
        playersHolder = powerups.playersHolder;
        source = GetComponent<AudioSource>();
        lifetimeCoroutine = StartCoroutine(PowerupLifetime());
    }

    private IEnumerator PowerupGrabEffect()
    {
        GameObject effect = Instantiate(powerups.powerupGrabEffect, transform.position, Quaternion.identity, transform.parent);
        yield return new WaitForSeconds(effect.GetComponent<ParticleSystem>().main.duration);
        Destroy(effect);
    }

    private IEnumerator PowerupLifetime()
    {
        yield return new WaitForSeconds(15f);
        int counter = 0;
        float smallPowerupTimer = 0.25f;
        float bigPowerupTimer = 1f;
        while (counter < 3)
        {
            if(counter == 1)
            {
                smallPowerupTimer = 0.1f;
                bigPowerupTimer = 0.5f;
            }
            else if(counter == 2)
            {
                smallPowerupTimer = 0.05f;
                bigPowerupTimer = 0.25f;
            }
            for (int i = 0; i < 7; i++)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                transform.GetChild(0).gameObject.SetActive(false);

                yield return new WaitForSeconds(smallPowerupTimer);
                
                GetComponent<SpriteRenderer>().enabled = true;
                transform.GetChild(0).gameObject.SetActive(true);

                yield return new WaitForSeconds(bigPowerupTimer);
            }
            counter++;
        }
        Destroy(transform.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && !powerupGrabbed)
        {
            powerupGrabbed = true;
            StopCoroutine(lifetimeCoroutine);
            StartCoroutine(PowerupGrabEffect());
            HideNuke();
            if(!StaticVariables.nukedImageActive)
            {
                HUD.Instance.SetNukedImage();
            }
            StartCoroutine(MakeNukeEffect());
        }
    }

    private void HideNuke()
    {
        transform.GetComponent<SpriteRenderer>().enabled = false;
        if (TryGetComponent(out SphereCollider collider))
        {
            Destroy(collider);
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private IEnumerator MakeNukeEffect()
    {
        source.Stop();
        AudioClip grabbingPowerupSound = powerups.nukeFlash;
        source.PlayOneShot(grabbingPowerupSound);
        yield return new WaitForSeconds(grabbingPowerupSound.length);

        powerups.PlayAnnouncerPowerupVocals(powerups.nukeSound);

        KillAllZombies();
        RewardPlayers();
        Destroy(gameObject);
    }

    private void KillAllZombies()
    {
        GameObject zombiesHolder = transform.parent.GetComponent<Powerups>().zombiesHolder;
        if (zombiesHolder.transform.childCount > 1)
        {
            int killedZombies = 0;

            for (int i = 1; i < zombiesHolder.transform.childCount; i++)
            {
                zombiesHolder.transform.GetChild(i).GetComponentInChildren<Zombie_Damage_and_Extras>().KillZombie();
                killedZombies++;
            }

            StaticVariables.zombiesKilledThisRound += killedZombies;

            if (StaticVariables.zombiesKilledThisRound >= LevelManager.TotalZombiePerRound())
            {
                //transform.parent.GetComponent<Powerups>().levelManager.GetComponent<LevelManager>().MakeRoundSwitchAudio();
                HUD.Instance.DoRoundTransition();
            }
        }
    }

    private void RewardPlayers()
    {
        for(int i = 0; i < playersHolder.transform.childCount; i++)
        {
            PlayerSoundsAndExtras sript = playersHolder.transform.GetChild(i).GetComponent<PlayerSoundsAndExtras>();
            int amount = StaticVariables.nukeRewardPoints;

            sript.playerScore += amount;
            HUD.Instance.UpdatePlayerScoreHUD(sript.playerScore, amount);
        }
    }
}
