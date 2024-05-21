using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSounds : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioClip hoverSound;
    public AudioSource source;

    // button click
    public void ButtonClicked()
    {
        source.PlayOneShot(clickSound);
    }

    // button hover
    public void ButtonHovered()
    {
        source.PlayOneShot(hoverSound);
    }

    public void PlayPressedKeySound()
    {

    }
}
