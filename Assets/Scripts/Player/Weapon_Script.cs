using UnityEngine;

public class Weapon_Script : MonoBehaviour
{
    [Header("General Variables")]
    public string weaponName;
    public WEAPON_TYPE weaponType;
    public int clipAmmo;
    public int reserveAmmo;
    public int damage;
    public float coolDownShoot;
    public GameObject hudImage;
    public GameObject specialBullet = null;
    public Sprite papSprite;

    [Header("Sounds")]
    public AudioClip shootSound;
    public AudioClip shootSoundUpgrade;
    public AudioClip reloadSound;

    [Header("Semi Auto Variables")]
    public int totalBulletsPerShot = 5;
    public float bulletSpawnSpeed = 0.1f;

    [Header("Shotgun Variables")]
    public int bulletsToSpawn = 5;

    [HideInInspector] public int maxClipAmmo;
    [HideInInspector] public int maxReserveAmmo;
    [HideInInspector] public bool isWeaponUpgraded = false;
    [HideInInspector] public Sprite weaponSprite;
    public enum WEAPON_TYPE
    {
        SINGLE_SHOT,
        SEMI_AUTO,
        FULL_AUTO,
        SHOTGUN,
        LAUNCHER,
        WONDER_WEAPON
    }

    private void Awake()
    {
        maxClipAmmo = clipAmmo;
        maxReserveAmmo = reserveAmmo;
        if (GetComponent<SpriteRenderer>() != null)
        {
            weaponSprite = GetComponent<SpriteRenderer>().sprite;
        }
        
    }
}
