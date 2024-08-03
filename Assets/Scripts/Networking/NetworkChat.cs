using System.Collections;
using TMPro;
using UnityEngine;

public class NetworkChat : MonoBehaviour
{
    private readonly float timeToDisable = 15f;
    public float time;
    public TMP_Text text;

    private void Start()
    {
        time = timeToDisable;
    }

    public void SetMessage(string message)
    {
        StopAllCoroutines(); // Stop any existing coroutine
        text.text = message;
        gameObject.SetActive(true);
        StartCoroutine(StartTimer()); // Start a new coroutine for the timer
    }

    private IEnumerator StartTimer()
    {
        time = timeToDisable; // Reset the timer
        while (time > 0)
        {
            yield return null;
            time -= Time.deltaTime;
        }
        gameObject.SetActive(false); // Deactivate the message after the timer ends
    }
}
