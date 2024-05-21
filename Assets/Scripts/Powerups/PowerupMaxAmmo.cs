using System.Collections;
using UnityEngine;

public class PowerupMaxAmmo : MonoBehaviour
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !powerupGrabbed)
        {
            powerupGrabbed = true;

            StopCoroutine(lifetimeCoroutine);
            StartCoroutine(PowerupGrabEffect());

            HideMaxAmmo();
            StartCoroutine(MakeMaxAmmoEffect());
        }
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

    private void HideMaxAmmo()
    {
        transform.GetComponent<SpriteRenderer>().enabled = false;
        if(TryGetComponent(out SphereCollider collider))
        {
            Destroy(collider);
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private IEnumerator MakeMaxAmmoEffect()
    {
        RewardPlayers();
        source.Stop();
        AudioClip grabbingPowerupSound = powerups.powerupGrab;
        source.PlayOneShot(grabbingPowerupSound);
        yield return new WaitForSeconds(grabbingPowerupSound.length);

        powerups.PlayAnnouncerPowerupVocals(powerups.maxAmmoSound);


        Destroy(gameObject);
    }

    private void RewardPlayers()
    {
        for (int i = 0; i < playersHolder.transform.childCount; i++)
        {
            if (playersHolder.transform.GetChild(i).gameObject.activeSelf)
            {
                GameObject playerHand = playersHolder.transform.GetChild(i).GetComponent<PlayerMovement>().firstPlayerHand;
                Weapon_Script weaponOne = playerHand.transform.GetChild(0).GetComponent<Weapon_Script>();
                Weapon_Script weaponTwo = playerHand.transform.GetChild(1).GetComponent<Weapon_Script>();

                if (weaponOne != null)
                {
                    weaponOne.clipAmmo = weaponOne.maxClipAmmo;
                    weaponOne.reserveAmmo = weaponOne.maxReserveAmmo;
                    HUD.Instance.UpdateAmmoHUD(weaponOne.clipAmmo, weaponOne.reserveAmmo);
                }

                if (weaponTwo != null)
                {
                    weaponTwo.transform.gameObject.SetActive(true);
                    weaponTwo.clipAmmo = weaponTwo.maxClipAmmo;
                    weaponTwo.reserveAmmo = weaponTwo.maxReserveAmmo;
                    weaponTwo.transform.gameObject.SetActive(false);
                }
            }
        }
    }
}
