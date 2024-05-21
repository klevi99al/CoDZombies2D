using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PowerupHUD_Helper : MonoBehaviour
{
    private GameObject powerupsHolder;
    private Coroutine slotAnimationCoroutine;

    private void Awake()
    {
        powerupsHolder = GameObject.Find("PowerupsHolder");
    }

    public void RestartPowerup(string name)
    {
        StopAllCoroutines();
        switch (name)
        {
            case "insta_kill":      StartPowerupTimer(StaticVariables.instakillLastSlot, name);     break;
            case "double_points":   StartPowerupTimer(StaticVariables.doublePointsLastSlot, name);  break;
            case "fire_sale":       StartPowerupTimer(StaticVariables.firesaleLastSlot, name);      break;
            default: break;
        }
    }

    public void StopPowerupTimer(GameObject slot, string name)
    {
        Sprite slotSprite = null;
        switch (name)
        {
            case "insta_kill":      slotSprite = powerupsHolder.GetComponent<Powerups>().instaKillSprite;       break;
            case "double_points":   slotSprite = powerupsHolder.GetComponent<Powerups>().doublePointsSprite;    break;
            case "fire_sale":       slotSprite = powerupsHolder.GetComponent<Powerups>().firesaleSprite;        break;
            default: break;
        }

        slot.SetActive(true);
        slot.GetComponent<Image>().sprite = slotSprite;
    }

    public void StopHUDAnimation()
    {
        StopCoroutine(slotAnimationCoroutine);
    }

    public void StartPowerupTimer(GameObject slot, string name)
    {
        switch (name)
        {
            case "insta_kill":    StaticVariables.isInstakillActive = true;     break;
            case "double_points": StaticVariables.isDoublePointsActive = true;  break;
            case "fire_sale":     StaticVariables.isFiresaleActive = true;      break;
            default: break;
        }

        StartCoroutine(PowerupCountdown(slot, name));
        slotAnimationCoroutine = StartCoroutine(SlotAnimation(slot, name));
    }

    private IEnumerator PowerupCountdown(GameObject slot, string name)
    {
        switch (name)
        {
            case "insta_kill":
                while (StaticVariables.instakillDuration > 0)
                {
                    yield return new WaitForSeconds(1f);
                    StaticVariables.instakillDuration--;
                }
                StaticVariables.isInstakillActive = false;
                StaticVariables.instakillDuration = StaticVariables.powerupDuration;
                StaticVariables.instakillLastSlot = null;
                break;
            case "double_points":
                while (StaticVariables.doublePointsDuration > 0)
                {
                    yield return new WaitForSeconds(1f);
                    StaticVariables.doublePointsDuration--;
                }
                StaticVariables.isDoublePointsActive = false;
                StaticVariables.doublePointsDuration = StaticVariables.powerupDuration;
                StaticVariables.doublePointsLastSlot = null;
                break;
            case "fire_sale":
                while (StaticVariables.firesaleDuration > 0)
                {
                    yield return new WaitForSeconds(1f);
                    StaticVariables.firesaleDuration--;
                }
                StaticVariables.isFiresaleActive = false;
                StaticVariables.firesaleDuration = StaticVariables.powerupDuration;
                StaticVariables.firesaleLastSlot = null;
                StaticVariables.mysteryBoxCost = 950;
                DisableMysteryBoxes();
                break;
            default: break;
        }

        StopCoroutine(nameof(SlotAnimation));

        Powerups script = powerupsHolder.GetComponent<Powerups>();
        //StaticVariables.screenActivePowerups--;

        slot.GetComponent<Image>().sprite = null;
        slot.GetComponent<Image>().enabled = false;
        script.announcer.PlayOneShot(script.instaKillEnd);
    }

    private void DisableMysteryBoxes()
    {
        GameObject boxHolder = powerupsHolder.GetComponent<Powerups>().mysteryBoxHolder;
        MysteryBox[] mysteryBoxScripts = boxHolder.GetComponentsInChildren<MysteryBox>();

        for (int i = 0; i < mysteryBoxScripts.Length; i++)
        {
            for (int j = 0; j < StaticVariables.mysteryBoxIndexesBeforeFiresale.Count; j++)
            {
                int boxIndex = StaticVariables.mysteryBoxIndexesBeforeFiresale[j];
                if (i == boxIndex)
                {
                    mysteryBoxScripts[i].FiresaleCloseMysteryBox();
                }
            }
        }
        StaticVariables.mysteryBoxIndexesBeforeFiresale.Clear();
    }

    private IEnumerator SlotAnimation(GameObject slot, string powerupName)
    {
        switch (powerupName)
        {
            case "insta_kill":      yield return new WaitUntil(() => StaticVariables.instakillDuration <= 10);      break;
            case "double_points":   yield return new WaitUntil(() => StaticVariables.doublePointsDuration <= 10);   break;
            case "fire_sale":       yield return new WaitUntil(() => StaticVariables.firesaleDuration <= 10);       break;
            default: break;
        }

        for (int i = 0; i < 5; i++)
        {
            slot.GetComponent<Image>().enabled = false;
            yield return new WaitForSeconds(0.5f);
            slot.GetComponent<Image>().enabled = true;
            yield return new WaitForSeconds(0.5f);
        }

        int counter = 10;
        while (counter > 0)
        {
            slot.GetComponent<Image>().enabled = false;
            yield return new WaitForSeconds(0.25f);
            slot.GetComponent<Image>().enabled = true;
            yield return new WaitForSeconds(0.25f);
            counter--;
        }
    }
}
