using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public class Utils : MonoBehaviour
    {
        public void GiveWeapon(PlayerReferences references, string name)
        {
            GameObject currentWeapon = references.playerActions.transform.GetChild(0).gameObject;
            GameObject exchangeWeapon = null;
            GameObject availableWeapons = references.playerMovement.weaponsHolder;

            for (int i = 0; i < availableWeapons.transform.childCount; i++)
            {
                if (name.ToLower() == availableWeapons.transform.GetChild(i).GetComponent<Weapon_Script>().weaponName.ToLower())
                {
                    exchangeWeapon = availableWeapons.transform.GetChild(i).gameObject;
                    break;
                }
            }

            int weaponsNum = references.playerInventory.playerWeapons.Count;

            if (exchangeWeapon != null)
            {
                exchangeWeapon.SetActive(true);
                exchangeWeapon.transform.SetParent(references.playerMovement.firstPlayerHand.transform);
                exchangeWeapon.transform.SetSiblingIndex(0);
                exchangeWeapon.GetComponent<Weapon_Script>().clipAmmo = exchangeWeapon.GetComponent<Weapon_Script>().maxClipAmmo;
                exchangeWeapon.GetComponent<Weapon_Script>().reserveAmmo = exchangeWeapon.GetComponent<Weapon_Script>().maxReserveAmmo;

                if (weaponsNum > 1)
                {
                    currentWeapon.GetComponent<SpriteRenderer>().sprite = currentWeapon.GetComponent<Weapon_Script>().weaponSprite;
                    currentWeapon.GetComponent<Weapon_Script>().isWeaponUpgraded = false;
                    currentWeapon.transform.SetParent(availableWeapons.transform);
                    currentWeapon.transform.SetSiblingIndex(0);
                    currentWeapon.SetActive(false);
                }

                references.playerActions.transform.GetChild(1).gameObject.SetActive(false);
                PlayerInventory inventory = references.playerInventory;
                if (!inventory.playerWeapons.Contains(exchangeWeapon))
                {
                    inventory.playerWeapons.Add(exchangeWeapon);
                }
                inventory.playerWeapons.Remove(currentWeapon);

                references.playerActions.primaryHand.SetActive(true);
            }
        }
    }
}