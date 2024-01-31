using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    public GameObject explosionPrefab;
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
        {
            Physics.IgnoreCollision(collision.collider, transform.GetComponent<BoxCollider>());
        }
    }
}
