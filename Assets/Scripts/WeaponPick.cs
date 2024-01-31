using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class WeaponPick : MonoBehaviour
{
    public GameObject activator;
    public MysteryBox box;

    public TYPE TRIGGER_TYPE;
    public enum TYPE
    {
        MYSTERY_BOX_WEAPON,
        WALL_WEAPON,
        FREE_WEAPON,
        PACKAPUNCH_WEAPON
    }


    public bool activatorInTrigger = false;

    private void Start()
    {
        box = transform.GetComponentInParent<MysteryBox>();
    }

    private void Update()
    {
        if (activatorInTrigger && activator != null)
        {
            if (Input.GetKeyDown(KeyCode.F) && activator.GetComponent<PlayerReferences>().IsPlayerFocused() && activatorInTrigger)
            {
                switch (TRIGGER_TYPE)
                {
                    case TYPE.MYSTERY_BOX_WEAPON:
                        PickWeaponFromBox();
                        break;
                    case TYPE.PACKAPUNCH_WEAPON:
                        PickWeaponFromPackapunch();
                        break;
                    default:
                        break;

                }
            }
        }
    }

    private void PickWeaponFromPackapunch()
    {
        GameObject weaponsHolder = activator.GetComponent<PlayerMovement>().weaponsHolder;
        Packapunch packapunch = GetComponentInParent<Packapunch>();  
        
        for (int i = 0; i < weaponsHolder.transform.childCount; i++)
        {
            if (weaponsHolder.transform.GetChild(i).gameObject == packapunch.activeWeapon)
            {
                GameObject papedWeapon = weaponsHolder.transform.GetChild(i).gameObject;
                CompleteWeaponPickFromPap(papedWeapon);
                break;
            }
        }
    }

    private void CompleteWeaponPickFromPap(GameObject papedWeapon)
    {
        PlayerReferences references = activator.GetComponent<PlayerReferences>();
        references.playerActions.canCycleWeapons = true;

        int weaponCount = references.playerInventory.playerWeapons.Count;
        if (weaponCount <= 1)
        {
            references.playerActions.primaryHand.SetActive(true);
            papedWeapon.transform.SetParent(references.playerActions.primaryHand.transform);
            papedWeapon.transform.SetSiblingIndex(0);
            
            if(weaponCount == 1)
            {
                references.playerActions.primaryHand.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
        else
        {
            GameObject currentWeapon = references.playerActions.primaryHand.transform.GetChild(0).gameObject;
            currentWeapon.SetActive(false);
            currentWeapon.transform.SetParent(references.playerMovement.weaponsHolder.transform);
            references.playerInventory.playerWeapons.Remove(currentWeapon);

            papedWeapon.transform.SetParent(references.playerActions.primaryHand.transform);
            papedWeapon.transform.SetSiblingIndex(0);
        }

        references.playerInventory.playerWeapons.Add(papedWeapon);

        Weapon_Script script = papedWeapon.GetComponent<Weapon_Script>();
        script.isWeaponUpgraded = true;

        references.playerActions.primaryHand.SetActive(true);
        references.playerActions.secondaryHand.SetActive(true);
        references.playerActions.papInPorgress = false;

        papedWeapon.GetComponent<SpriteRenderer>().sprite = script.papSprite;
        papedWeapon.SetActive(true);

        ResetPackapunch(transform.parent.GetComponent<Packapunch>());
        HUD.Instance.CloseHintString();
        activator = null;
        activatorInTrigger = false;
    }

    public void ResetPackapunch(Packapunch packapunch)
    {
        packapunch.tempPapWeapon.GetComponent<SpriteRenderer>().sprite = null;
        packapunch.activeWeapon = null;
        packapunch.activator = null;
        packapunch.weaponTrigger.enabled = false;
        packapunch.sparksEffect.SetActive(false);
        packapunch.papInUse = false;
        packapunch.StopWeaponDisappear();
    }

    private void PickWeaponFromBox()
    {       
        Debug.Log("Pressed F");
        Weapon_Script weaponScript = box.activeWeapon.GetComponent<Weapon_Script>();
        GameObject weaponSprite = weaponScript.hudImage;

        HUD.Instance.SwitchWeaponSprite(weaponSprite);
        HUD.Instance.UpdateAmmoHUD(weaponScript.clipAmmo, weaponScript.reserveAmmo);
        box.GiveWeapon(box.activeWeapon.name);
        box.CloseMysteryBox(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == activator)
        {
            if (TRIGGER_TYPE == TYPE.MYSTERY_BOX_WEAPON)
            {
                if (box.activeWeapon != null)
                {
                    string weaponName = box.activeWeapon.GetComponent<Weapon_Script>().weaponName;
                    HUD.Instance.SetHintString("Press and hold <color=yellow>F</color> to take " + weaponName);
                }
            }
            if (TRIGGER_TYPE == TYPE.PACKAPUNCH_WEAPON)
            {
                GameObject wepaon = transform.parent.GetComponent<Packapunch>().activeWeapon;
                string weaponName = wepaon.GetComponent<Weapon_Script>().weaponName;
                HUD.Instance.SetHintString("Press and hold <color=yellow>F</color> to take " + weaponName + " upgraded");
            }
            activatorInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == activator)
        {
            if (TRIGGER_TYPE == TYPE.MYSTERY_BOX_WEAPON)
            {
                if (box.activeWeapon != null)
                {
                    HUD.Instance.CloseHintString();
                }
            }
            if (TRIGGER_TYPE == TYPE.PACKAPUNCH_WEAPON)
            {
                HUD.Instance.CloseHintString();
            }
            activatorInTrigger = false;
        }
    }
}
