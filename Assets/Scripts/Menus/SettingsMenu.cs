using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [Header("Menu Colors")]
    public Vector3 normalColor = new(60, 60, 60);
    public Vector3 hoveredColor = new(255, 255, 255);
    public Vector3 selectedColor = new(216, 86, 0);
    public TMP_Text optionDescription;
    public GameObject applySettingsUi;

    [HideInInspector] public GameObject lastSelectedSetting;
    [HideInInspector] public GameObject activeSetting;

    [HideInInspector] public GameObject lastActiveOption;
    [HideInInspector] public GameObject activeOption;

    public static SettingsMenu Instance;

    [Header("Other references")]
    public GameObject[] settings;
    public GameObject[] submenus;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        SetFirstOptionActive(settings[0]);
        optionDescription.text = activeOption.GetComponent<UiAnimationHelper>().description;
    }

    public void SetFirstOptionActive(GameObject setting)
    {
        if(activeSetting != null)
        {
            activeSetting.GetComponent<TMP_Text>().color = new(hoveredColor.x / 255, hoveredColor.y / 255, hoveredColor.z / 255);
        }

        if(activeOption != null)
        {
            activeOption.GetComponent<TMP_Text>().color = new(hoveredColor.x / 255, hoveredColor.y / 255, hoveredColor.z / 255);
            UiAnimationHelper helper = activeOption.GetComponent<UiAnimationHelper>();
            helper.connectedOption.GetComponent<TMP_Text>().color = new(hoveredColor.x / 255, hoveredColor.y / 255, hoveredColor.z / 255);
            helper.uiCorners.SetActive(false);
        }

        activeSetting = setting;
        activeSetting.GetComponent<TMP_Text>().color = new(selectedColor.x / 255, selectedColor.y / 255, selectedColor.z / 255);
        lastSelectedSetting = activeSetting;

        activeOption = setting.transform.GetChild(0).GetChild(0).gameObject;
        activeOption.GetComponent<TMP_Text>().color = new(selectedColor.x / 255, selectedColor.y / 255, selectedColor.z / 255);
        lastActiveOption = activeOption;

        UiAnimationHelper uiAnimationHelper = activeOption.GetComponent<UiAnimationHelper>();
        uiAnimationHelper.connectedOption.GetComponent<TMP_Text>().color = new(selectedColor.x / 255, selectedColor.y / 255, selectedColor.z / 255);
        uiAnimationHelper.uiCorners.SetActive(true);
    }

    public void ApplySettings(bool state)
    {
        applySettingsUi.SetActive(state);
        GetComponentInChildren<TMP_Text>().color = new(selectedColor.x / 255, selectedColor.y / 255, selectedColor.z / 255);
    }

    public void SetColorToHelperButton(TMP_Text text)
    {
        text.color = new(selectedColor.x / 255, selectedColor.y / 255, selectedColor.z / 255);
    }
}
