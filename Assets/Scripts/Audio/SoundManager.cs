using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioSource music;
    public AudioSource soundEffects;

    public AudioSource sourceToBeUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetActiveAudioSource(AudioSource source)
    {
        sourceToBeUpdated = source;
    }

    public void ChangeSoundVolume(Slider slider)
    {
        if(sourceToBeUpdated != null)
        {
            sourceToBeUpdated.volume = slider.value / slider.maxValue;
            slider.gameObject.GetComponentInChildren<TMP_Text>().text = slider.value.ToString();
        }
    }
}
