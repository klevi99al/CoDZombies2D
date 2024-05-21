using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsMenu : MonoBehaviour
{
    [Header("References")]
    public TMP_Text resolutionText;
    public TMP_Text fullScreenText;
    public TMP_Text vSyncText;
    public TMP_Text fpsText;
    public TMP_Text maxFpsText;
    public TMP_Text currentDisplayText;
    public TMP_Text graphicsQuality;
    public DisplayChanger displayChanger;
    public GameObject fpsHolder;

    private TMP_Text lastResolutionText;
    private TMP_Text lastfullScreenText;
    private TMP_Text lastDisplayText;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions = new();

    private Dictionary<int, string> qualitySettingsNames = new()
    {
        { 0, "Very Low" },
        { 1, "Low" },
        { 2, "Medium" },
        { 3, "High" },
        { 4, "Very High" },
        { 5, "Ultra" }
    };

    public int[] targetFPSOptions = { 30, 45, 60, 90, 120, 200, 0 };
    public int selectedFPSIndex = 2;

    private int lastMonitorIndex;
    private int resolutionIndex;
    private int lastResolutionIndex;
    private int monitorIndex;
    private bool canSetTargetFPS = true;
    [HideInInspector] public int numberOfDisplays = 1;
    [HideInInspector] public bool isFullScreen = true;

    private void Start()
    {
        resolutions = Screen.resolutions;
        double currentRefreshRate = Screen.currentResolution.refreshRateRatio.value;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRateRatio.value == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
                Debug.Log("Resolution is: " + resolutions[i].width + " x " + resolutions[i].height);
            }
        }

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                resolutionIndex = i;
            }
        }

        numberOfDisplays = Display.displays.Length;

        monitorIndex = GetActiveDisplayIndex();
        lastMonitorIndex = monitorIndex;

        currentDisplayText.text = monitorIndex.ToString();
        lastDisplayText = currentDisplayText;

        lastResolutionIndex = resolutionIndex;
        lastResolutionText = resolutionText;
        lastfullScreenText = fullScreenText;
        
        isFullScreen = Screen.fullScreen;
        resolutionText.text = Screen.currentResolution.width + " x " + Screen.currentResolution.height;
        fullScreenText.text = Screen.fullScreen ? "On" : "Off";
        vSyncText.text = QualitySettings.vSyncCount == 1 ? "On" : "Off";

        canSetTargetFPS = QualitySettings.vSyncCount != 1;
        if(canSetTargetFPS)
        {
            UpdateTargetFPS();
        }
    }

    public void SetTargetFPSIndex()
    {
        Vector4 color;
        if (canSetTargetFPS)
        {
            selectedFPSIndex++;
            if (selectedFPSIndex > targetFPSOptions.Length - 1)
            {
                selectedFPSIndex = 0;
            }
            color = new(SettingsMenu.Instance.hoveredColor.x, SettingsMenu.Instance.hoveredColor.y, SettingsMenu.Instance.hoveredColor.z, 255);
            maxFpsText.color = color / 255;
            maxFpsText.transform.parent.GetComponent<TMP_Text>().color = color / 255;
            maxFpsText.transform.parent.GetComponent<UiAnimationHelper>().canBeChangable = true;
            UpdateTargetFPS();
        }
        else
        {
            color = new(SettingsMenu.Instance.normalColor.x, SettingsMenu.Instance.normalColor.y, SettingsMenu.Instance.normalColor.z, 255);
            maxFpsText.color = color / 255;
            maxFpsText.transform.parent.GetComponent<TMP_Text>().color = color / 255;
            maxFpsText.transform.parent.GetComponent<UiAnimationHelper>().canBeChangable = false;
        }
    }

    void UpdateTargetFPS()
    {
        if (targetFPSOptions[selectedFPSIndex] > 0)
        {
            Application.targetFrameRate = targetFPSOptions[selectedFPSIndex];
            maxFpsText.text = $"{targetFPSOptions[selectedFPSIndex]} FPS";
        }
        else
        {
            Application.targetFrameRate = -1;
            maxFpsText.text = "Unlimited";
        }
    }


    private int GetActiveDisplayIndex()
    {
        for (int i = 0; i < Display.displays.Length; i++)
        {
            if (Display.displays[i].active)
            {
                return i;
            }
        }
        return 0;
    }


    public void ChangeDisplayText()
    {
        if (numberOfDisplays <= 1) return;
        monitorIndex++;
        if (monitorIndex > Display.displays.Length - 1)
        {
            monitorIndex = 0;
        }

        currentDisplayText.text = monitorIndex.ToString();
    }

    public void ChangeQualitySettings()
    {
        int index = QualitySettings.GetQualityLevel();
        index++;
        if (index > QualitySettings.count - 1)
        {
            index = 0;
        }
        QualitySettings.SetQualityLevel(index);
        graphicsQuality.text = qualitySettingsNames[index];
    }


    public void ChangeResolution()
    {
        resolutionIndex++;
        if (resolutionIndex > filteredResolutions.Count - 1)
        {
            resolutionIndex = 0;
        }
        resolutionText.text = filteredResolutions[resolutionIndex].width + " x " + filteredResolutions[resolutionIndex].height;
    }

    public void ChangeFullScreenOption()
    {
        if (fullScreenText.text.ToLower().Equals("on"))
        {
            fullScreenText.text = "Off";
        }
        else
        {
            fullScreenText.text = "On";
        }
    }

    public void ApplyChanges(bool isTrue)
    {
        isFullScreen = fullScreenText.text.ToLower() == "on";
        if (isTrue)
        {
            lastResolutionIndex = resolutionIndex;
            lastDisplayText.text = currentDisplayText.text;
            lastMonitorIndex = monitorIndex;

            resolutionText = lastResolutionText;
            lastfullScreenText.text = fullScreenText.text;

            StartCoroutine(ChangeDisplayAndSetResolution(lastMonitorIndex, filteredResolutions[lastResolutionIndex].width, filteredResolutions[lastResolutionIndex].height, isFullScreen));
        }
        else
        {
            resolutionIndex = lastResolutionIndex;
            resolutionText.text = filteredResolutions[resolutionIndex].width + " x " + filteredResolutions[resolutionIndex].height;

            fullScreenText.text = Screen.fullScreen ? "On" : "Off";
            lastfullScreenText.text = fullScreenText.text;

            isFullScreen = fullScreenText.text.ToLower() == "on";

            currentDisplayText.text = lastDisplayText.text;
            monitorIndex = lastMonitorIndex;
            currentDisplayText.text = monitorIndex.ToString();
        }
    }


    private IEnumerator ChangeDisplayAndSetResolution(int monitorIndex, int width, int height, bool isFullScreen)
    {
        // Change the display
        displayChanger.ChangeDisplayClicked(monitorIndex);

        yield return new WaitForSeconds(0.55f);

        // Set the resolution
        Screen.SetResolution(width, height, isFullScreen);
        Screen.fullScreen = isFullScreen;
    }



    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, true);
    }

    public void SetVsyncValue()
    {
        if (vSyncText.text.ToLower().Equals("on"))
        {
            vSyncText.text = "Off";
            QualitySettings.vSyncCount = 0;
            canSetTargetFPS = true;
            SetTargetFPSIndex();
        }
        else
        {
            vSyncText.text = "On";
            QualitySettings.vSyncCount = 1;
            canSetTargetFPS = false;
            SetTargetFPSIndex();
            maxFpsText.text = "Unlimited";
        }
    }

    public void SetFpsValue()
    {
        if (fpsText.text.ToLower().Equals("on"))
        {
            fpsText.text = "Off";
            fpsHolder.SetActive(false);
        }
        else
        {
            fpsText.text = "On";
            fpsHolder.SetActive(true);
        }
    }
}
