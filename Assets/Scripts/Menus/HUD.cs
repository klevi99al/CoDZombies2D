using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    // BOTTOM-RIGHT
    [Header("General Variables")]
    public TMP_Text clipAmmoHUD;
    public TMP_Text reserveAmmoHUD;
    public TMP_Text playerScore;
    public TMP_Text playerHealthbarNumber;
    public TMP_Text addToScoreHUD;
    public TMP_Text nextWaveText;
    public Transform addToScorePointsHolder;
    public TMP_Text hintStringText;
    public GameObject hintStringImage;
    public Image weaponHUDImage;
    public Image whiteHudImage;
    public int inBetweenRoundsTimer = 15;
    public float addToScoreHUDSpeed;
    public float hintStringWindowScaleSpeed;
    public TMP_Text roundNumberText;
    public static HUD Instance;
    public GameObject granadesHolder;

    [Header("Helper variables")]
    public GameObject progressBar;
    public GameObject textTemplate;

    [Header("Sliding Animation variables")]
    public float slideFactor = 0.5f;
    public Image slideFill;
    public Image slideBackground;
    private Coroutine slideFade;

    [HideInInspector] public bool canHintStringImageScale = false;


    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        nextWaveText.gameObject.SetActive(false);
    }

    public void DoRoundTransition()
    {
        StaticVariables.levelInBetweenRounds = true;
        StartCoroutine(FadeRoundHUDColor());
    }


    private IEnumerator FadeRoundHUDColor()
    {
        nextWaveText.gameObject.SetActive(true);
        // Do HUD transition for the next round

        for (int i = inBetweenRoundsTimer; i >= 0; i--)
        {
            nextWaveText.text = " Next Wave in " + i;
            yield return new WaitForSeconds(1f);
        }
        ResetVariables();
    }

    private void ResetVariables()
    {
        StaticVariables.levelInBetweenRounds = false;
        StaticVariables.roundNumber++;

        StaticVariables.zombiesKilledThisRound = 0;
        StaticVariables.zombiesSpawnedThisRound = 0;

        roundNumberText.text = StaticVariables.roundNumber.ToString();
        roundNumberText.alpha = 1;
        inBetweenRoundsTimer = 15;
        nextWaveText.gameObject.SetActive(false);
    }

    public void SwitchWeaponSprite(GameObject weaponSprite)
    {
        weaponHUDImage.GetComponent<RectTransform>().sizeDelta = weaponSprite.transform.GetComponent<RectTransform>().sizeDelta;
        weaponHUDImage.sprite = weaponSprite.GetComponent<Image>().sprite;
    }

    public void UpdateAmmoHUD(int clip, int reserve)
    {
        clipAmmoHUD.text = clip.ToString();
        reserveAmmoHUD.text = reserve.ToString();
    }

    public void CloseHintString()
    {
        Instance.canHintStringImageScale = false;
        Instance.hintStringImage.transform.localScale = new Vector3(1, 0, 1);
    }

    // amount is gonna update the real total score
    // kill points is gonna make a new temp HUD where the scores move to play a little nice animation
    public void UpdatePlayerScoreHUD(int amount, int killPoints)
    {
        StartCoroutine(PlayerGivePointsAnimation(killPoints));
        playerScore.text = amount.ToString();
    }

    public IEnumerator PlayerGivePointsAnimation(int amount)
    {
        int startingXOffset = -(Random.Range(0, 100));
        Vector3 startingPosition = playerScore.transform.position - new Vector3(startingXOffset, 0, 0);
        int endingXOffset = -Random.Range(10, 100);
        int endingYOffset = -Random.Range(50, 100);
        Vector3 endPosition = startingPosition - new Vector3(endingXOffset, endingYOffset, 0);
        endPosition -= new Vector3(50, 0, 0);

        if (StaticVariables.isDoublePointsActive)
        {
            if (amount > 0)
            {
                amount *= 2;
            }
        }

        TMP_Text movingScore = Instantiate(addToScoreHUD, startingPosition, Quaternion.identity, addToScorePointsHolder);
        if (amount > 0)
        {
            movingScore.text = "+" + amount.ToString();
        }
        else
        {
            movingScore.text = amount.ToString();
            movingScore.color = Color.gray;
        }
        while (Vector2.Distance(movingScore.transform.position, endPosition) > 0.2f)
        {
            movingScore.transform.position = Vector3.Lerp(movingScore.transform.position, endPosition, addToScoreHUDSpeed * Time.deltaTime);
            yield return null;
        }
        Destroy(movingScore.gameObject);
    }

    public void SetHintString(string hintLine, GameObject player = null)
    {
        canHintStringImageScale = true;
        StartCoroutine(PlayHintStringWindowAnimation(hintLine, player));
    }

    // When in online, should update this block of code so it sets the hint string only for the specific player
    private IEnumerator PlayHintStringWindowAnimation(string hintLine, GameObject player = null)
    {
        TMP_Text hintString = hintStringImage.GetComponentInChildren<TMP_Text>();
        hintString.text = hintLine;

        int maxScale = 1; // since originally the value is set to 1
        Vector3 finalScale = hintStringImage.transform.localScale + new Vector3(0, maxScale, 0);
        while (hintStringImage.transform.localScale.y <= maxScale && canHintStringImageScale == true)
        {
            hintStringImage.transform.localScale = Vector3.MoveTowards(hintStringImage.transform.localScale, finalScale, hintStringWindowScaleSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void SetNukedImage(float duration = 0.005f)
    {
        StartCoroutine(ImageColorTransition(duration));
    }

    private IEnumerator ImageColorTransition(float duration)
    {
        StaticVariables.nukedImageActive = true;
        whiteHudImage.enabled = true;
        float startValue = 0;
        float endValue = 100;

        bool incrementing = true;
        int counter = 0;
        while (counter < 2)
        {
            for (float i = 0; i < 100; i++)
            {
                Color color = whiteHudImage.color;
                if (incrementing)
                {
                    color.a = (float)(startValue + i) / 100;
                }
                else
                {
                    color.a = (float)(endValue - i) / 100;
                }
                whiteHudImage.color = color;
                yield return new WaitForSeconds(duration);
                Debug.Log(color.a);
            }
            incrementing = !incrementing;
            counter++;
        }

        whiteHudImage.enabled = false;
        StaticVariables.nukedImageActive = false;
    }

    public GameObject CreateProgressBar(float xOffset, float yOffset)
    {
        GameObject bar = Instantiate(progressBar);
        bar.transform.SetParent(transform);
        bar.transform.position = Vector3.zero;
        bar.transform.localScale = Vector2.one;
        bar.GetComponent<RectTransform>().anchoredPosition = Vector2.zero - new Vector2(xOffset, yOffset);
        return bar;
    }

    public GameObject CreateText(string text, float xOffset = 0, float yOffset = 0)
    {
        GameObject template = Instantiate(textTemplate);
        template.transform.SetParent(transform);
        template.transform.position = Vector3.zero;
        template.transform.localScale = Vector2.one;
        template.GetComponent<TMP_Text>().text = text;
        template.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        template.GetComponent<RectTransform>().anchoredPosition -= new Vector2(xOffset, yOffset);
        return template;
    }

    public void SlidingProgress(PlayerMovement script)
    {
        if (slideFade != null)
        {
            StopCoroutine(slideFade);
        }

        slideBackground.color = new Vector4(slideBackground.color.r, slideBackground.color.g, slideBackground.color.b, 1);
        slideFill.color = new Vector4(slideFill.color.r, slideFill.color.g, slideFill.color.b, 1);
        
        StartCoroutine(PlaySlideProgressBarAnimation(script));
    }

    private IEnumerator PlaySlideProgressBarAnimation(PlayerMovement script)
    {
        int counter = 0;
        while (true)
        {
            if (counter > 0)
            {
                slideFill.fillAmount += Time.deltaTime / slideFactor;
                if (slideFill.fillAmount >= 1)
                {
                    break;
                }
            }
            else
            {
                slideFill.fillAmount -= Time.deltaTime;
                if (slideFill.fillAmount <= 0)
                {
                    yield return new WaitForSeconds(1f);
                    counter++;
                }
            }
            yield return null;
        }
        script.canSlide = true;
        slideFade = StartCoroutine(FadeSliderCoroutine());
    }

    private IEnumerator FadeSliderCoroutine()
    {
        yield return new WaitForSeconds(1f);
        while (slideFill.color.a > 0)
        {
            slideBackground.color = Vector4.MoveTowards(slideBackground.color, new Vector4(slideBackground.color.r, slideBackground.color.g, slideBackground.color.b, 0), Time.deltaTime);
            slideFill.color = Vector4.MoveTowards(slideFill.color, new Vector4(slideFill.color.r, slideFill.color.g, slideFill.color.b, 0), Time.deltaTime);
            yield return null;
        }
    }
}