using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_Damage_and_Extras : MonoBehaviour
{
    [Header("General")]
    public int playerHitDamage;
    public bool inflictDamage;
    public float hitAnimationWaitTime;
    public GameObject bloodSplatter;
    public GameObject deathBloodSplatter;
    public GameObject deathAudioPrefab;

    [Header("Audios")]
    public int minStopSoundFrequency;
    public int maxStopSoundFrequency;
    public float inBetweenNextStepFootWalk;
    public float inBetweenNextStepFootRun;
    public AudioClip playerGettingZombieDamage;
    public AudioClip zombieGettingKnifed;
    public List<AudioClip> zombieDeathSounds;
    public List<AudioClip> zombieWalkingSounds;
    public List<AudioClip> zombieAttackSounds;
    public List<AudioClip> zombieRunningSounds;
    public List<AudioClip> zombieCrawlingSounds;

    [HideInInspector] public LevelManager levelManager;
    [HideInInspector] public bool traverseCheck = false;
    [HideInInspector] public bool zombieInCombat = false;
    [HideInInspector] public bool zombieIsCrawler = false;
    [HideInInspector] public bool zombieTouchingTarget = false;
    [HideInInspector] public float zombieHealth;
    [HideInInspector] public float zombieMaxHealth;

    private Zombie_Movements zombieScript;
    private AudioSource audioSource;
    private int lastLayerTouched = -1;
    private bool zombieKillCounted = false;
    private bool damageTracker = false;
    private bool canPlayKnifeDamageSound = true;
    private bool zombieIsPlayingSound = false;
    private bool zombieIsMoving = true;
    private bool canDoKnifeDamage = true;
    private bool zombieGaveDeathPoints = false;
    private bool hasSpawnedPowerup = false;
    private readonly float knifeDamageCooldown = 0.7f;
    private List<GameObject> powerups;
    [HideInInspector] public enum KILL_TYPE { BULLETKILL, KNIFEKILL, EXPLOSIVEKILL, WONDERWEAPONKILL, OTHER }
    [HideInInspector] public KILL_TYPE mod;


    private void Start()
    {
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();


        zombieHealth = CalculateZombieHealth();
        //Debug.Log(zombieHealth);
        //zombieHealth = (StaticVariables.roundNumber * 50) + 50;
        zombieMaxHealth = zombieHealth;
        zombieScript = transform.parent.GetComponent<Zombie_Movements>();
        audioSource = transform.GetComponent<AudioSource>();

        StartCoroutine(PlayZombieMouthSounds());
        StartCoroutine(PlayZombieFootstepSounds());
        powerups = levelManager.powerupsHolder.GetComponent<Powerups>().powerupList;
    }

    private float CalculateZombieHealth()
    {
        float health;
        health = StaticVariables.zombieStartingHealth;
        for (int i = 2; i < StaticVariables.roundNumber; i++)
        {
            if (i >= 10)
            {
                health += (int)(health * StaticVariables.zombieHealthMultiplier);
            }
            else
            {
                health = (int)(health + StaticVariables.zombieHealthIncrease);
            }
        }
        return health;
    }

    private void Update()
    {
        if (zombieInCombat)
        {
            if (inflictDamage)
            {
                if (damageTracker == false)
                {
                    damageTracker = true;
                    PlayerHealth playerHealth = zombieScript.zombieTarget.GetComponent<PlayerHealth>();
                    if (playerHealth != null && playerHealth.playerCanTakeDamage)
                    {
                        playerHealth.playerHealth -= playerHitDamage;
                        audioSource.PlayOneShot(playerGettingZombieDamage);

                        if (playerHealth.playerHealth < 0)
                        {
                            playerHealth.playerHealth = 0;
                        }
                    }
                }
            }
            else
            {
                damageTracker = false;
            }
        }
    }


    private void OnTriggerEnter(Collider collision)
    {
        string tag = collision.gameObject.tag;
        if (tag.Equals("Player"))
        {

            if (collision.gameObject == transform.parent.GetComponent<Zombie_Movements>().zombieTarget.gameObject)
            {
                zombieTouchingTarget = true;
            }

            zombieIsMoving = false;
            zombieInCombat = true;
            zombieScript.zombieAnimator.SetBool("ShouldAttack", true);

            // should the zombie play a sound when colliding with the player or not
            int randomisedSound = Random.Range(0, 2);
            if (randomisedSound == 0)
            {
                if (!zombieIsPlayingSound)
                {
                    int index = Random.Range(0, zombieAttackSounds.Count);
                    zombieIsPlayingSound = true;
                    audioSource.PlayOneShot(zombieAttackSounds[index]);
                    StartCoroutine(ReActivateZombieSounds(zombieAttackSounds[index].length));
                }
            }
        }
        if (tag.Equals("Bullet"))
        {
            if (collision.gameObject.GetComponent<Bullet>().enemiesTouched < 3)
            {
                collision.gameObject.GetComponent<Bullet>().enemiesTouched++;
                zombieHealth -= (collision.GetComponent<Bullet>().bulletDamage / collision.gameObject.GetComponent<Bullet>().enemiesTouched);
            }
            else
            {
                Destroy(collision.gameObject);
            }

            CheckZombieHealth(collision.gameObject.GetComponent<Bullet>().playerWhoShotTheBullet.GetComponent<PlayerSoundsAndExtras>(), mod = KILL_TYPE.BULLETKILL);
        }
        // Layers 7 = Grass, 8 = Mud
        if (collision.gameObject.layer == 7)
        {
            lastLayerTouched = 7;
        }
        if (collision.gameObject.layer == 8)
        {
            lastLayerTouched = 8;
        }
        if (collision.gameObject.layer == 10)
        {
            lastLayerTouched = 10;
        }
        if (collision.gameObject.layer == 11)
        {
            lastLayerTouched = 11;
        }
    }

    public void CheckZombieHealth(PlayerSoundsAndExtras playerScoreScript, KILL_TYPE MOD)
    {
        if (StaticVariables.isInstakillActive)
        {
            zombieHealth = 0;
        }
        if (zombieHealth <= 0)
        {
            if (zombieKillCounted == false)
            {
                zombieKillCounted = true;
                StaticVariables.zombiesKilledThisRound++;
            }
            
            Debug.Log(StaticVariables.zombiesKilledThisRound + " space " + LevelManager.TotalZombiePerRound());
            if (StaticVariables.zombiesKilledThisRound >= LevelManager.TotalZombiePerRound())
            {
                // this check for between rounds is a dirty way to fix a bug where when there were multiple zombies at one spot and you shot them
                // and killed them all at once, if there were no more zombies, it would skip 2,3 rounds at once, its like all the last zombies was calling this function 
                // at the same frame, i still dont know what was causing that, probably something to keep in mind and fix it the right way later on
                if (!StaticVariables.levelInBetweenRounds)
                {
                    HUD.Instance.DoRoundTransition();
                    levelManager.MakeRoundSwitchAudio();
                }
            }

            if (zombieGaveDeathPoints == false)
            {
                zombieGaveDeathPoints = true;
                KillZombie();

                // instead of a switch, used expression lol
                var amount = MOD switch
                {
                    KILL_TYPE.BULLETKILL        =>     StaticVariables.normalKillPoints,
                    KILL_TYPE.KNIFEKILL         =>     StaticVariables.knifeKillPoints,
                    KILL_TYPE.EXPLOSIVEKILL     =>     StaticVariables.granadeKillPoints,
                    KILL_TYPE.WONDERWEAPONKILL  =>     StaticVariables.wonderWeaponPoints,
                    KILL_TYPE.OTHER => 0,
                    _ => 0,
                };

                playerScoreScript.playerScore += amount;
                if (StaticVariables.isDoublePointsActive)
                {
                    playerScoreScript.playerScore += amount;
                }
                HUD.Instance.UpdatePlayerScoreHUD(playerScoreScript.playerScore, amount);
            }
        }
        else
        {
            playerScoreScript.playerScore += StaticVariables.damagePoints;
            if (StaticVariables.isDoublePointsActive)
            {
                playerScoreScript.playerScore += StaticVariables.damagePoints;
            }
            HUD.Instance.UpdatePlayerScoreHUD(playerScoreScript.playerScore, StaticVariables.damagePoints);
        }

        if (!hasSpawnedPowerup && (StaticVariables.totalPowerupsSpawnedThisRound < powerups.Count * StaticVariables.maxDropsForPowerup) && StaticVariables.canPowerupsDrop)
        {
            WatchForPowerupDrop();
        }
    }

    private void WatchForPowerupDrop()
    {
        int magicNumber = Random.Range(1, 100);
        
        if (magicNumber <= StaticVariables.powerupDropPercentage)
        {
            StaticVariables.canPowerupsDrop = false;
            hasSpawnedPowerup = true;
            int powerupIndex =  GetRandomPowerup(Random.Range(0, powerups.Count));
            Vector3 position = transform.position + new Vector3(0, 1, 0);
            GameObject powerup = powerups[powerupIndex];
            
            Powerups powerupsScript = levelManager.powerupsHolder.GetComponent<Powerups>();
            powerupsScript.EnablePowerupDrops();
            powerupsScript.SpawnPowerup(powerup, position);
            
            StaticVariables.totalPowerupsSpawnedThisRound++;
        }
    }

    private int GetRandomPowerup(int index)
    {
        GameObject powerup = powerups[index];
        switch(powerup.name.ToLower())
        {
            case "nuke":
                if(StaticVariables.totalNukesSpawned >= 3)
                {
                    index = GetNextIndex(index);
                    return GetRandomPowerup(index);
                }
                else
                {
                    StaticVariables.totalNukesSpawned++;
                    return index;
                }
            case "instakill":
                if (StaticVariables.totalInstakillSpawned >= 3)
                {
                    index = GetNextIndex(index);
                    return GetRandomPowerup(index);
                }
                else
                {
                    StaticVariables.totalInstakillSpawned++;
                    return index;
                }
            case "maxammo":
                if (StaticVariables.totalMaxAmmosSpawned >= 3)
                {
                    index = GetNextIndex(index);
                    return GetRandomPowerup(index);
                }
                else
                {
                    StaticVariables.totalMaxAmmosSpawned++;
                    return index;
                }
            case "doublepoints":
                if (StaticVariables.totaldoublePointsSpawned >= 3)
                {
                    index = GetNextIndex(index);
                    return GetRandomPowerup(index);
                }
                else
                {
                    StaticVariables.totaldoublePointsSpawned++;
                    return index;
                }
            case "firesale":
                if (StaticVariables.totalFiresalesSpawned >= 3)
                {
                    index = GetNextIndex(index);
                    return GetRandomPowerup(index);
                }
                else
                {
                    StaticVariables.totalFiresalesSpawned++;
                    return index;
                }
            default: return index;
        }
    }

    private int GetNextIndex(int index)
    {
        if (index == powerups.Count - 1)
        {
            index = 0;
        }
        else
        {
            index++;
        }
        return index;
    }

    public void KillZombie()
    {
        Vector3 position = transform.position + new Vector3(0, 1.8f, 0);
        int index = Random.Range(0, zombieDeathSounds.Count);

        // since the zombie will get destroyed its self audio source wont be active anymore
        // so we temporary spawn a new one and play the death sound from there
        GameObject deathAudio = Instantiate(deathAudioPrefab, transform.position, Quaternion.identity);
        deathAudio.GetComponent<AudioSource>().PlayOneShot(zombieDeathSounds[index]);
        Destroy(deathAudio, 3f);
        Destroy(transform.parent.gameObject);
        GameObject deathBlood = Instantiate(deathBloodSplatter, position, Quaternion.identity);
        Destroy(deathBlood, 0.2f);
    }


    private void OnTriggerExit(Collider collision)
    {
        string tag = collision.gameObject.tag;
        if (tag.Equals("Player"))
        {
            if (collision.gameObject == transform.parent.GetComponent<Zombie_Movements>().zombieTarget.gameObject)
            {
                zombieTouchingTarget = false;
            }

            zombieIsMoving = true;
            zombieInCombat = false;
            zombieScript.zombieAnimator.SetBool("ShouldAttack", false);

            int randomisedSound = Random.Range(0, 2);
            if (randomisedSound == 0)
            {
                if (zombieIsPlayingSound)
                {
                    return;
                }
                int index = Random.Range(0, zombieAttackSounds.Count);
                zombieIsPlayingSound = true;
                audioSource.PlayOneShot(zombieAttackSounds[index]);
                StartCoroutine(ReActivateZombieSounds(zombieAttackSounds[index].length));
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        string tag = collision.gameObject.tag;

        if (tag.Equals("Player"))
        {
            if (collision.gameObject.GetComponentInParent<PlayerMovement>().playerInLaststand == false)
            {
                HandleFireAction playerActions = collision.gameObject.GetComponentInChildren<HandleFireAction>();
                if (playerActions != null)
                {
                    if (playerActions.isKnifing && canPlayKnifeDamageSound)
                    {
                        canPlayKnifeDamageSound = false;
                        GameObject targetHand = zombieScript.zombieTarget.GetComponent<PlayerMovement>().playerHand;
                        GameObject blood = Instantiate(bloodSplatter, targetHand.transform.position, Quaternion.identity, transform);
                        Vector2 secondPos = new(targetHand.transform.position.x, targetHand.transform.position.y - 0.5f);
                        GameObject blood2 = Instantiate(bloodSplatter, secondPos, Quaternion.identity, transform);
                        Destroy(blood, 0.35f);
                        Destroy(blood2, 0.35f);
                        if (canDoKnifeDamage == true)
                        {
                            canDoKnifeDamage = false;
                            zombieHealth -= collision.gameObject.GetComponentInChildren<HandleFireAction>().knifeDamage;
                            audioSource.PlayOneShot(zombieGettingKnifed);
                            StartCoroutine(WaitUntilYouCanDoKnifeDamageAgain());
                        }
                        CheckZombieHealth(collision.gameObject.GetComponent<PlayerSoundsAndExtras>(), mod = KILL_TYPE.KNIFEKILL);
                        StartCoroutine(EnableKnifeFlashSound(zombieGettingKnifed.length));
                    }
                }
            }
            else
            {
                zombieScript.zombieAnimator.SetBool("ShouldAttack", false);
            }
        }
    }

    private IEnumerator WaitUntilYouCanDoKnifeDamageAgain()
    {
        yield return new WaitForSeconds(knifeDamageCooldown);
        canDoKnifeDamage = true;
    }

    private IEnumerator EnableKnifeFlashSound(float time)
    {
        yield return new WaitForSeconds(time);
        canPlayKnifeDamageSound = true;
    }

    private IEnumerator PlayZombieMouthSounds()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minStopSoundFrequency, maxStopSoundFrequency));
            if (zombieIsPlayingSound == false)
            {
                // play a speed according to the zombie speed => walk or run
                if (zombieIsCrawler == false)
                {
                    if (zombieScript.isSprinter == false)
                    {
                        if (zombieIsMoving && !zombieIsPlayingSound)
                        {
                            int index = Random.Range(0, zombieWalkingSounds.Count);
                            zombieIsPlayingSound = true;
                            audioSource.PlayOneShot(zombieWalkingSounds[index]);
                            StartCoroutine(ReActivateZombieSounds(zombieWalkingSounds[index].length));
                        }
                    }
                    else
                    {
                        if (zombieIsMoving && !zombieIsPlayingSound)
                        {
                            int index = Random.Range(0, zombieRunningSounds.Count);
                            zombieIsPlayingSound = true;
                            audioSource.PlayOneShot(zombieRunningSounds[index]);
                            StartCoroutine(ReActivateZombieSounds(zombieRunningSounds[index].length));
                        }
                    }
                }
                else
                {
                    int index = Random.Range(0, zombieCrawlingSounds.Count);
                    zombieIsPlayingSound = true;
                    audioSource.PlayOneShot(zombieCrawlingSounds[index]);
                    StartCoroutine(ReActivateZombieSounds(zombieCrawlingSounds[index].length));
                }
            }
        }
    }

    private IEnumerator PlayZombieFootstepSounds()
    {
        // wait one second after the zombie animation spawn is done and after that play the footstep sounds
        Audios audioScript = levelManager.GetComponent<Audios>();
        yield return new WaitForSeconds(1f);
        while (true)
        {
            if (zombieIsMoving)
            {
                List<AudioClip> type = new();
                if (zombieScript.isSprinter == true)
                {
                    if (lastLayerTouched == 7) // grass
                    {
                        type = audioScript.grassRun;
                    }
                    else if (lastLayerTouched == 8) // mud
                    {
                        type = audioScript.mudWalk;
                    }
                    else if (lastLayerTouched == 10) // mud
                    {
                        type = audioScript.woodRun;
                    }
                    else if (lastLayerTouched == 11) // mud
                    {
                        type = audioScript.concreteRun;
                    }
                }
                else
                {
                    if (lastLayerTouched == 7) // grass
                    {
                        type = audioScript.grassWalk;
                    }
                    else if (lastLayerTouched == 8) // mud
                    {
                        type = audioScript.mudWalk;
                    }
                    else if (lastLayerTouched == 10) // wood
                    {
                        type = audioScript.woodWalk;
                    }
                    else if (lastLayerTouched == 11) // concrete
                    {
                        type = audioScript.concreteWalk;
                    }
                }
                yield return new WaitForSeconds(inBetweenNextStepFootRun);
                int index = 0;
                audioSource.PlayOneShot(type[index]);
            }
            yield return null;
        }
    }

    private IEnumerator ReActivateZombieSounds(float soundTime)
    {
        yield return new WaitForSeconds(soundTime);
        zombieIsPlayingSound = false;
    }
}
