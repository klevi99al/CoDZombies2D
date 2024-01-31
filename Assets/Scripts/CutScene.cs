using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutScene : MonoBehaviour
{
    [Header("Cutscene Variables")]
    public bool shouldPlayCutScene = false;
    public float zoom;
    public float zoomMultiplier = 4f;
    public float minZoom = 2f;
    public float maxZoom = 8f;
    public float velocity = 0f;
    public float smoothTime = 0.25f;
    public float scrollValue = 0.1f;
    public Vector3 cameraPosition;

    [Header("Cutscene Objects")]
    public Camera cam;
    public Image blackImage;

    [Header("Audios")]
    public AudioClip endGameSound;
    public AudioSource source;

    private float blackScreenTransitionTime;

    private void Start()
    {
        zoom = cam.orthographicSize;
        blackScreenTransitionTime = endGameSound.length;
    }

    private void Update()
    {
        if (shouldPlayCutScene)
        {
            zoom -= scrollValue * zoomMultiplier;
            zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocity, smoothTime);
        }
    }

    public void EndGamePlayCutscene()
    {
        blackImage.enabled = true;
        source.PlayOneShot(endGameSound);
        StartCoroutine(PlayEndGameCutscene());
    }

    private IEnumerator PlayEndGameCutscene(float blackScreenTime = 2f)
    {
        Vector4 start = Vector4.zero;
        Vector4 end = new(0, 0, 0, 1);

        int counter = 0;

        while(counter < 2)
        {
            for (float t = 0f; t < blackScreenTime; t += Time.deltaTime)
            {
                float normalizedTime = t / blackScreenTime;
                blackImage.color = Color.Lerp(start, end, normalizedTime);
                yield return null;
            }

            blackImage.color = end;
            shouldPlayCutScene = true;

            Vector4 temp = start;
            start = end;
            end = temp;
            blackScreenTransitionTime -= blackScreenTime;
            blackScreenTime = blackScreenTransitionTime;
            counter++;
        }
        SceneManager.LoadScene("MainMenu");
    }
}
