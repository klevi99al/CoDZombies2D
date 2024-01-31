using System.Collections;
using UnityEngine;

public class Buyables : MonoBehaviour
{
    public int zoneToOpen;
    public int cost;
    public string hintString;
    private bool canCheck = true;
    public GameObject effect;
    public Transform zombieSpanwers;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HUD.Instance.SetHintString(hintString, other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HUD.Instance.CloseHintString();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (canCheck == true)
            {
                GameObject player = other.gameObject;
                if (Input.GetKey(KeyCode.F))
                {
                    canCheck = false;
                    PlayerSoundsAndExtras script = player.GetComponent<PlayerSoundsAndExtras>();
                    AudioSource source = transform.GetComponent<AudioSource>();
                    if (script.playerScore >= cost)
                    {
                        // to fix a small bug, since the debris stays there for one more sec the trigger can be reactivated during that time
                        transform.gameObject.GetComponent<BoxCollider>().enabled = false; 
                        
                        script.playerScore -= cost;
                        source.PlayOneShot(player.GetComponent<PlayerMovement>().levelManager.GetComponent<Audios>().purchase);
                        Destroy(transform.GetChild(0).gameObject);
                        StartCoroutine(PlayRotateAnimation(source, player));

                        HUD.Instance.UpdatePlayerScoreHUD(other.gameObject.GetComponent<PlayerSoundsAndExtras>().playerScore, -cost);
                        HUD.Instance.CloseHintString();
                        LevelManager.zoneNumbers.Add(zoneToOpen);
                        for (int i = 0; i < zombieSpanwers.childCount; i++)
                        {
                            if (zombieSpanwers.GetChild(i).GetComponent<ZombieSpawner>().zoneNumber == zoneToOpen)
                            {
                                zombieSpanwers.GetChild(i).GetComponent<ZombieSpawner>().PrepareSpawningLogic();
                            }
                        }
                    }
                    else
                    {
                        source.PlayOneShot(player.GetComponent<PlayerMovement>().levelManager.GetComponent<Audios>().noPurchase);
                        StartCoroutine(AllowPlayerToPurchaseAgain());
                    }
                }
            }
        }
    }

    private IEnumerator PlayRotateAnimation(AudioSource source, GameObject player)
    {
        float duration = 0.5f;
        Quaternion startRot = transform.rotation;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.rotation = startRot * Quaternion.AngleAxis(t / duration * 360f, Vector3.up); //or transform.right if you want it to be locally based
            yield return null;
        }
        transform.rotation = startRot;

        AudioClip chaChing = player.GetComponent<PlayerMovement>().levelManager.GetComponent<Audios>().chaChing;
        source.PlayOneShot(chaChing);
        yield return new WaitForSeconds(chaChing.length + 0.1f);

        float counter = 0;
        Vector3 target = transform.position + new Vector3(0, 1000, 0);
        while (transform.gameObject != null && counter < 3)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, 100 * Time.deltaTime);
            effect.transform.position = Vector3.MoveTowards(effect.transform.position, target, 100 * Time.deltaTime);
            counter += Time.deltaTime;
            yield return null;
        }
        Destroy(transform.gameObject);
    }


    private IEnumerator AllowPlayerToPurchaseAgain()
    {
        yield return new WaitForSeconds(1f);
        canCheck = true;
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canCheck = true;
        }
    }
}
