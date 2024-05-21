using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkQuickRevive : MonoBehaviour
{
    [Header("Perk References")]
    public int cost = 1500;
    public float speedFactor = 2;
    public float drinkAnimationTime = 1;
    public string purchaseStringFirstPart = "Press and hold <color=yellow>F</color> to buy Quick Revive[Cost: <color=yellow>";
    public string purchaseStringLastPart = "</color>]";
    public Sprite perkSprite;
    public Sprite bottleSprite;

    [Header("Audio References")]
    public AudioClip perkMusic;
    public AudioClip perkStinger;
    public AudioSource looper;
    public AudioSource source;


    [Header("General References")]
    public GameObject levelManager;
    public GameObject playersHolder;

    [HideInInspector] public bool perkInUse = false;
    [HideInInspector] public GameObject activator = null;

    private int playersTouchingPerk = 0;
    private float perkTime = 0;
    private bool perkIsActive = false;
    private bool canPlayDenySound = true;
    private Perks perks;

    private void Start()
    {
        perks = transform.parent.GetComponent<Perks>();
        StartCoroutine(WaitForPower());
    }

    private void Update()
    {

        if (perkIsActive && !perkInUse && playersTouchingPerk > 0)
        {
            PlayerReferences references = playersHolder.transform.GetChild(0).GetComponent<PlayerReferences>();
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (references.playerExtras.playerScore >= cost && !references.playerInventory.playerPerks.Contains(gameObject))
                {
                    if (references.IsPlayerFocused())
                    {
                        if (Input.GetKeyDown(references.playerActions.fireKey) || Input.GetKey(references.playerActions.fireKey))
                        {
                            return;
                        }
                        references.playerActions.canPerformActions = false;
                        references.playerInventory.playerPerks.Add(gameObject);
                        perkInUse = true;
                        activator = playersHolder.transform.GetChild(0).gameObject;

                        SetPlayerVariables(references);
                        perks.SetPerkHud(perkSprite);
                        SetPerkEffect(references);

                        HUD.Instance.CloseHintString();
                        HUD.Instance.UpdatePlayerScoreHUD(activator.GetComponent<PlayerSoundsAndExtras>().playerScore, -cost);

                        looper.PlayOneShot(perks.perkPurchase);
                        source.Stop();
                        source.PlayOneShot(perkStinger);
                        StartCoroutine(StartMusicLoopSound());
                    }
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
        if (perkInUse)
        {
            perkTime += Time.deltaTime;
            if (perkTime >= 1)
            {
                perkInUse = false;
                if (activator != null)
                {
                    activator.GetComponent<PlayerHealth>().isDrinking = false;
                    activator = null;
                }
            }
        }
    }

    private IEnumerator WaitForPower()
    {
        yield return new WaitUntil(() => StaticVariables.powerTurnedOn == true);
        perkIsActive = true;
        looper.volume = 0.5f;
        source.PlayOneShot(perkMusic);
    }

    private void SetPlayerVariables(PlayerReferences references)
    {
        references.playerMovement.canMove = false;
        references.playerMovement.HandSpriteVisibility(false);

        references.playerHealth.isDrinking = true;
        references.playerExtras.playerScore -= cost;

        references.playerMovement.drinkAnimator.gameObject.SetActive(true);
        references.playerMovement.PlayDrinkingAnimation(bottleSprite);
    }

    private void SetPerkEffect(PlayerReferences references)
    {
        references.playerLaststand.progressFactor /= speedFactor;
    }

    private IEnumerator PlayDenySound()
    {
        canPlayDenySound = false;

        float audioLength = levelManager.GetComponent<Audios>().noPurchase.length;
        source.PlayOneShot(levelManager.GetComponent<Audios>().noPurchase);
        yield return new WaitForSeconds(audioLength);

        canPlayDenySound = true;
    }

    private IEnumerator StartMusicLoopSound()
    {
        while (true)
        {
            int waittime = Random.Range(60, 120);
            Debug.Log(waittime + " wait time");
            yield return new WaitForSeconds(waittime);
            source.PlayOneShot(perkMusic);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && perkIsActive)
        {
            playersTouchingPerk++;
            if (!other.gameObject.GetComponent<PlayerInventory>().playerPerks.Contains(gameObject))
            {
                HUD.Instance.SetHintString(UpdateHintString());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && perkIsActive)
        {
            playersTouchingPerk--;
            if (playersTouchingPerk <= 0)
            {
                HUD.Instance.CloseHintString();
            }
        }
    }

    private string UpdateHintString()
    {
        return purchaseStringFirstPart + cost + purchaseStringLastPart;
    }
}
