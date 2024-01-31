using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImageScaler : MonoBehaviour
{
    [Header("Menu Image Scale Properties")]
    public float targetWidth = 1.25f;
    public float targetHeight = 1.5f;
    public float scaleTime = 2f;
    public GameObject outlineImage;

    private float startingWidth;
    private float startingHeight;

    private void Start()
    {
        startingWidth = transform.localScale.x;
        startingHeight = transform.localScale.y;
    }

    public void ScaleUI(bool gettingBigger)
    {
        Vector3 targetScale;

        if (gettingBigger)
        {
            targetScale = new(targetWidth, targetHeight, transform.localScale.z);
        }
        else
        {
            targetScale = new(startingWidth, startingHeight, transform.localScale.z);
        }
        StopAllCoroutines();
        StartCoroutine(ResizeUI(gameObject, targetScale));
    }

    private IEnumerator ResizeUI(GameObject element, Vector3 desiredScale)
    {
        while (element.transform.localScale != desiredScale)
        {
            element.transform.localScale = Vector3.MoveTowards(element.transform.localScale, desiredScale, scaleTime * Time.deltaTime);
            if((Mathf.Abs(element.transform.localScale.x - desiredScale.x) <= 0.05) && (Mathf.Abs(element.transform.localScale.y - desiredScale.y) <= 0.05))
            {
                element.transform.localScale = desiredScale;
                break;
            }
            yield return null;
        }
    }
}
