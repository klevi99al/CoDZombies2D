using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Perks : MonoBehaviour
{
    [Header("References")]
    public GameObject perkShadersContainer;
    public List<GameObject> perks;

    [Header("Perk Machine Sounds")]
    public AudioClip perkPurchase;
    public AudioClip perkLoop;
    public AudioClip perkTurnedOn;
    public AudioClip playerDrinkPerk;



    public void SetPerkHud(Sprite perkShader)
    {
        for(int i = 0; i < perkShadersContainer.transform.childCount; i++)
        {
            if (perkShadersContainer.transform.GetChild(i).GetComponent<Image>().sprite == null)
            {
                perkShadersContainer.transform.GetChild(i).GetComponent<Image>().enabled = true;
                perkShadersContainer.transform.GetChild(i).GetComponent<Image>().sprite = perkShader;
                break;
            }
        }
    }
}
