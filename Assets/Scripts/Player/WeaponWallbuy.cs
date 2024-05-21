using Helpers;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWallbuy : MonoBehaviour
{
    [Header("References")]
    public int cost;
    public int reloadCost;
    public string weaponName;
    public string hintStringName;
    public GameObject weaponSprite;
    public GameObject playersHolder;

    [Header("Scripts")]
    public Audios audios;
    public LevelManager levelManager;
    public Utils utils;

    [Header("Audios")]
    public AudioSource source;

    private int playersTouchingTrigger = 0;
    private bool wallbuyInUse = false;
    private Weapon_Script weaponScript;


    private void Awake()
    {
        weaponScript = GetComponent<Weapon_Script>();
    }

    private void Update()
    {
        if (!wallbuyInUse && playersTouchingTrigger > 0)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                PlayerReferences references = playersHolder.transform.GetChild(0).GetComponent<PlayerReferences>();
                if (references.IsPlayerFocused())
                {
                    if (references.playerExtras.playerScore >= cost)
                    {
                        wallbuyInUse = true;
                        references.playerExtras.playerScore -= cost;
                        HUD.Instance.UpdatePlayerScoreHUD(references.playerExtras.playerScore, -cost);
                        Invoke(nameof(EnablePurchasing), 1f);
                        weaponSprite.SetActive(true);

                        PlayerBuyWeapon(references);
                    }
                }
            }
        }
    }

    private void PlayerBuyWeapon(PlayerReferences references)
    {
        source.PlayOneShot(audios.purchase);
        GameObject weaponSprite = weaponScript.hudImage;

        HUD.Instance.SwitchWeaponSprite(weaponSprite);
        HUD.Instance.UpdateAmmoHUD(weaponScript.clipAmmo, weaponScript.reserveAmmo);

        utils.GiveWeapon(references, weaponScript.weaponName);
    }

    private void EnablePurchasing()
    {
        wallbuyInUse = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersTouchingTrigger++;
            HUD.Instance.SetHintString("Press and hold <color=yellow>F</color> to take " + hintStringName + " [Cost: <color=yellow>" + cost + "</color>]");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersTouchingTrigger--;
            if (playersTouchingTrigger <= 0)
            {
                HUD.Instance.CloseHintString();
            }
        }
    }
}
