using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;

public class PlayerLaststand : MonoBehaviour
{
    [HideInInspector] public float progressFactor = 6;
    [HideInInspector] public bool isBeingRevived = false;
    [HideInInspector] public bool canShowProgressbar = false;
    [HideInInspector] public bool playerInReviveTrigger = false;
    [HideInInspector] public GameObject activator;

    private int dotCounter = 0;
    private bool barCreated = false;
    private bool holdingUseButton = false;
    private bool progressHasFinished = false;
    private float dotTime = 0;
    private float progress = 0;
    private GameObject progressBar = null;

    private readonly string text = " Reviving player";

    PlayerReferences references;

    private void Start()
    {
        references = transform.parent.GetComponent<PlayerReferences>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            holdingUseButton = true;
        }
        else
        {
            progressHasFinished = false;
            holdingUseButton = false;
        }


        if (holdingUseButton && !progressHasFinished && canShowProgressbar)
        {
            if (!references.IsPlayerFocused())
            {
                return;
            }
            
            if (!barCreated)
            {
                references.playerMovement.canMove = false;
                references.playerActions.canPerformActions = false;

                progressBar = HUD.Instance.CreateProgressBar(0, 400);
                barCreated = true;
                playerInReviveTrigger = false;
            }
            else
            {
                progress += Time.deltaTime;
                progressBar.transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(progress / (progressFactor - 1), 1, 1);

                dotTime += Time.deltaTime;
                if (dotTime >= 0.2f)
                {
                    dotCounter++;
                    dotTime = 0;
                    if (dotCounter == 4)
                    {
                        progressBar.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
                        dotCounter = 0;
                    }
                    else
                    {
                        progressBar.transform.GetChild(0).GetComponent<TMP_Text>().text += ".";
                    }
                }

                if (progress >= (progressFactor - 1))
                {
                    progressHasFinished = true;
                    DestroyBar();
                    RevivePlayers();
                }
            }
        }
        else
        {
            if (progressBar != null)
            {
                DestroyBar();
                if(references.playerHealth.playersToRevive.Count > 0)
                {
                    HUD.Instance.CloseHintString();
                }
            }
        }
    }

    private void DestroyBar()
    {
        Destroy(progressBar);
        progressBar = null;
        barCreated = false;
        progress = 0;
        references.playerMovement.canMove = true;
        references.playerActions.canPerformActions = true;
        playerInReviveTrigger = false;
    }

    private void RevivePlayers()
    {
        List<GameObject> playersToRevive = references.playerHealth.playersToRevive;
        if (playersToRevive.Count > 0)
        {
            for (int i = 0; i < playersToRevive.Count; i++)
            {
                playersToRevive[i].transform.parent.GetComponent<PlayerHealth>().PlayerRespawn();
            }
        }
    }
}
