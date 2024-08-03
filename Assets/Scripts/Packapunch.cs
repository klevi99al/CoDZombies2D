using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Packapunch : MonoBehaviour
{
    [Header("General Variables")]
    public int cost;
    public string hintString;
    public GameObject tempPapWeapon;
    public GameObject playersHolder;
    public GameObject sparksEffect;
    public GameObject papWeaponsHolder;
    public BoxCollider weaponTrigger;

    [Header("Audio")]
    public AudioClip papTurnedOn;
    public AudioClip papTakeWeapon;
    public AudioClip papLoop;
    public AudioClip papUpgradingWeapon;
    public AudioClip papWeaponReady;
    public AudioClip papMusic;
    public AudioClip papSting;
    public AudioSource looper;

    [HideInInspector] public bool isActive = false;
    [HideInInspector] public bool papInUse = false;
    [HideInInspector] public GameObject activator;
    public GameObject activeWeapon;

    private int playersInTrigger = 0;
    private LevelManager levelManager;

    private void Start()
    {
        weaponTrigger.enabled = false;
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        playersHolder = levelManager.playersHolder;
        tempPapWeapon.GetComponent<WeaponPick>().enabled = false;
        hintString = GetPapHintString();
        StartCoroutine(WaitForPower());
    }

    private void Update()
    {
        if (isActive && !papInUse)
        {
            if (Input.GetKeyDown(KeyCode.F) && playersInTrigger > 0)
            {
                activator = playersHolder.transform.GetChild(0).gameObject;
                if (activator.GetComponent<PlayerReferences>().playerActions.transform.GetChild(0).GetComponent<Weapon_Script>().isWeaponUpgraded)
                {
                    return;
                }

                PlayerReferences references = activator.GetComponent<PlayerReferences>();

                if (references.IsPlayerFocused())
                {
                    if (references.playerExtras.playerScore >= cost)
                    {
                        references.playerExtras.playerScore -= cost;
                        HUD.Instance.CloseHintString();
                        StartCoroutine(UsePackapunch(references));
                    }
                }
            }
        }
    }

    private IEnumerator WaitForPower()
    {
        yield return new WaitUntil(() => StaticVariables.powerTurnedOn == true);
        isActive = true;
        looper.loop = false;
        looper.PlayOneShot(papTurnedOn);
        yield return new WaitForSeconds(papTurnedOn.length);
        looper.loop = true;
        looper.Play();
    }

    public IEnumerator UsePackapunch(PlayerReferences references)
    {
        GameObject weapon = references.playerActions.transform.GetChild(0).gameObject;

        if (!weapon.GetComponent<Weapon_Script>().isWeaponUpgraded)
        {
            papInUse = true;
            int weaponsNum = references.playerInventory.playerWeapons.Count;
            if (weaponsNum == 1)
            {
                references.playerActions.primaryHand.SetActive(false);
                references.playerActions.secondaryHand.SetActive(false);
            }

            activeWeapon = weapon;

            references.playerInventory.playerWeapons.Remove(weapon);
            references.playerActions.primaryHand.transform.GetChild(0).gameObject.SetActive(false);
            references.playerActions.primaryHand.transform.GetChild(0).SetParent(references.playerMovement.weaponsHolder.transform);

            if(weaponsNum > 1)
            {
                references.playerActions.primaryHand.transform.GetChild(0).gameObject.SetActive(true);
            }

            // Play Packapunch animation
            StartCoroutine(SetEffectStateAfterTime(true, 1));
            tempPapWeapon.GetComponent<SpriteRenderer>().sprite = weapon.GetComponent<SpriteRenderer>().sprite;
            looper.PlayOneShot(papTakeWeapon);
            yield return new WaitForSeconds(papTakeWeapon.length);
            looper.PlayOneShot(papUpgradingWeapon);
            yield return new WaitForSeconds(papUpgradingWeapon.length - 1f);
            looper.PlayOneShot(papWeaponReady);
            tempPapWeapon.GetComponent<SpriteRenderer>().sprite = weapon.GetComponent<Weapon_Script>().papSprite;
            tempPapWeapon.GetComponent<WeaponPick>().enabled = true;
            weaponTrigger.enabled = true;
            weaponTrigger.GetComponent<WeaponPick>().activator = activator;
            StartCoroutine(nameof(WeaponDisappear));
            StartCoroutine(SetEffectStateAfterTime(true));
        }

        yield return null;
    }

    public void StopWeaponDisappear()
    {
        Debug.Log("Stopped something");
        StopCoroutine(nameof(WeaponDisappear));
    }

    private IEnumerator WeaponDisappear()
    {
        yield return new WaitForSeconds(20f);
        if (activator != null)
        {
            for (int i = 0; i < activator.GetComponent<PlayerInventory>().playerWeapons.Count; i++)
            {
                if (activator.GetComponent<PlayerInventory>().playerWeapons[i] == activeWeapon)
                {
                    activator.GetComponent<PlayerInventory>().playerWeapons.Remove(activeWeapon);
                    break;
                }
            }
            PlayerReferences references = activator.GetComponent<PlayerReferences>();
            references.playerActions.papInProgress = false;
            tempPapWeapon.GetComponent<SpriteRenderer>().sprite = null;
            activeWeapon = null;
            activator = null;
            weaponTrigger.enabled = false;
            sparksEffect.SetActive(false);
            papInUse = false;
        }
    }

    private IEnumerator SetEffectStateAfterTime(bool state, float time = 0)
    {
        yield return new WaitForSeconds(time);
        sparksEffect.SetActive(state);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isActive)
        {
            playersInTrigger++;
            HUD.Instance.CloseHintString();
            HUD.Instance.SetHintString(hintString);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isActive)
        {
            playersInTrigger--;
            if (playersInTrigger <= 0)
            {
                HUD.Instance.CloseHintString();
            }
        }
    }

    private string GetPapHintString()
    {
        return "Press and hold <color=yellow>F</color> to upgrade weapon[Cost: <color=yellow>" + cost + "</color>]";
    }
}
