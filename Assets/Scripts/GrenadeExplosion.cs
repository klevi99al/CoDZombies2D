using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeExplosion : MonoBehaviour
{
    public GameObject granadeThrower = null;
    private void OnTriggerEnter(Collider other)
    {
        float distanceToGranade = Mathf.Abs(other.transform.position.x - transform.position.x);
        float percentage = (100 * distanceToGranade / 2) / transform.GetComponent<SphereCollider>().radius;

        if (percentage < 100)
        {
            if (other.gameObject.CompareTag("ZombieRange"))
            {
                Zombie_Damage_and_Extras zombieHealthScript = other.gameObject.GetComponentInChildren<Zombie_Damage_and_Extras>();
                float zombieTenPercentageHealth = zombieHealthScript.zombieMaxHealth / 10;

                float damage;
                if (percentage < 30)
                {
                    damage = 160;
                }
                else
                {
                    damage = 90;
                }

                zombieHealthScript.zombieHealth -= damage;
                //Debug.Log(transform.gameObject.name + " it is my name");

                if(granadeThrower != null)
                {
                    zombieHealthScript.CheckZombieHealth(granadeThrower.GetComponent<PlayerSoundsAndExtras>(), zombieHealthScript.mod = Zombie_Damage_and_Extras.KILL_TYPE.EXPLOSIVEKILL);
                }
                else
                {
                    Debug.Log("it is not defined");
                }

                //zombieHealthScript.CheckZombieHealth();
                if (zombieHealthScript.zombieHealth > 0 && zombieHealthScript.zombieHealth < zombieTenPercentageHealth)
                {
                    zombieHealthScript.zombieIsCrawler = true;
                    zombieHealthScript.transform.parent.GetComponent<Zombie_Movements>().isCrawler = true;
                    zombieHealthScript.transform.parent.GetComponent<Zombie_Movements>().zombieAnimator.SetBool("ShouldCrawl", true);
                    zombieHealthScript.transform.parent.GetComponent<Zombie_Movements>().zombieAnimator.SetBool("IsCrawler", true);
                    zombieHealthScript.transform.GetComponent<CapsuleCollider>().height /= 2;
                    zombieHealthScript.transform.GetComponent<CapsuleCollider>().center /= 2;
                    zombieHealthScript.transform.parent.GetComponent<CapsuleCollider>().height /= 2;
                    Vector3 center = zombieHealthScript.transform.parent.GetComponent<CapsuleCollider>().center;
                    zombieHealthScript.transform.parent.GetComponent<CapsuleCollider>().center = new Vector3(center.x, center.y / 2, center.z);
                }
            }
            else if (other.gameObject.CompareTag("Player"))
            {
                PlayerHealth healthScript = other.gameObject.GetComponentInChildren<PlayerHealth>();
                if (percentage <= 20)
                {
                    healthScript.playerHealth = 0;
                }
                else if (percentage < 51 && percentage > 20)
                {
                    healthScript.playerHealth -= 60;
                }
                else
                {
                    healthScript.playerHealth -= 30;
                }

                if (healthScript.playerHealth <= 0)
                {
                    healthScript.playerHealth = 0;
                }
            }
        }
    }
}
