using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSwitch : MonoBehaviour
{
    [Header("General Variables")]
    public float leverMoveValue = 0.3f;
    public GameObject panel;
    public GameObject lever;
    public string hintString = "Press and hold <color=yellow>F</color> to turn on the power";

    [Header("Sound References")]
    public AudioClip powerTurnedOnAudio;
    public AudioSource source;
    
    private int playersTouchingTrigger = 0;

    private void Update()
    {
        if (!StaticVariables.powerTurnedOn)
        {
            if (Input.GetKeyDown(KeyCode.F) && playersTouchingTrigger > 0)
            {
                TurnOnThePower();
            }
        }
    }

    public void TurnOnThePower()
    {
        StartCoroutine(CompletePowerSwitchAnimation());
    }

    private IEnumerator CompletePowerSwitchAnimation()
    {
        source.PlayOneShot(powerTurnedOnAudio);
        Vector3 target = lever.transform.position + new Vector3(0, leverMoveValue, 0);
        while (Vector3.Distance(target, lever.transform.position) >= 0.025f)
        {
            lever.transform.position = Vector3.MoveTowards(lever.transform.position, target, Time.deltaTime);
            yield return null;
        }

        NotifyPower();
    }
    
    private void NotifyPower()
    {
        StaticVariables.powerTurnedOn = true;
        Destroy(transform.GetComponent<BoxCollider>());
        HUD.Instance.CloseHintString();
        enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playersTouchingTrigger++;
            HUD.Instance.SetHintString(hintString);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersTouchingTrigger--;
            if(playersTouchingTrigger <= 0)
            {
                HUD.Instance.CloseHintString();
            }
        }
    }
}
