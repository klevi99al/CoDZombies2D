using System;
using System.Collections;
using UnityEngine;

public class ZombieTraverse : MonoBehaviour
{
    public string direction;
    public Transform zombiesHolder;
    public GameObject teleportationEffect;
    public AudioClip crawlerTraverseSound;

    private Transform gotoPosition;
    private bool animPlayedOnce = false;
    private bool crawlerCanTeleport = true;
    private float moveSpeedToTraverse = 5f;

    private void Start()
    {
        gotoPosition = transform.GetChild(0);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Zombie"))
        {
            if ((direction.ToLower().Equals("right") && other.transform.rotation.y == 0) || (direction.ToLower().Equals("left") && other.transform.rotation.y != 0))
            {
                if (other.gameObject.GetComponent<Zombie_Movements>().isCrawler == false)
                {
                    other.gameObject.GetComponent<Zombie_Movements>().zombieAnimator.Play("zombie_traverse");
                    animPlayedOnce = true;
                }
                //StartCoroutine(MoveZombieToGoal(other.gameObject, gotoPosition.position));
            }
        }
        if (other.gameObject.CompareTag("Player"))
        {
            Physics.IgnoreCollision(transform.GetComponent<Collider>(), other.gameObject.GetComponent<Collider>());
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Zombie") && animPlayedOnce == false)
        {
            if (other.transform.GetChild(0).GetComponent<Zombie_Damage_and_Extras>().zombieTouchingTarget == false)
            {
                if ((direction.ToLower().Equals("right") && other.transform.rotation.y == 0) || (direction.ToLower().Equals("left") && other.transform.rotation.y != 0))
                {
                    if (other.gameObject.GetComponent<Zombie_Movements>().isCrawler == false)
                    {
                        other.gameObject.GetComponent<Zombie_Movements>().zombieAnimator.Play("zombie_traverse");
                        animPlayedOnce = true;
                    }
                    StartCoroutine(MoveZombieToGoal(other.gameObject, gotoPosition.position));
                }
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Zombie"))
        {
            other.transform.GetChild(0).GetComponent<Zombie_Damage_and_Extras>().zombieTouchingTarget = false;
            animPlayedOnce = false;
            if (other.gameObject.GetComponent<Zombie_Movements>().isCrawler == false)
            {
                other.gameObject.GetComponent<Zombie_Movements>().zombieAnimator.Play("zombie_chase");
            }
        }
    }

    private IEnumerator MoveZombieToGoal(GameObject zombie, Vector3 goal)
    {
        if (zombie.GetComponent<Zombie_Movements>().isCrawler == false)
        {
            Debug.Log("called");
            Zombie_Damage_and_Extras child = zombie.transform.GetChild(0).GetComponent<Zombie_Damage_and_Extras>();
            //while (zombie.gameObject != null && Vector3.Distance(zombie.transform.position, goal) >= 0.2f)
            while (zombie.gameObject != null && child.traverseCheck == false)
            {
                Debug.Log("traversing");
                zombie.transform.position = Vector3.Lerp(zombie.transform.position, goal, moveSpeedToTraverse * Time.deltaTime);
                yield return null;
            }
            animPlayedOnce = false;
            child.traverseCheck = false;
        }
        else
        {
            if (crawlerCanTeleport == true)
            {
                crawlerCanTeleport = false;
                StartCoroutine(DoZombieCrawlerTeleportation(zombie, goal));
            }
        }
    }

    private IEnumerator DoZombieCrawlerTeleportation(GameObject zombie, Vector3 goal)
    {
        AudioSource audio = zombie.GetComponentInChildren<AudioSource>();
        GameObject effect = Instantiate(teleportationEffect, zombie.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        Destroy(effect, 0.3f);
        audio.PlayOneShot(crawlerTraverseSound);
        zombie.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        zombie.transform.position = goal;
        GameObject effectAfter = Instantiate(teleportationEffect, zombie.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        Destroy(effectAfter, 0.3f);
        audio.PlayOneShot(crawlerTraverseSound);
        zombie.GetComponent<SpriteRenderer>().enabled = true;
        Destroy(effect);
        StartCoroutine(ReActivateTeleportation());
    }

    private IEnumerator ReActivateTeleportation()
    {
        // Since the crawler would teleport, it would also enter through the oncollisionenter function, and it would get stuck teleporting right after the
        // first teleportation was done, and get stuck in a loop, with this WAIT, we are sure that it would already enter the oncollision enter and when the bool
        // of the crawler teleportation switches to true again, it means that the crawler would already be in the oncollisionstay, and not get teleported since we dont call
        // the teleportation from there anyway
        yield return new WaitForSeconds(1f);
        crawlerCanTeleport = true;
    }
}
