using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Menu sounds")]
    public AudioClip buttonHovered;
    public AudioClip buttonClicked;
    public AudioSource source;
    public CutScene gameOverCutscene;

    [HideInInspector] public bool canOpenPauseMenu = true;

    private GameObject pauseMenu;

    private void Start()
    {
        source.ignoreListenerPause = true;
        pauseMenu = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canOpenPauseMenu)
        {
            if (pauseMenu.activeSelf)
            {
                Time.timeScale = 1;
                pauseMenu.SetActive(false);
                AudioListener.pause = false;
            }
            else
            {
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
                AudioListener.pause = true;
            }
            StaticVariables.gameIsPaused = pauseMenu.activeSelf;
        }
    }

    public void PlayMenuSound(bool pressed)
    {
        if (pressed)
        {
            source.PlayOneShot(buttonClicked);
        }
        else
        {
            source.PlayOneShot(buttonHovered);
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        pauseMenu.SetActive(false);
        StaticVariables.gameIsPaused = false;
    }

    public void Settings()
    {

    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
        AudioListener.pause = false;
        StaticVariables.gameIsPaused = false;
        pauseMenu.SetActive(false);
    }

    public void Quit()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        StaticVariables.gameIsPaused = false;
        pauseMenu.SetActive(false);
        gameOverCutscene.EndGamePlayCutscene();
    }
}
