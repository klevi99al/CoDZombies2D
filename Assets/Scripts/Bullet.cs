using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public float bulletDamage;
    [HideInInspector] public int enemiesTouched = 0;                // if a bullet has already hit an enemy we dont do that same amount of damage to the zombie nearby
    [HideInInspector] public GameObject playerWhoShotTheBullet;     // to keep track of the player kills

    private void OnTriggerEnter(Collider other)
    {
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
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}