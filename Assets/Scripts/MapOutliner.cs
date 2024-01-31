using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOutliner : MonoBehaviour
{
    public GameObject outline;

    public void OutlineActivation(bool active)
    {
        outline.SetActive(active);
    }
}
