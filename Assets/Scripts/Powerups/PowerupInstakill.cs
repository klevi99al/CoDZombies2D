using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PowerupInstakill : MonoBehaviour
{
    [HideInInspector] public bool powerupGrabbed = false;   // we keep this to avoid any bugs with double activating of the powerup
    private readonly string powerupName = "insta_kill";
    private AudioSource source;
    private Powerups powerups;
    private Coroutine lifetimeCoroutine;

    private void Start()
    {
        powerups = transform.parent.GetComponent<Powerups>();
        source = GetComponent<AudioSource>();
        lifetimeCoroutine = StartCoroutine(PowerupLifetime());
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !powerupGrabbed)
        {
            powerupGrabbed = true;
            StaticVariables.instakillDuration = StaticVariables.powerupDuration;

            StopCoroutine(lifetimeCoroutine);
            StartCoroutine(PlayInstakillSounds());
            StartCoroutine(PowerupGrabEffect());

            HideInstaKill(); 

            if (StaticVariables.isInstakillActive)
            {
                if(StaticVariables.instakillLastSlot != null)
                {
                    StaticVariables.instakillLastSlot.GetComponent<Image>().enabled = true;
                    StaticVariables.instakillLastSlot.GetComponent<PowerupHUD_Helper>().RestartPowerup(powerupName);
                }
                StartCoroutine(WaitAndDestroy());
            }
            else
            {
                GameObject slot = null;
                for (int i = 0; i < powerups.screenSlots.Count; i++)
                {
                    if (powerups.screenSlots[i].GetComponent<Image>().sprite == null)
                    {
                        slot = powerups.screenSlots[i];
                        break;
                    }
                }

                slot.GetComponent<Image>().enabled = true;

                slot.GetComponent<PowerupHUD_Helper>().StopPowerupTimer(slot, powerupName);
                slot.GetComponent<PowerupHUD_Helper>().StartPowerupTimer(slot, powerupName);
                StaticVariables.instakillLastSlot = slot;

                StartCoroutine(WaitForDeactivation());

                // Make screen HUD
                slot.SetActive(true);
                slot.GetComponent<Image>().sprite = powerups.instaKillSprite;
            }
        }
    }

    private IEnumerator WaitForDeactivation()
    {
        yield return new WaitUntil(() => StaticVariables.isInstakillActive == false);
        Destroy(gameObject);
    }

    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(powerups.powerupGrab.length);
        Destroy(gameObject);
    }

    private void HideInstaKill()
    {
        if (transform.gameObject != null)
        {
            transform.GetComponent<SpriteRenderer>().enabled = false;
            if (TryGetComponent(out SphereCollider collider))
            {
                Destroy(collider);
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }


    private IEnumerator PlayInstakillSounds()
    {
        source.PlayOneShot(powerups.powerupGrab);
        powerups.PlayAnnouncerPowerupVocals(powerups.instakillSound);
        source.loop = true;
        source.spatialBlend = 0;
        source.volume = 0.4f;
        source.Play();
        yield return null;
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
            if (counter == 1)
            {
                smallPowerupTimer = 0.1f;
                bigPowerupTimer = 0.5f;
            }
            else if (counter == 2)
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
}
