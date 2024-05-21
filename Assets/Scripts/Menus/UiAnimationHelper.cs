using TMPro;
using UnityEngine;

public class UiAnimationHelper : MonoBehaviour
{
    [Header("References")]
    private Vector3 normalColor;
    private Vector3 hoveredColor;
    private Vector3 selectedColor;

    public bool isSetting = false;              // Graphics, Controls, Sounds, the rest are options of them
    public bool shouldChangeOnlyColor = false;  // use this to only change the color of it, basically it can be something which is not a setting or option
    public string description = string.Empty;
    private TMP_Text element;
    public bool canBeChangable = true;
    [Header("Corners")]
    [SerializeField] private float cornerMovementStartTime;
    [SerializeField] private float cornerMovementDuration = 0.5f;

    [SerializeField] private float cornerOffset = 35;
    public GameObject uiCorners;

    [HideInInspector] public RectTransform leftCorner;
    [HideInInspector] public RectTransform rightCorner;

    [HideInInspector] public Vector2 leftCornerPosition;
    [HideInInspector] public Vector2 rightCornerPosition;
    private bool cornerMovementCompleted = true;

    public TMP_Text connectedOption;


    private void Awake()
    {
        if (uiCorners != null)
        {
            leftCorner = uiCorners.transform.GetChild(0).GetComponent<RectTransform>();
            rightCorner = uiCorners.transform.GetChild(1).GetComponent<RectTransform>();

            leftCornerPosition = leftCorner.anchoredPosition;
            rightCornerPosition = rightCorner.anchoredPosition;
        }
    }

    private void Start()
    {
        normalColor = SettingsMenu.Instance.normalColor;
        hoveredColor = SettingsMenu.Instance.hoveredColor;
        selectedColor = SettingsMenu.Instance.selectedColor;

        element = GetComponent<TMP_Text>();
        if (transform.GetSiblingIndex() != 0)
        {
            if (isSetting)
            {
                element.color = new(normalColor.x / 255, normalColor.y / 255, normalColor.z / 255, 1);
            }
            else
            {
                element.color = new(hoveredColor.x / 255, hoveredColor.y / 255, hoveredColor.z / 255, 1);
            }
        }
    }

    public void CursorEnter()
    {
        Debug.Log(name);
        if (!canBeChangable) return;

        if (shouldChangeOnlyColor)
        {
            element.color = new(selectedColor.x / 255, selectedColor.y / 255, selectedColor.z / 255, 1);
            return;
        }

        if (!isSetting)
        {

            SettingsMenu.Instance.activeOption = gameObject;
            if (SettingsMenu.Instance.lastActiveOption != SettingsMenu.Instance.activeOption)
            {
                if (SettingsMenu.Instance.lastActiveOption != null)
                {
                    UiAnimationHelper option = SettingsMenu.Instance.lastActiveOption.GetComponent<UiAnimationHelper>();
                    if (option.uiCorners != null)
                    {
                        option.leftCorner.anchoredPosition = option.leftCornerPosition;
                        option.rightCorner.anchoredPosition = option.rightCornerPosition;
                    }
                    SettingsMenu.Instance.lastActiveOption.GetComponent<TMP_Text>().color = new(hoveredColor.x / 255, hoveredColor.y / 255, hoveredColor.z / 255, 1);
                    if (SettingsMenu.Instance.lastActiveOption.GetComponent<UiAnimationHelper>().connectedOption != null)
                    {
                        SettingsMenu.Instance.lastActiveOption.GetComponent<UiAnimationHelper>().connectedOption.color = new(hoveredColor.x / 255, hoveredColor.y / 255, hoveredColor.z / 255, 1);
                    }
                    option.uiCorners.SetActive(false);
                }

                SettingsMenu.Instance.lastActiveOption = SettingsMenu.Instance.activeOption;
            }

            uiCorners.SetActive(true);
            cornerMovementCompleted = false;
            leftCorner.anchoredPosition = new Vector2(leftCorner.anchoredPosition.x - cornerOffset, leftCorner.anchoredPosition.y);
            rightCorner.anchoredPosition = new Vector2(rightCorner.anchoredPosition.x + cornerOffset, rightCorner.anchoredPosition.y);

            SettingsMenu.Instance.activeOption.GetComponent<TMP_Text>().color = new(selectedColor.x / 255, selectedColor.y / 255, selectedColor.z / 255, 1);
            if (SettingsMenu.Instance.activeOption.GetComponent<UiAnimationHelper>().connectedOption != null)
            {
                SettingsMenu.Instance.activeOption.GetComponent<UiAnimationHelper>().connectedOption.color = new(selectedColor.x / 255, selectedColor.y / 255, selectedColor.z / 255, 1);
            }

            SettingsMenu.Instance.optionDescription.text = description;
            return;
        }
        else
        {
            if (SettingsMenu.Instance.lastSelectedSetting != gameObject)
            {
                element.color = new(hoveredColor.x / 255, hoveredColor.y / 255, hoveredColor.z / 255, 1);
            }
        }
    }

    public void CursorExit()
    {
        if (!canBeChangable) return;
        if (shouldChangeOnlyColor)
        {
            element.color = Color.yellow;
            return;
        }
        if (SettingsMenu.Instance.lastSelectedSetting != gameObject && isSetting)
        {
            element.color = new(normalColor.x / 255, normalColor.y / 255, normalColor.z / 255, 1);
        }
    }

    public void CursorClicked()
    {
        if (!canBeChangable) return;
        if (shouldChangeOnlyColor)
        {
            return;
        }
        if (isSetting)
        {
            SettingsMenu.Instance.activeSetting = gameObject;
            if (SettingsMenu.Instance.lastSelectedSetting != SettingsMenu.Instance.activeSetting)
            {
                if (SettingsMenu.Instance.lastSelectedSetting != null)
                {
                    SettingsMenu.Instance.lastSelectedSetting.GetComponent<TMP_Text>().color = new(normalColor.x / 255, normalColor.y / 255, normalColor.z / 255, 1);
                }
                SettingsMenu.Instance.lastSelectedSetting = SettingsMenu.Instance.activeSetting;
            }
            element.color = new(selectedColor.x / 255, selectedColor.y / 255, selectedColor.z / 255, 1);
        }
    }


    private void Update()
    {
        if (!cornerMovementCompleted)
        {
            if (uiCorners != null)
            {
                if (cornerMovementStartTime < 0f)
                {
                    cornerMovementStartTime = Time.time;
                }

                float elapsedTime = Time.time - cornerMovementStartTime;
                float progress = Mathf.Clamp01(elapsedTime / cornerMovementDuration);

                leftCorner.anchoredPosition = Vector2.Lerp(leftCorner.anchoredPosition, leftCornerPosition, progress);
                rightCorner.anchoredPosition = Vector2.Lerp(rightCorner.anchoredPosition, rightCornerPosition, progress);

                if (elapsedTime >= cornerMovementDuration)
                {
                    leftCorner.anchoredPosition = leftCornerPosition;
                    rightCorner.anchoredPosition = rightCornerPosition;
                    cornerMovementCompleted = true;
                    cornerMovementStartTime = -1f;
                }
            }
        }
    }
}
