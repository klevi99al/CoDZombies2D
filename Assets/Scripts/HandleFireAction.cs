using System.Collections;
using UnityEngine;
using TMPro;

public class HandleFireAction : MonoBehaviour
{
    [Header("Keycodes")]
    public KeyCode fireKey;
    public KeyCode reloadKey;
    public KeyCode grenadeKey;
    public KeyCode nextWeapon;
    public KeyCode knifeKey;

    [Header("Bullet Types")]
    public Transform bulletHolder;
    public GameObject pistolBullet;
    public GameObject rifleBullet;
    public GameObject launcherBullet;
    public GameObject wonderWeaponBullet;
    public GameObject firingEffect;
    public GameObject granadePrefab;
    public GameObject grenadeExplosion;

    [Header("Offsets and Extras")]
    public int granadesNumber;
    public float shotgunBulletAngleOffset;
    public float timeToDestroyBullet;
    public float bulletSpeed;
    public float bulletLauncherSpawnTimer;
    public float knifeAnimationTimer;
    public float granadeAnimationTimer;
    public float granadeThrowForce;
    public float knifeDamage;
    public Transform granadesHolder;
    public Transform granadeSpawnPoint;
    public GameObject primaryHand;
    public GameObject secondaryHand;
    public GameObject doubleTap;

    [Header("Audios")]
    public AudioClip knifeHitNothing;
    public AudioClip grenadeRoll;

    [Header("Animators")]
    public Animator knifeAnimator;
    public Animator granadeAnimator;
    public Animator secondaryHandAnimator;
    public Animator drinkAnimator;

    [HideInInspector] public bool papInPorgress = false;
    [HideInInspector] public bool canCycleWeapons = true;
    [HideInInspector] public bool isReloading = false;
    [HideInInspector] public bool isKnifing;
    [HideInInspector] public bool isThrowingGrenade;
    [HideInInspector] public bool canPerformActions = true;         // we set this so we don't have to check for any action in use and whenever we are doing something
    [HideInInspector] public GameObject weaponEmpty = null;         // instead of disabling everything, knife,shoot,granade, reload we use canPerformActions this instead
     //public GameObject weaponBeingPaped = null;

    private AudioSource audioSource;
    private Weapon_Script weaponScript;
    private PlayerInventory inventory;

    private bool canShoot = true;
    private readonly bool canKnife = true;
    private readonly bool canReload = true;
    private readonly bool canThrowGranade = true;

    [Header("Testing")]
    public TMP_Text debugger;
    public GameObject player;

    private void Start()
    {
        granadesHolder = HUD.Instance.granadesHolder.transform;
        audioSource = GetComponent<AudioSource>();
        weaponScript = transform.GetChild(0).GetComponent<Weapon_Script>();

        HUD.Instance.SwitchWeaponSprite(weaponScript.hudImage);
        HUD.Instance.UpdateAmmoHUD(weaponScript.clipAmmo, weaponScript.reserveAmmo);

        inventory = transform.GetComponentInParent<PlayerInventory>();
        if (primaryHand.transform.GetChild(0).GetComponent<Weapon_Script>() != null)
        {
            inventory.playerWeapons.Add(primaryHand.transform.GetChild(0).gameObject);
        }
        weaponEmpty = transform.GetChild(1).gameObject;

        player = transform.parent.parent.gameObject;
        debugger = HUD.Instance.transform.parent.GetChild(5).GetComponent<TMP_Text>();
    }

    private void Update()
    {
        
            if (canPerformActions && !StaticVariables.gameIsPaused)
            {
                if (Input.GetKey(fireKey))
                {
                    if (canShoot == true)
                    {
                        weaponScript = transform.GetChild(0).GetComponent<Weapon_Script>();
                        if (weaponScript.weaponType == Weapon_Script.WEAPON_TYPE.FULL_AUTO)
                        {
                            canShoot = false;
                            StartCoroutine(CoolDownBeforeYouShootAgain(weaponScript.coolDownShoot));
                            StartCoroutine(WeaponFire());
                        }
                    }
                }

                if (Input.GetKeyDown(fireKey))
                {
                    if (canShoot == true)
                    {
                        weaponScript = transform.GetChild(0).GetComponent<Weapon_Script>();
                        canShoot = false;
                        StartCoroutine(CoolDownBeforeYouShootAgain(weaponScript.coolDownShoot));
                        StartCoroutine(WeaponFire());
                    }
                }

                if (Input.GetKeyDown(nextWeapon) && canCycleWeapons)
                {
                    SwitchWeapon();
                }

                if (Input.GetKey(reloadKey))
                {
                    if (canReload)
                    {
                        weaponScript = transform.GetChild(0).GetComponent<Weapon_Script>();
                        if (weaponScript.clipAmmo != weaponScript.maxClipAmmo && weaponScript.reserveAmmo > 0)
                        {
                            StartCoroutine(HandleWeaponReload());
                        }
                    }
                }

                if (Input.GetKey(knifeKey))
                {
                    if (canKnife)
                    {
                        StartCoroutine(HandleKnifeAnimation());
                    }
                }
                if (Input.GetKeyUp(grenadeKey))
                {
                    if (canThrowGranade && granadesNumber > 0)
                    {
                        StartCoroutine(HandleThrowingGranade());
                    }
                }
            
        }
    }

    public void SwitchWeapon()
    {
        GameObject currentWeapon = transform.GetChild(0).gameObject;
        GameObject secondaryWeapon = transform.GetChild(1).gameObject;

        if (secondaryWeapon.GetComponent<Weapon_Script>() == null)
        {
            return;
        }

        //if (inventory.playerWeapons.Contains(weaponBeingPaped))
        //{
        //    return;
        //}

        currentWeapon.SetActive(false);
        secondaryWeapon.SetActive(true);
        secondaryWeapon.transform.SetSiblingIndex(0);

        weaponScript = transform.GetChild(0).GetComponent<Weapon_Script>();

        HUD.Instance.SwitchWeaponSprite(weaponScript.hudImage);
        HUD.Instance.UpdateAmmoHUD(weaponScript.clipAmmo, weaponScript.reserveAmmo);
    }

    private IEnumerator HandleThrowingGranade()
    {
        granadesNumber--;
        granadesHolder.GetChild(granadesNumber).gameObject.SetActive(false);
        canPerformActions = false;
        secondaryHand.SetActive(false);
        isThrowingGrenade = true;

        granadeAnimator.SetBool("ShouldThrowGranade", true);
        StartCoroutine(ThrowTheGranade());
        yield return new WaitForSeconds(granadeAnimationTimer);
        granadeAnimator.SetBool("ShouldThrowGranade", false);

        isThrowingGrenade = false;
        secondaryHand.SetActive(true);
        canPerformActions = true;
    }

    private IEnumerator ThrowTheGranade()
    {
        GameObject granade = Instantiate(granadePrefab, granadeSpawnPoint.position, Quaternion.Euler(0, 0, -90), bulletHolder);
        granade.GetComponent<Rigidbody>().AddForce(primaryHand.transform.right * 20, ForceMode.Impulse);
        yield return new WaitForSeconds(1.5f);
        GameObject explosion = Instantiate(grenadeExplosion, granade.transform.position, Quaternion.identity, bulletHolder);
        explosion.GetComponent<GrenadeExplosion>().granadeThrower = transform.GetComponentInParent<PlayerMovement>().gameObject;
        Destroy(granade);
        Destroy(explosion, 1f);
    }

    private IEnumerator HandleKnifeAnimation()
    {
        canPerformActions = false;
        secondaryHand.SetActive(false);
        isKnifing = true;

        audioSource.PlayOneShot(knifeHitNothing);
        knifeAnimator.SetBool("ShouldKnife", true);
        yield return new WaitForSeconds(knifeAnimationTimer);
        knifeAnimator.SetBool("ShouldKnife", false);

        isKnifing = false;
        secondaryHand.SetActive(true);
        canPerformActions = true;
    }
    private IEnumerator HandleWeaponReload()
    {
        canPerformActions = false;
        isReloading = true;
        secondaryHand.SetActive(false);
        secondaryHandAnimator.SetBool("ShouldReload", true);
        audioSource.PlayOneShot(weaponScript.reloadSound);
        yield return new WaitForSeconds(weaponScript.reloadSound.length + 0.1f);
        secondaryHandAnimator.SetBool("ShouldReload", false);
        secondaryHand.SetActive(true);
        isReloading = false;
        canPerformActions = true;
        AddBulletsToWeapon();
    }

    private void AddBulletsToWeapon()
    {
        int reserve = weaponScript.reserveAmmo;
        if (reserve > 0)
        {
            int clip = weaponScript.clipAmmo;
            int maxClip = weaponScript.maxClipAmmo;

            if (clip == maxClip)
            {
                return;
            }


            int difference = maxClip - clip;
            if (difference <= reserve)
            {
                weaponScript.reserveAmmo -= difference;
                weaponScript.clipAmmo += difference;
            }
            else
            {
                weaponScript.clipAmmo += weaponScript.reserveAmmo;
                weaponScript.reserveAmmo = 0;
            }
            HUD.Instance.UpdateAmmoHUD(weaponScript.clipAmmo, weaponScript.reserveAmmo);
        }
    }

    private IEnumerator WeaponFire()
    {
        if (weaponScript.clipAmmo > 0)
        {
            GameObject bullet;
            int amount = 0;
            Vector3 rotation = transform.eulerAngles + new Vector3(0, 0, -90);
            Vector3 spawnPos = transform.GetChild(0).GetChild(0).position;
            string bulletName = string.Empty;
            switch (weaponScript.weaponType)
            {
                case Weapon_Script.WEAPON_TYPE.SINGLE_SHOT:
                    amount = 1;
                    bullet = Instantiate(pistolBullet, spawnPos, Quaternion.Euler(rotation), bulletHolder);
                    bulletName = pistolBullet.name;
                    break;
                case Weapon_Script.WEAPON_TYPE.FULL_AUTO:
                    amount = 1;
                    bullet = Instantiate(rifleBullet, spawnPos, Quaternion.Euler(rotation), bulletHolder);
                    bulletName = rifleBullet.name;
                    break;
                case Weapon_Script.WEAPON_TYPE.SEMI_AUTO:
                    amount = weaponScript.clipAmmo;
                    if (amount >= 3)
                    {
                        amount = 3;
                    }
                    else if (amount == 2)
                    {
                        amount = 2;
                    }
                    else
                    {
                        amount = 1;
                    }
                    bullet = Instantiate(rifleBullet, spawnPos, Quaternion.Euler(rotation), bulletHolder);
                    bulletName = rifleBullet.name;
                    break;
                case Weapon_Script.WEAPON_TYPE.LAUNCHER:
                    amount = 1;
                    bullet = transform.GetChild(0).GetChild(0).gameObject;
                    StartCoroutine(LauncherBulletRespawn(bullet.transform.position, bullet.transform.rotation, transform.GetChild(0).GetChild(0)));
                    break;
                case Weapon_Script.WEAPON_TYPE.SHOTGUN:
                    amount = 1;
                    bullet = Instantiate(rifleBullet, spawnPos, Quaternion.Euler(rotation), bulletHolder);
                    bulletName = rifleBullet.name;
                    break;
                default:
                    bullet = new GameObject("Random Bullet Name");
                    break;
            }

            weaponScript.clipAmmo -= amount;
            HUD.Instance.UpdateAmmoHUD(weaponScript.clipAmmo, weaponScript.reserveAmmo);

            if (!weaponScript.isWeaponUpgraded)
            {
                audioSource.PlayOneShot(weaponScript.shootSound);
            }
            else
            {
                audioSource.PlayOneShot(weaponScript.shootSound);
                audioSource.PlayOneShot(weaponScript.shootSoundUpgrade);
            }
            bullet.GetComponent<Bullet>().playerWhoShotTheBullet = transform.GetComponentInParent<PlayerMovement>().gameObject;

            if (bullet != null)
            {
                bullet.transform.GetComponent<Bullet>().bulletDamage = weaponScript.damage;
                if (inventory.playerPerks.Contains(doubleTap))
                {
                    bullet.transform.GetComponent<Bullet>().bulletDamage *= 2;
                }
            }

            // to instantiate the firing effect
            if (weaponScript.weaponType != Weapon_Script.WEAPON_TYPE.LAUNCHER)
            {
                GameObject effect = Instantiate(firingEffect, spawnPos, Quaternion.Euler(rotation.x, rotation.y, rotation.z + 90), bulletHolder);
                Destroy(effect, 0.05f);
            }

            if (weaponScript.weaponType == Weapon_Script.WEAPON_TYPE.SHOTGUN)
            {
                // if a weapon is a shotgun i have put a limit so even if the variable in the inspector says that we can shoot abive 5 bullets, we limit it to spawn max 5 bullets- so if the shotgun can shoot
                // 5 bullets at a time - 1 in the middle, 2 up and 2 down
                // 4 bullets at a time - 2 up and 2 down
                // 3 bullets at a time - 1 in the middle, 1 up and 1 down
                // 2 bullets at a time - 1 up and 1 down
                // 1 bullet - bullet destroyts itself

                if (weaponScript.bulletsToSpawn >= 5)
                {
                    weaponScript.bulletsToSpawn = 5;
                }
                int checkIndex = 3;

                switch (weaponScript.bulletsToSpawn)
                {
                    case 5:
                    case 4:
                        checkIndex = 3;
                        break;
                    case 3:
                    case 2:
                        checkIndex = 2;
                        break;
                    default: break;
                }

                SpawnUpperBullets(bullet, rotation, spawnPos, checkIndex);
                SpawnDownBullets(bullet, rotation, spawnPos, checkIndex);

                if (weaponScript.bulletsToSpawn > 2 && weaponScript.bulletsToSpawn % 2 != 0)
                {
                    StartCoroutine(BulletMovement(bullet));
                }
                else
                {
                    Destroy(bullet);
                }
            }
            else if (weaponScript.weaponType == Weapon_Script.WEAPON_TYPE.LAUNCHER)
            {

            }
            else if (weaponScript.weaponType == Weapon_Script.WEAPON_TYPE.SEMI_AUTO)
            {
                StartCoroutine(SpawnMoreBullets());
                StartCoroutine(BulletMovement(bullet));
            }
            else
            {
                StartCoroutine(BulletMovement(bullet));
            }
        }
        yield return null;
    }

    private IEnumerator SpawnMoreBullets()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPos = transform.GetChild(0).GetChild(0).position;
            Vector3 rotation = transform.eulerAngles + new Vector3(0, 0, -90);
            GameObject bullet = Instantiate(rifleBullet, spawnPos, Quaternion.Euler(rotation), bulletHolder);
            bullet.transform.GetComponent<Bullet>().bulletDamage = transform.GetChild(0).GetComponent<Weapon_Script>().damage;
            bullet.GetComponent<Bullet>().playerWhoShotTheBullet = transform.GetComponentInParent<PlayerMovement>().gameObject;
            if (!weaponScript.isWeaponUpgraded)
            {
                audioSource.PlayOneShot(weaponScript.shootSound);
            }
            else
            {
                audioSource.PlayOneShot(weaponScript.shootSoundUpgrade);
            }
            StartCoroutine(BulletMovement(bullet));
            yield return new WaitForSeconds(transform.GetChild(0).GetComponent<Weapon_Script>().bulletSpawnSpeed);
        }
    }

    private IEnumerator LauncherBulletRespawn(Vector3 position, Quaternion rotation, Transform parent)
    {
        // wait 2 seconds first
        yield return new WaitForSeconds(bulletLauncherSpawnTimer);
        Instantiate(launcherBullet, transform.GetChild(0).GetChild(1).position, Quaternion.identity, parent);
        yield return null;
    }

    private void SpawnUpperBullets(GameObject referenceBullet, Vector3 referenceRotation, Vector3 referencePos, int index)
    {
        for (int i = 1; i < index; i++)
        {
            Vector3 rotation = referenceRotation + new Vector3(0, 0, (i * shotgunBulletAngleOffset));
            GameObject bullet = Instantiate(referenceBullet, referencePos, Quaternion.Euler(rotation), bulletHolder);
            bullet.GetComponent<Bullet>().playerWhoShotTheBullet = transform.GetComponentInParent<PlayerMovement>().gameObject;
            StartCoroutine(BulletMovement(bullet));
        }
    }

    private void SpawnDownBullets(GameObject referenceBullet, Vector3 referenceRotation, Vector3 referencePos, int index)
    {
        for (int i = 1; i < index; i++)
        {
            Vector3 rotation = referenceRotation - new Vector3(0, 0, (i * shotgunBulletAngleOffset));
            GameObject bullet = Instantiate(referenceBullet, referencePos, Quaternion.Euler(rotation), bulletHolder);
            bullet.GetComponent<Bullet>().playerWhoShotTheBullet = transform.GetComponentInParent<PlayerMovement>().gameObject;
            StartCoroutine(BulletMovement(bullet));
        }
    }

    private IEnumerator BulletMovement(GameObject bullet)
    {
        Vector3 target = bullet.transform.up * 100000;
        float elapsedTime = 0f;

        while (elapsedTime < timeToDestroyBullet && bullet != null)
        {
            bullet.transform.position = Vector3.MoveTowards(bullet.transform.position, target, bulletSpeed * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            elapsedTime += Time.deltaTime;
        }
        Destroy(bullet);
        yield return null;
    }
    private IEnumerator CoolDownBeforeYouShootAgain(float time)
    {
        yield return new WaitForSeconds(time);
        canShoot = true;
        yield return null;
    }
}
