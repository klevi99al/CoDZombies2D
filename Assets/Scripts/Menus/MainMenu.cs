using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using Unity.Netcode;

public class MainMenu : MonoBehaviour
{
    [Header("Audio Resources")]
    public AudioClip clickSound;
    public AudioClip clickSound_new;
    public AudioClip hoverSound;
    public AudioClip hoverSound_new;
    public AudioClip pressSound;
    public AudioClip selectSound;
    public AudioClip menuBackSound;
    public AudioClip menuBackSound2;
    public AudioSource source;
    public AudioMixer audioMixer;

    [Header("Other Resources")]
    public TMP_Dropdown resolutionDropDown;
    public Toggle vsyncToggle;
    public Toggle fullscreenToggle;
    public GameObject invisibleBackgroundOfSettings;

    [Header("Solo Main Menu References")]
    public int selectedMapIndex = -1;
    public int selectedCharacterIndex = -1;
    public GameObject characterOne;
    public GameObject characterTwo;

    [Header("Loading Screen Settings")]
    public List<Sprite> loadScreenImages;
    public GameObject loadingScreen;
    public Slider loadingScreenSlider;
    public TMP_Text progressText;

    [Header("Other")]
    public GameObject settingsSubMenu;

    [Header("Testing")]
    public RectTransform rectTransform;
    public Camera mainCamera;

    private bool mapIsSelected = false;
    private bool playerIsSelected = false;
    private Resolution[] resolutions;
    private GameObject currentSelectedCharacter = null;
    private GameObject lastSelectedCharacter = null;
    private GameObject currentMapOutliner = null;
    private GameObject lastMapOutliner = null;

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
            {
                source.PlayOneShot(pressSound);
            }
        }
    }

    public void LaunchSingleplayer()
    {
        if (playerIsSelected && mapIsSelected)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("MainGame", LoadSceneMode.Single);
            //StaticVariables.isCoopGame = false;
            //StartCoroutine(LoadingScreenProgress(selectedMapIndex));
        }
    }

    private IEnumerator LoadingScreenProgress(int index)
    {
        Sprite loadScreenSprite = loadScreenImages[selectedMapIndex - 1];
        loadingScreen.transform.GetChild(0).GetComponent<Image>().sprite = loadScreenSprite;
        loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(index);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingScreenSlider.value = progress;
            progressText.text = progress * 100f + "%";
            yield return null;
        }
    }

    public void LaunchSettingsMenu(bool activation)
    {
        settingsSubMenu.SetActive(activation);
        //invisibleBackgroundOfSettings.SetActive(activation);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    // button click
    public void ButtonClicked(bool subMenu)
    {
        if (!subMenu)
        {
            source.PlayOneShot(clickSound);
        }
        else
        {
            source.PlayOneShot(clickSound_new);
        }
    }

    // button hover
    public void ButtonHovered(bool hoveredInSubmenu)
    {
        if (!hoveredInSubmenu)
        {
            source.PlayOneShot(hoverSound);
        }
        else
        {
            source.PlayOneShot(hoverSound_new);
        }
    }

    public void PlayBackSound()
    {
        source.PlayOneShot(menuBackSound2);
    }

    public void PlayPressedKeySound()
    {
        source.PlayOneShot(pressSound);
    }

    public void PlaySelectSound()
    {
        source.PlayOneShot(selectSound);
    }


    // SETTINGS FUNCTIONS FROM THE GAME MAIN MENU

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("GameVolume", volume);
    }
    public void SetGraphicsQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }
    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, fullscreenToggle.isOn);
    }

    public void HandleMainMenuSelection(GameObject character)
    {
        playerIsSelected = true;

        currentSelectedCharacter = character;

        // set the current active player index cos we need it when we change menu
        selectedCharacterIndex = character.transform.GetSiblingIndex();
        StaticVariables.selectedCharacterIndex = selectedCharacterIndex;
        // activate the current player outline
        GameObject outline = character.transform.GetChild(0).gameObject;
        outline.SetActive(true);

        // disbale the last selected player outline
        if (lastSelectedCharacter != null && currentSelectedCharacter != lastSelectedCharacter)
        {
            lastSelectedCharacter.transform.GetChild(0).gameObject.SetActive(false);
        }

        if (currentSelectedCharacter != lastSelectedCharacter)
        {
            lastSelectedCharacter = currentSelectedCharacter;
        }
    }

    public void CloseMenuSolo()
    {
        if (currentSelectedCharacter != null)
        {
            mapIsSelected = false;
            playerIsSelected = false;

            selectedMapIndex = -1;
            selectedCharacterIndex = -1;
            StaticVariables.selectedCharacterIndex = 0;
            currentSelectedCharacter.transform.GetChild(0).gameObject.SetActive(false);

            currentMapOutliner.SetActive(false);
            lastMapOutliner.SetActive(false);
        }
    }

    public void ShowMapOutliner(GameObject outliner)
    {
        mapIsSelected = true;

        outliner.SetActive(true);

        currentMapOutliner = outliner;

        if (lastMapOutliner != null && lastMapOutliner != currentMapOutliner)
        {
            lastMapOutliner.SetActive(false);
        }

        if (lastMapOutliner != currentMapOutliner)
        {
            lastMapOutliner = currentMapOutliner;
        }
    }

    public void SetSelectedMapIndex(int index)
    {
        selectedMapIndex = index;
        Debug.Log("Selected map index is: " + selectedCharacterIndex);
    }
}
