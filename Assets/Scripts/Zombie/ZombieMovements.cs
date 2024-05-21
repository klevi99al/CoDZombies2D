using UnityEngine;
using System.Collections;
using GizmoDebugger;
public class ZombieMovements : MonoBehaviour
{
    public int zoneNumber; // shows the number of the zone where its located, used for spawning purposes
    public float updateTargetTime;
    public Vector2 zombieSpeedLimits;
    public Vector2 checkDistanceLimits;
    [SerializeField] LayerMask rockLayer;


    [HideInInspector] public GameObject playersHolder;
    [HideInInspector] public GameObject zombiesHolder;
    [HideInInspector] public GameObject zombieFakeTarget;
    [HideInInspector] public bool zombieHasTarget;
    [HideInInspector] public bool isSprinter = false;
    [HideInInspector] public bool isTraversing = false;
    [HideInInspector] public bool canCheckForSlopes = false;
    [HideInInspector] public float zombieSpeed;
    [HideInInspector] public Animator zombieAnimator;
    [HideInInspector] public Transform zombieTarget;
    public ZombieTraverse.TRAVERSE_DIRECTION lastTraversalDirection;

    // for crawler zombies
    [HideInInspector] public bool isCrawler = false; // want to do a func in update, so in order to not call getcomponentinchildren each frame ill do a similar bool here instead

    private bool firstSkip = false;
    private float tempSpeed;
    private float dirX;
    private Rigidbody zombieRb;
    private Coroutine traversalCoroutine;
    private Coroutine crawlerTraversalCoroutine;
    private LevelManager levelManager;

    private void Start()
    {
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        playersHolder = GameObject.FindGameObjectWithTag("PlayersHolder");
        zombiesHolder = GameObject.FindGameObjectWithTag("ZombiesHolder");
        zombieFakeTarget = zombiesHolder.transform.GetChild(0).gameObject;

        zombieRb = transform.GetComponent<Rigidbody>();
        zombieAnimator = transform.GetComponent<Animator>();
        zombieSpeed = Random.Range(zombieSpeedLimits.x, zombieSpeedLimits.y);
        tempSpeed = zombieSpeed;
        // check if this zombue should be a sprinter according to the round and we go to max the round 8
        // so for the higher rounds at least there is a 20 or less percentage to have non spriter zombies
        int sprintPercentage;
        if (StaticVariables.roundNumber <= 2 || StaticVariables.roundNumber >= 9)
        {
            if (StaticVariables.roundNumber <= 2)
            {
                isSprinter = false;
            }
        }
        else
        {
            sprintPercentage = StaticVariables.roundNumber * 10;
            int random = Random.Range(0, 100);
            if (random <= sprintPercentage)
            {
                isSprinter = true;
                tempSpeed += 1;
            }
            else
            {
                isSprinter = false;
            }
        }

        zombieTarget = zombieTarget = playersHolder.transform.GetChild(0); // set this as the first player at the beginning, to fix some errors, it will eventually update

        if (zoneNumber == 0)
        {
            StartCoroutine(SetZombieTarget());
        }

        //if (zoneNumber != 0 && LevelManager.zoneNumbers.Contains(zoneNumber) == false)
        //{
        //    transform.gameObject.SetActive(false);
        //}
    }

    private void FixedUpdate()
    {
        if (canCheckForSlopes == true)
        {
            HandleSlopes();
        }
        HandleZombieMovement();
        HandleZombieRotation();
    }

    private void HandleSlopes()
    {
        Vector3 direction = transform.right;
        if (isTraversing)
        {
            if(lastTraversalDirection == ZombieTraverse.TRAVERSE_DIRECTION.LEFT)
            {
                direction = -direction;
            }
        }
        
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, 0.3f, rockLayer))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.06f, transform.position.z);
        }
    }

    private void HandleZombieRotation()
    {
        if (isTraversing) return;

        if (zombieTarget != null)
        {
            if (zombieTarget.position.x > transform.position.x) // zombie pointing right
            {
                dirX = 1;
                transform.eulerAngles = Vector3.zero;
            }
            else
            {
                dirX = -1;
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
    }

    private void HandleZombieMovement()
    {
        if (isTraversing) return;

        Zombie_Damage_and_Extras script = transform.GetComponentInChildren<Zombie_Damage_and_Extras>();

        bool zombieInCombat = script.zombieInCombat;
        if (zombieInCombat && zombieTarget.GetComponent<PlayerMovement>() != null)
        {
            zombieSpeed = 0;
        }
        else
        {
            zombieSpeed = tempSpeed;
        }
        zombieRb.velocity = new Vector2(dirX * zombieSpeed, zombieRb.velocity.y);
    }

    public void StartZombieTraversalMovement(Vector3 targetPosition)
    {
        if (isCrawler)
        {
            crawlerTraversalCoroutine = StartCoroutine(DoZombieCrawlerTeleportation(transform.gameObject, targetPosition));
            return;
        }
        if (!isTraversing)
        {
            traversalCoroutine = StartCoroutine(DoZombieTraversalMovement(targetPosition));
        }
    }

    private IEnumerator DoZombieTraversalMovement(Vector3 targetPosition)
    {
        zombieRb.isKinematic = true;
        zombieAnimator.Play("zombie_traverse");
        
        isTraversing = true;

        while (true)
        {
            HandleSlopes();
            float distance = Vector3.Distance(transform.position, targetPosition); 
            transform.position = Vector3.Lerp(transform.position, targetPosition, 5 * Time.deltaTime);
            if (distance <= 0.1f) break;
            yield return null;
        }

        isTraversing = false;
        zombieRb.isKinematic = false;
    }

    public void StopTraversalMovement()
    {
        if (traversalCoroutine != null)
        {
            StopCoroutine(traversalCoroutine);
        }
        isTraversing = false;
        zombieRb.isKinematic = false;
    }

    private IEnumerator DoZombieCrawlerTeleportation(GameObject zombie, Vector3 goal)
    {
        AudioSource audio = zombie.GetComponentInChildren<AudioSource>();
        GameObject effect = Instantiate(levelManager.teleportationEffect, zombie.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        Destroy(effect, 0.3f);
        audio.PlayOneShot(levelManager.crawlerTraverseSound);
        zombie.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        zombie.transform.position = goal;
        GameObject effectAfter = Instantiate(levelManager.teleportationEffect, zombie.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        Destroy(effectAfter, 0.3f);
        audio.PlayOneShot(levelManager.crawlerTraverseSound);
        zombie.GetComponent<SpriteRenderer>().enabled = true;
        Destroy(effect);
    }

    private IEnumerator SetZombieTarget()
    {
        // wait until the spawning animation is played
        yield return new WaitForSeconds(zombieAnimator.GetCurrentAnimatorStateInfo(0).length);
        if (playersHolder.transform.childCount > 1)
        {
            while (true)
            {
                Transform closestPlayer = playersHolder.transform.GetChild(0);
                for (int i = 0; i < playersHolder.transform.childCount; i++)
                {
                    Transform currentPlayer = playersHolder.transform.GetChild(i);

                    if (!currentPlayer.gameObject.activeSelf) continue;

                    float currentPlayerDistance = Vector3.Distance(currentPlayer.position, transform.position);
                    float closestPlayerDistance = Vector3.Distance(transform.position, closestPlayer.position);

                    if ((currentPlayerDistance < closestPlayerDistance) && currentPlayer.GetComponent<PlayerHealth>().playerHealth > 0)
                    {
                        closestPlayer = playersHolder.transform.GetChild(i);
                        zombieTarget = closestPlayer;
                    }
                }
                yield return new WaitForSeconds(updateTargetTime);
            }
        }
        else
        {
            while (true)
            {
                if (playersHolder.transform.GetChild(0).GetComponent<PlayerMovement>().playerInLaststand == false)
                {
                    zombieTarget = playersHolder.transform.GetChild(0);
                }
                else
                {
                    zombieTarget = zombieFakeTarget.transform;
                }
                yield return new WaitForSeconds(updateTargetTime);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Zombie"))
        {
            Physics.IgnoreCollision(collision.collider, transform.GetComponent<Collider>());
        }
        if (collision.gameObject.CompareTag("Slope")) //Rock
        {
            canCheckForSlopes = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Slope")) //Rock
        {
            canCheckForSlopes = false;
        }
    }

    private void OnEnable()
    {
        // to fix some coroutine issues
        if (firstSkip)
        {
            StartCoroutine(SetZombieTarget());
        }
        if (!firstSkip)
        {
            firstSkip = true;
        }
    }
}
