using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerMovement : MonoBehaviour
{
    [Header("Body Parts")]
    public GameObject playerShoulder;
    public GameObject playerHead;
    public GameObject playerHand;
    public GameObject firstPlayerHand;
    public GameObject secondPlayerHand;
    public GameObject playerHands;
    public GameObject playerNeck;

    [Header("Animators")]
    public Animator drinkAnimator;
    public Animator playerAnimator;
    public Animator slideAnimator;

    [Header("Keyboard ShortKeys")]
    public KeyCode moveRight;
    public KeyCode moveLeft;
    public KeyCode jumpKey;
    public KeyCode slideKey;
    public KeyCode useButton;

    [Header("Hand Rotation Limit")]
    private int handAngleUpperLimit = 30;
    private int handAngleDownLimit = 330;

    [Header("Head Rotation Limits")]
    public int headUpperLimit;
    public int headDownLimit;

    [Header("Speed Limits")]
    public float movementSpeed;
    public float jumpSpeed;

    [Header("Audios")]
    public AudioClip playerSlide;
    public AudioSource slideSource;
    public AudioSource footAudioSource;

    [Header("Extras")]
    //[SerializeField] private LayerMask groundLayer;
    public float extraHight;
    public float slideForce = 1400f;
    public GameObject levelManager;
    public GameObject weaponsHolder;
    public Rigidbody playerRigidbody;
    public PlayerReferences references;

    [HideInInspector] public float moveValue = 0;
    [HideInInspector] public string playerDirection = "";
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool isDrinking = false;
    [HideInInspector] public bool isSliding = false;
    [HideInInspector] public bool playerInLaststand = false;
    [HideInInspector] public bool canSlide = true;

    private int playerLastLayerTouched = -1;
    private bool canJump = false;
    private BoxCollider boxCollider;
    private Vector3 mouseOnScreen;
    private Vector2 positionOnScreen;
    private PhotonView photonView;
    public readonly float playerZposition = -3.767f;


    private void Awake()
    {
        levelManager = GameObject.Find("LevelManager");
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        playerAnimator = transform.GetChild(1).GetChild(0).GetComponent<Animator>();
        boxCollider = transform.GetComponent<BoxCollider>();
        StartCoroutine(HandlePlayerFootStepSounds());
        SetPlayerControls();
    }

    private void SetPlayerControls()
    {
        moveRight  = PlayerSettingsLoader.Instance.moveRightKey;
        moveLeft   = PlayerSettingsLoader.Instance.moveLeftKey;
        jumpKey    = PlayerSettingsLoader.Instance.jumpKey;
        slideKey   = PlayerSettingsLoader.Instance.slideKey;
        useButton  = PlayerSettingsLoader.Instance.useKey;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        mouseOnScreen = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);

        if (playerInLaststand == false && canMove && !StaticVariables.gameIsPaused)
        {
            HandlePlayerMovement();
            HandlePlayerJump();
            HandlePlayerSlide();
            HandePlayerBodyRotation();
            HandePlayerHeadRotation();
            HandlePlayerHandRotation();
            IsGrounded();
            if (canJump == false)
            {
                footAudioSource.Stop();
            }
        }
    }

    private void HandlePlayerSlide()
    {
        if (moveValue != 0 && canJump && !isSliding && canSlide)
        {
            if (Input.GetKeyDown(slideKey) && references.IsPlayerFocused(true))
            {
                HUD.Instance.SlidingProgress(this);
                isSliding = true;
                canSlide = false;
                slideSource.Play();
                SetPlayerSlideVariables(false);
                slideAnimator.gameObject.SetActive(true);
                slideAnimator.SetBool("Slide", true);
                playerRigidbody.AddForce(transform.forward * slideForce, ForceMode.Acceleration);
            }
        }
    }

    public void SetPlayerSlideVariables(bool state)
    {
        canMove = state;
        references.playerActions.canPerformActions = state;

        // we disable the sprites since if we disable the gameobject itself, the player might have shot and spawn a bullet, it would also stop the bullet movement and break
        // everything else from functioning

        playerNeck.GetComponent<SpriteRenderer>().enabled = state;
        firstPlayerHand.GetComponent<SpriteRenderer>().enabled = state;
        secondPlayerHand.GetComponent<SpriteRenderer>().enabled = state;

        // we need this check when the player has no weapons so it doesnt break
        if (GetComponent<PlayerInventory>().playerWeapons.Count > 0)
        {
            firstPlayerHand.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = state;
        }

        for (int i = 0; i < references.playerHealth.playerSprites.Length; i++)
        {
            references.playerHealth.playerSprites[i].enabled = state;
        }
    }

    public bool IsGrounded()
    {
        bool raycastHit = Physics.BoxCast(boxCollider.bounds.center, transform.lossyScale / 2, Vector2.down, out RaycastHit hit, transform.rotation, extraHight);
        if (raycastHit)
        {
            // This check is needed so for example we collide with the mystery box, perks or such, they actually are just triggers not actual colliders
            if (hit.collider.isTrigger == true)
            {
                canJump = false;
            }
            else
            {
                canJump = true;
            }
        }
        else
        {
            canJump = false;
        }
        return canJump;
    }

    private void HandePlayerBodyRotation()
    {
        if (!isDrinking)
        {
            if (mouseOnScreen.x > positionOnScreen.x)
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, -90, 0);
            }
            //if (playerDirection == "right")
            //{
            //    transform.rotation = Quaternion.Euler(0, 90, 0);
            //}
            //else
            //{
            //    transform.rotation = Quaternion.Euler(0, -90, 0);
            //}
        }
    }

    private void HandePlayerHeadRotation()
    {
        if (!isDrinking)
        {

            Vector3 headAngles = playerHead.transform.eulerAngles;


            // flicker when you point up left side needs to be fixed
            if (mouseOnScreen.y > positionOnScreen.y && mouseOnScreen.x < positionOnScreen.x)
            {
                if (headAngles.z > headUpperLimit)
                {
                    playerHead.transform.eulerAngles = new Vector3(headAngles.x, headAngles.y, headUpperLimit);
                }
            }
            if (mouseOnScreen.y < positionOnScreen.y)
            {
                if (headAngles.z < headDownLimit)
                {
                    playerHead.transform.eulerAngles = new Vector3(headAngles.x, headAngles.y, headDownLimit);
                }
            }
            playerHead.transform.eulerAngles = firstPlayerHand.transform.eulerAngles;
        }
    }

    public void PlayDrinkingAnimation(Sprite sprite = null)
    {
        // change perk sprite
        if (sprite != null)
        {
            drinkAnimator.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sprite;
        }
        isDrinking = true;
        drinkAnimator.SetBool("ShouldDrink", true);
        StartCoroutine(ResetDrinking());
    }

    private IEnumerator ResetDrinking()
    {
        yield return new WaitForSeconds(1f);
        isDrinking = false;
        drinkAnimator.SetBool("ShouldDrink", false);
        yield return new WaitForSeconds(0.5f);
        HandSpriteVisibility(true);
        firstPlayerHand.GetComponent<HandleFireAction>().canPerformActions = true;
        canMove = true;
        drinkAnimator.gameObject.SetActive(false);
    }

    public void HandSpriteVisibility(bool state)
    {
        firstPlayerHand.GetComponent<SpriteRenderer>().enabled = state;
        secondPlayerHand.GetComponent<SpriteRenderer>().enabled = state;

        PlayerInventory inventory = transform.GetComponent<PlayerInventory>();
        for (int i = 0; i < inventory.playerWeapons.Count; i++)
        {
            inventory.playerWeapons[i].GetComponent<SpriteRenderer>().enabled = state;
        }
    }

    private void HandlePlayerHandRotation()
    {
        Vector3 difference = mouseOnScreen - transform.position;
        difference.Normalize();

        float x = 180;
        float y = 90;
        float z = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);

        if (mouseOnScreen.x < positionOnScreen.x)
        {
            z = 180 - z;
        }

        //Debug.Log(z);

        playerHands.transform.localEulerAngles = new(x, y, z);
    }

    private void HandlePlayerMovement()
    {
        float horizontalSpeed = Input.GetAxisRaw("Horizontal") * movementSpeed;
        moveValue = horizontalSpeed;

        playerAnimator.SetFloat("Speed", Mathf.Abs(horizontalSpeed));

        if (canJump == false)
        {
            playerAnimator.SetBool("WalkingBackwards", false);
            playerAnimator.SetFloat("Speed", 0);
        }

        if (Input.GetKey(moveRight)) // D
        {
            if (mouseOnScreen.x > positionOnScreen.x)
            {
                playerAnimator.SetBool("WalkingBackwards", false);
                transform.Translate(movementSpeed * Time.deltaTime * Vector3.forward);
            }
            else
            {
                playerAnimator.SetBool("WalkingBackwards", true);
                transform.Translate(movementSpeed * Time.deltaTime * -Vector3.forward);
            }
        }
        if (Input.GetKey(moveLeft)) // A
        {
            if (mouseOnScreen.x > positionOnScreen.x)
            {
                playerAnimator.SetBool("WalkingBackwards", true);
                transform.Translate(movementSpeed * Time.deltaTime * -Vector3.forward);
            }
            else
            {
                playerAnimator.SetBool("WalkingBackwards", false);
                transform.Translate(movementSpeed * Time.deltaTime * Vector3.forward);
            }
        }
    }

    private void HandlePlayerJump()
    {
        if (Input.GetKeyDown(jumpKey)) // Space
        {
            if (canJump)
            {
                playerRigidbody.AddForce(new Vector2(0f, jumpSpeed), ForceMode.Impulse); // custom jump
            }
        }
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    private IEnumerator HandlePlayerFootStepSounds()
    {
        Audios script = levelManager.GetComponent<Audios>();
        while (true)
        {
            if (!playerInLaststand)
            {
                List<AudioClip> type = new();
                int soundIndex = 0;
                float delay = 0f;
                if (playerLastLayerTouched == 7) // grass
                {
                    type = script.grassRun;
                    soundIndex = 6;
                    delay = 0.33f;
                }
                else if (playerLastLayerTouched == 8) // mud
                {
                    type = script.mudWalk;
                    soundIndex = 0;
                    delay = 0.33f;
                }
                else if (playerLastLayerTouched == 10) // wood
                {
                    type = script.woodRun;
                    soundIndex = 1;
                    delay = 0.35f;
                }
                else if (playerLastLayerTouched == 11) // concrete
                {
                    type = script.concreteRun;
                    soundIndex = 0;
                    delay = 0.33f;
                }

                if (moveValue != 0 && type.Count != 0 && !isSliding)
                {
                    footAudioSource.PlayOneShot(type[soundIndex]);
                    yield return new WaitForSeconds(delay);
                }
            }
            yield return null;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Layers 7 = Grass, 8 = Mud, 10 = wood
        if (collision.gameObject.layer == 7)
        {
            playerLastLayerTouched = 7;
        }
        if (collision.gameObject.layer == 8)
        {
            playerLastLayerTouched = 8;
        }
        if (collision.gameObject.layer == 10)
        {
            playerLastLayerTouched = 10;
        }
        if (collision.gameObject.layer == 11)
        {
            playerLastLayerTouched = 11;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            BoxCollider myCollider = GetComponent<BoxCollider>();
            BoxCollider otherCollider = collision.gameObject.GetComponent<BoxCollider>();

            Physics.IgnoreCollision(myCollider, otherCollider);
        }
    }
}
