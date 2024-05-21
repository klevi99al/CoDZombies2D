using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    public int lifeNumber = 1;
    public int playerHealth;
    public int playerMaxHealth;
    public bool isActivePlayer = true;
    public float healTimer = 0.1f;
    public float coolDownToHealAfterBeingHit;
    public GameObject redSplashScreen;
    public GameObject laststandSprite;
    public LevelManager levelManager;
    public Slider healthSlider;
    public List<int> perkIndexes;

    [HideInInspector] public List<GameObject> playersToRevive = new();
    [HideInInspector] public bool playerCanTakeDamage = true;
    [HideInInspector] public bool isDrinking = false;
    [HideInInspector] public readonly string reviveHintString = " Press and hold <color=yellow>F</color> to Revive Player";
    [HideInInspector] public readonly Vector2 hintStringOffsets = new(0, 350);
    [HideInInspector] public float dividor = 100;
    [HideInInspector] public int previousHealth = 100;
    [HideInInspector] public SpriteRenderer[] playerSprites;

    private PlayerReferences references;
    private bool doPlayerLaststandCheck = true;
    private float timer = 0;

    private void Start()
    {
        references = GetComponent<PlayerReferences>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        healthSlider = HUD.Instance.transform.parent.GetComponentInChildren<Slider>();
        redSplashScreen = HUD.Instance.transform.parent.GetChild(0).gameObject;
        playerSprites = transform.GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        // Set the players red screen splash image according to its health
        if (previousHealth != playerHealth && !references.playerMovement.playerInLaststand)
        {
            if (playerHealth < previousHealth)
            {
                //stopHealing = true;
                timer = 0;
                StopAllCoroutines();
                StartCoroutine(StartCountdownToHeal());
                if (laststandSprite.activeSelf == false)
                {
                    float hudValue = playerHealth / dividor;
                    healthSlider.value = hudValue;
                    HUD.Instance.playerHealthbarNumber.text = Mathf.Floor(healthSlider.value * 100).ToString() + "%";
                    //HUD.Instance.playerHealthbarNumber.text = hudValue * 100 + "%";
                }
                else
                {
                    HUD.Instance.playerHealthbarNumber.text = "0%";
                    healthSlider.value = 0;
                }
            }
            //else if(playerHealth > previousHealth && )
            previousHealth = playerHealth;

            // Find how much does the player has health in %
            float currentPrecentage = playerHealth * dividor / playerMaxHealth;
            float alpha = currentPrecentage / dividor;
            float difference = 1 - alpha;

            Color color = redSplashScreen.GetComponent<Image>().color;
            color.a = difference;
            redSplashScreen.GetComponent<Image>().color = color;
        }
        if (playerHealth <= 0 && doPlayerLaststandCheck)
        {
            lifeNumber--;
            HUD.Instance.playerHealthbarNumber.text = "Dead";
            doPlayerLaststandCheck = false;

            // if there is more than one player
            if (transform.parent.childCount > 1)
            {
                if (lifeNumber > 0)
                {
                    SetPlayerLastStand();
                }
                else
                {
                    levelManager.GameOver();
                }
            }
            else
            {
                SetPlayerLastStand();
            }
        }
    }

    private IEnumerator StartCountdownToHeal()
    {

        while (timer <= 3 && !references.playerMovement.playerInLaststand)
        {
            if (previousHealth < playerHealth)
            {
                break;
            }
            yield return new WaitForSeconds(0.05f);
            timer += 0.05f;
        }
        //stopHealing = false;
        StartCoroutine(HealPlayer());
    }

    private IEnumerator HealPlayer()
    {
        while (playerHealth < playerMaxHealth && !references.playerMovement.playerInLaststand)
        {
            yield return new WaitForSeconds(healTimer);
            playerHealth++;
            healthSlider.value = playerHealth / dividor;
            HUD.Instance.playerHealthbarNumber.text = Mathf.Floor(healthSlider.value * 100).ToString() + "%";
        }
    }

    public void SetPlayerLastStand()
    {
        CheckForAllPlayersDead();
        transform.GetComponentInChildren<HandleFireAction>().canPerformActions = false;
        redSplashScreen.SetActive(false);
        Camera.main.GetComponent<PostProcess>().enabled = true;
        doPlayerLaststandCheck = false;

        references.playerMovement.playerNeck.GetComponent<SpriteRenderer>().enabled = false;
        references.playerMovement.secondPlayerHand.GetComponent<SpriteRenderer>().enabled = false;
        references.playerMovement.firstPlayerHand.GetComponent<AudioSource>().Stop();

        HideOrEnablePlayerSprites(false);

        references.playerMovement.playerInLaststand = true;
        laststandSprite.SetActive(true);
        Invoke(nameof(PlayerRespawn), 60f);
    }

    private void HideOrEnablePlayerSprites(bool state)
    {
        for (int i = 0; i < playerSprites.Length - 1; i++)
        {
            playerSprites[i].enabled = state;
        }
    }

    public void CheckForAllPlayersDead()
    {
        GameObject[] players = levelManager.players;
    }

    public void PlayerRespawn()
    {
        references.playerActions.canPerformActions = true;
        redSplashScreen.SetActive(true);
        Camera.main.GetComponent<PostProcess>().enabled = false;
        references.playerMovement.playerNeck.GetComponent<SpriteRenderer>().enabled = true;
        references.playerMovement.secondPlayerHand.GetComponent<SpriteRenderer>().enabled = true;

        HideOrEnablePlayerSprites(true);

        playerHealth = playerMaxHealth;
        HUD.Instance.playerHealthbarNumber.text = playerHealth.ToString() + "%";
        healthSlider.value = 1;
        doPlayerLaststandCheck = true;
        laststandSprite.SetActive(false);
        references.playerMovement.playerInLaststand = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Laststand_player"))
        {
            references.playerLaststand.enabled = true;
            if (!playersToRevive.Contains(other.gameObject))
            {
                playersToRevive.Add(other.gameObject);
            }
            if (playersToRevive.Count > 0)
            {
                references.playerLaststand.canShowProgressbar = true;
                HUD.Instance.SetHintString(reviveHintString);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Laststand_player"))
        {
            playersToRevive.Remove(other.gameObject);
            if (playersToRevive.Count <= 0)
            {
                references.playerLaststand.canShowProgressbar = false;
                references.playerLaststand.enabled = false;
                HUD.Instance.CloseHintString();
            }
        }
    }
}
