using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieTraverseArrival : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Zombie"))
        {
            other.transform.GetChild(0).GetComponent<Zombie_Damage_and_Extras>().traverseCheck = true;
        }
    }
}
