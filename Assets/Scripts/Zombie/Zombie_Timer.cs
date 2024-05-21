using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Zombie_Timer : MonoBehaviour
{
    public TMP_Text minutesText;
    public TMP_Text hoursText;
    public int minutes = 0;
    public int hours = 0;
    private int seconds = 0;

    private void Start()
    {
        StartCoroutine(StartCountingTime());
    }

    private IEnumerator StartCountingTime()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            seconds++;
            if(seconds == 60)
            {
                seconds = 0;
                minutes++;
                if(minutes == 60)
                {
                    hours++;
                    minutes = 0;
                }
            }
        }
    }
}
