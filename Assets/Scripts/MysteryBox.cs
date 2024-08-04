using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MysteryBox : MonoBehaviour
{
    [Header("Mystery Box Images")]
    public GameObject frontPart;
    public GameObject bottomPart;
    public GameObject lid;
    public GameObject shinyPart;
    public GameObject inactiveBox;
    public GameObject movingBear;
    public GameObject lightEffect;
    public GameObject weaponCyclingAnimation;

    [Header("Mystery Box Variables")]
    public int spinningCounter;
    public bool isValidMysteryBox = false;
    public string purchaseStringFirstPart = "Press and hold <color=yellow>F</color> to use Mystery Box[Cost: <color=yellow>";
    public string purchaseStringCostPart;
    public string purchaseStringLastPart = "</color>]";
    public GameObject playersHolder;

    [Header("Mystery Box Sounds")]
    public AudioClip boxOpen;
    public AudioClip boxClose;
    public AudioClip boxDisappear;
    public AudioClip boxMusic;
    public AudioClip boxPoof;
    public AudioClip boxTeddybear;

    [HideInInspector] public int playersTouchingBox = 0;                // a counter to know how many players are touching the box trigger
    [HideInInspector] public bool mysteryBoxIsInUse = false;            // check if the mystery box is being used
    [HideInInspector] public bool mysteryBoxHasGivenWeapon = false;     // we use this for the hintstring purposes
    [HideInInspector] public bool isPreviousMysteryBox = false;
    [HideInInspector] public GameObject activator = null;               // player who hit the mystery box
    [HideInInspector] public GameObject activeWeapon = null;            // the weapon that the mystery box gave
    [HideInInspector] public AudioSource source;

    private int totalMysteryBoxNumber = 0;
    private bool canPlayDenySound = true;
    private GameObject levelManager;


    private void Start()
    {
        if(playersHolder == null)
        {
            playersHolder = GameObject.FindGameObjectWithTag("PlayersHolder");
        }
        purchaseStringCostPart = StaticVariables.mysteryBoxCost.ToString();
        source = transform.GetComponent<AudioSource>();
        levelManager = GameObject.Find("LevelManager");
        playersHolder = GameObject.FindGameObjectWithTag("PlayersHolder");
        if (!isValidMysteryBox)
        {
            lid.SetActive(false);
            frontPart.SetActive(false);
            inactiveBox.SetActive(true);
        }
        else
        {
            isPreviousMysteryBox = true;
            lightEffect.SetActive(true);
            StaticVariables.mysteryBox = transform.gameObject;
        }
        spinningCounter = GetSpinningCounter();
        enabled = false;

        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if (transform.parent.GetChild(i).gameObject.activeSelf)
            {
                totalMysteryBoxNumber++;
            }
        }
    }

    private void Update()
    {
        if (isValidMysteryBox && !mysteryBoxIsInUse)
        {
            if (Input.GetKeyDown(KeyCode.F) && playersTouchingBox > 0)
            {
                PlayerSoundsAndExtras script = playersHolder.transform.GetChild(0).GetComponent<PlayerSoundsAndExtras>();
                if (script.playerScore >= StaticVariables.mysteryBoxCost)
                {
                    activator = script.gameObject;
                    // Get the player points
                    script.playerScore -= StaticVariables.mysteryBoxCost;
                    HUD.Instance.UpdatePlayerScoreHUD(script.playerScore, -StaticVariables.mysteryBoxCost);

                    // Clear HUD
                    HUD.Instance.CloseHintString();

                    mysteryBoxIsInUse = true;
                    if (transform.parent.childCount > 1)
                    {
                        if (!StaticVariables.isFiresaleActive)
                        {
                            if (totalMysteryBoxNumber > 1)
                            {
                                spinningCounter--;
                            }
                        }
                    }
                    OpenMysteryBox();
                }
                else
                {
                    if (canPlayDenySound)
                    {
                        StartCoroutine(PlayDenySound());
                    }
                }
            }
        }
    }

    private IEnumerator PlayDenySound()
    {
        canPlayDenySound = false;

        float audioLength = levelManager.GetComponent<Audios>().noPurchase.length;
        source.PlayOneShot(levelManager.GetComponent<Audios>().noPurchase);
        yield return new WaitForSeconds(audioLength);

        canPlayDenySound = true;
    }

    public void OpenMysteryBox()
    {
        source.PlayOneShot(boxOpen);
        source.PlayOneShot(boxMusic);

        shinyPart.SetActive(true);

        PlayWeaponSwitchingAnimation();
    }

    public void CloseMysteryBox(bool immediate = false)
    {
        if (immediate)
        {
            StopAllCoroutines();
        }
        if (activeWeapon != null)
        {
            activeWeapon.GetComponent<WeaponPick>().activator = null;
            activeWeapon.SetActive(false);
            activeWeapon = null;
        }
        activator = null;
        shinyPart.SetActive(false);
        source.PlayOneShot(boxClose);

        StartCoroutine(EnableMysteryBoxUse());
        HUD.Instance.CloseHintString();

        // we do this check so if its a different box than the real active mystery box, means that we are closing the box from a firesale
        // so if also the firesale is not active, deactivate this mystery box
        if (StaticVariables.mysteryBox != gameObject && !StaticVariables.isFiresaleActive)
        {
            DeActivateMysteryBox();
        }
    }

    private GameObject ReturnNextMysteryBox()
    {
        int index = Random.Range(0, transform.parent.childCount);
        if (transform.parent.GetChild(index).GetComponent<MysteryBox>().isPreviousMysteryBox)
        {
            if (index == transform.parent.childCount - 1)
            {
                index--;
            }
            else
            {
                index++;
            }
        }
        return transform.parent.GetChild(index).gameObject;
    }

    private IEnumerator CompleteMysteryBoxActivation(GameObject box)
    {
        MysteryBox script = box.GetComponent<MysteryBox>();
        script.movingBear.SetActive(true);
        script.movingBear.transform.localPosition += new Vector3(0, 3, 0);

        Vector3 targetPosition = Vector3.zero;

        lightEffect.SetActive(false);

        while (Vector3.Distance(targetPosition, script.movingBear.transform.localPosition) >= 0.05f)
        {
            script.movingBear.transform.localPosition = Vector3.Lerp(script.movingBear.transform.localPosition, targetPosition, 0.5f * Time.deltaTime);
            yield return null;
        }

        script.mysteryBoxIsInUse = false;
        script.isValidMysteryBox = true;
        script.isPreviousMysteryBox = true;
        script.spinningCounter = GetSpinningCounter();

        script.lid.SetActive(true);
        script.frontPart.SetActive(true);
        script.movingBear.SetActive(false);
        script.inactiveBox.SetActive(false);
        script.lightEffect.SetActive(true);

        // play box poof audio to notify the new location
        script.source.PlayOneShot(boxPoof);

        // reset the actual previous mystery box variables
        mysteryBoxIsInUse = false;
        isValidMysteryBox = false;
        isPreviousMysteryBox = false;
        spinningCounter = GetSpinningCounter();
        enabled = false;

        if (script.playersTouchingBox > 0)
        {
            HUD.Instance.SetHintString(UpdateHintString());
            script.enabled = true;
        }
    }

    public void ActivateMysteryBox(GameObject box)
    {
        StopAllCoroutines();
        StartCoroutine(CompleteMysteryBoxActivation(box));
    }

    public void FiresaleCloseMysteryBox()
    {
        StartCoroutine(CloseBoxFiresale());
    }

    private IEnumerator CloseBoxFiresale()
    {
        if (mysteryBoxIsInUse)
        {
            yield return new WaitUntil(() => mysteryBoxIsInUse == false);
            DeActivateMysteryBox();
        }
        else
        {
            DeActivateMysteryBox();
        }
    }

    public void DeActivateMysteryBox()
    {
        StopAllCoroutines();

        if (activeWeapon != null)
        {
            activeWeapon.GetComponent<WeaponPick>().activator = null;
            activeWeapon.SetActive(false);
            activeWeapon = null;
        }

        isValidMysteryBox = false;
        mysteryBoxIsInUse = false;

        shinyPart.SetActive(false);
        frontPart.SetActive(false);
        lid.SetActive(false);
        inactiveBox.SetActive(true);
        movingBear.transform.localPosition = Vector3.zero;
        movingBear.SetActive(false);
        lightEffect.SetActive(false);

        HUD.Instance.CloseHintString();
    }

    private IEnumerator EnableMysteryBoxUse()
    {
        yield return new WaitForSeconds(1f);
        mysteryBoxIsInUse = false;

        if (enabled)
        {
            HUD.Instance.SetHintString(UpdateHintString());
        }
    }

    private void PlayWeaponSwitchingAnimation()
    {
        StartCoroutine(DoWeaponChanging());
    }

    private IEnumerator DoWeaponChanging()
    {
        int counter = 0;
        int limit = weaponCyclingAnimation.transform.childCount;
        while (counter < 2)
        {
            for (int i = 0; i < limit; i++)
            {
                weaponCyclingAnimation.transform.GetChild(i).gameObject.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                weaponCyclingAnimation.transform.GetChild(i).gameObject.SetActive(false);
            }
            counter++;
        }
        BoxGiveWeapon();
        if (spinningCounter > 0)
        {
            StartCoroutine(WaitForPlayerWeaponPickup());
        }
    }

    public IEnumerator WaitForPlayerWeaponPickup()
    {
        yield return new WaitForSeconds(10f);
        CloseMysteryBox();
    }

    private void BoxGiveWeapon()
    {
        if (spinningCounter > 0)
        {
            GameObject playerWeapons = playersHolder.transform.GetChild(0).GetComponent<PlayerMovement>().weaponsHolder;
            GameObject chosenWeapon = playerWeapons.transform.GetChild(Random.Range(0, playerWeapons.transform.childCount)).gameObject;

            string chosenWeaponName = chosenWeapon.GetComponent<Weapon_Script>().name;
            Debug.Log("The chosen weapon is:    " + chosenWeaponName);
            for (int i = 0; i < weaponCyclingAnimation.transform.childCount; i++)
            {
                if (weaponCyclingAnimation.transform.GetChild(i).GetComponent<Weapon_Script>().name == chosenWeaponName)
                {
                    weaponCyclingAnimation.transform.GetChild(i).gameObject.SetActive(true);
                    activeWeapon = weaponCyclingAnimation.transform.GetChild(i).gameObject;
                    activeWeapon.GetComponent<WeaponPick>().activator = playersHolder.transform.GetChild(0).gameObject;
                }
            }
        }
        else
        {
            movingBear.SetActive(true);
            StartCoroutine(MoveTeddyBear());
        }
    }

    private IEnumerator MoveTeddyBear()
    {
        source.PlayOneShot(boxTeddybear);

        yield return new WaitForSeconds(boxTeddybear.length);

        source.PlayOneShot(boxDisappear);

        activator.GetComponent<PlayerSoundsAndExtras>().playerScore += StaticVariables.mysteryBoxCost;
        HUD.Instance.UpdatePlayerScoreHUD(activator.GetComponent<PlayerSoundsAndExtras>().playerScore, StaticVariables.mysteryBoxCost);

        Vector3 targetPosition = movingBear.transform.localPosition + new Vector3(0, 3, 0);
        while (Vector3.Distance(targetPosition, movingBear.transform.localPosition) >= 0.5f)
        {
            movingBear.transform.localPosition = Vector3.Lerp(movingBear.transform.localPosition, targetPosition, 0.5f * Time.deltaTime);
            yield return null;
        }

        DeActivateMysteryBox();

        GameObject nextMysteryBox = ReturnNextMysteryBox();
        StaticVariables.mysteryBox = nextMysteryBox;
        ActivateMysteryBox(nextMysteryBox);

    }

    public void GiveWeapon(string name)
    {
        PlayerInventory inventory = activator.GetComponent<PlayerInventory>();
        bool hasMoreThanOneWeapon = inventory.playerWeapons.Count > 1;
        
        // enable player hands
        activator.GetComponent<PlayerReferences>().playerActions.primaryHand.SetActive(true);
        activator.GetComponent<PlayerReferences>().playerActions.secondaryHand.SetActive(true);
        
        GameObject currentWeapon = playersHolder.transform.GetChild(0).GetComponent<PlayerMovement>().firstPlayerHand.transform.GetChild(0).gameObject;
        GameObject exchangeWeapon = null;
        GameObject availableWeapons = playersHolder.transform.GetChild(0).GetComponent<PlayerMovement>().weaponsHolder;
        for (int i = 0; i < availableWeapons.transform.childCount; i++)
        {
            if (name.ToLower() == availableWeapons.transform.GetChild(i).GetComponent<Weapon_Script>().name.ToLower())
            {
                exchangeWeapon = availableWeapons.transform.GetChild(i).gameObject;
                break;
            }
        }

        if (exchangeWeapon != null)
        {
            exchangeWeapon.SetActive(true);
            exchangeWeapon.transform.SetParent(playersHolder.transform.GetChild(0).GetComponent<PlayerMovement>().firstPlayerHand.transform);
            exchangeWeapon.transform.SetSiblingIndex(0);

            GiveMaxAmmo(exchangeWeapon);

            if (hasMoreThanOneWeapon)
            {
                currentWeapon.transform.SetParent(availableWeapons.transform);
                currentWeapon.GetComponent<SpriteRenderer>().sprite = currentWeapon.GetComponent<Weapon_Script>().weaponSprite;
                currentWeapon.GetComponent<Weapon_Script>().isWeaponUpgraded = false;
                currentWeapon.transform.SetSiblingIndex(0);
                currentWeapon.SetActive(false);
                inventory.playerWeapons.Remove(currentWeapon);
            }
            else
            {
                currentWeapon.transform.SetSiblingIndex(1);
                currentWeapon.SetActive(false);
            }

            if (!inventory.playerWeapons.Contains(exchangeWeapon))
            {
                inventory.playerWeapons.Add(exchangeWeapon);
            }
        }
    }

    private void GiveMaxAmmo(GameObject weapon)
    {
        Weapon_Script script = weapon.GetComponent<Weapon_Script>();
        script.clipAmmo = script.maxClipAmmo;
        script.reserveAmmo = script.maxReserveAmmo;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playersTouchingBox++;
            if (isValidMysteryBox)
            {
                if (mysteryBoxIsInUse)
                {
                    if (other.gameObject != activator)
                    {
                        HUD.Instance.CloseHintString();
                    }
                }
                else
                {
                    HUD.Instance.SetHintString(UpdateHintString());
                }
                enabled = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playersTouchingBox--;

            if (isValidMysteryBox)
            {
                HUD.Instance.CloseHintString();
                enabled = false;
            }
        }
    }

    private string UpdateHintString()
    {
        return purchaseStringFirstPart + StaticVariables.mysteryBoxCost.ToString() + purchaseStringLastPart;
    }

    private int GetSpinningCounter()
    {
        return Random.Range(5, 15);
    }
}
