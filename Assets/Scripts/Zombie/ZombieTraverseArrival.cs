using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieTraverseArrival : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag.ToLower();
        if (tag.Equals("zombie"))
        {
            other.GetComponent<ZombieMovements>().StopTraversalMovement();
        }
        if (tag.Equals("player"))
        {
            Physics.IgnoreCollision(transform.GetComponent<Collider>(), other.gameObject.GetComponent<Collider>());
        }
    }
}
