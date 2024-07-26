using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [HideInInspector] public float bulletDamage;
    [HideInInspector] public int enemiesTouched = 0;                // if a bullet has already hit an enemy we don't do that same amount of damage to the zombie nearby
    [HideInInspector] public GameObject playerWhoShotTheBullet;     // to keep track of the player kills

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; // Ensure this logic runs only on the server

        if (other.gameObject.CompareTag("Zombie"))
        {
            if (other.gameObject != null)
            {
                Zombie_Damage_and_Extras script = other.transform.GetChild(0).GetComponent<Zombie_Damage_and_Extras>();

                if (script.zombieHealth > 0)
                {
                    GameObject blood = Instantiate(script.bloodSplatter, transform.position, Quaternion.identity, other.transform);
                    Destroy(blood, 0.5f);
                }
                else
                {
                    GameObject blood = Instantiate(script.deathBloodSplatter, transform.position, Quaternion.identity, other.transform);
                    Destroy(blood, 0.5f);
                    Destroy(other.gameObject, 0.45f);
                }
            }
        }
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Slope"))
        {
            DestroyBullet();
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer)
        {
            enabled = false;
            return;
        }

        Debug.Log("Bullet spawned successfully on the server.");
    }

    private void DestroyBullet()
    {
        if (IsServer)
        {
            NetworkObject networkObject = GetComponent<NetworkObject>();

            if (networkObject.IsSpawned)
            {
                Debug.Log("Despawning bullet.");
                networkObject.Despawn();
            }
            else
            {
                Debug.LogWarning("Attempted to despawn a bullet that is not spawned.");
            }
        }
        else
        {
            Debug.LogError("Attempted to destroy bullet on a non-server instance.");
        }
    }

    private void OnBecameInvisible()
    {
        if (!IsServer) return; // Ensure this logic runs only on the server

        Debug.Log("Bullet became invisible, attempting to destroy.");
        DestroyBullet();
    }
}
