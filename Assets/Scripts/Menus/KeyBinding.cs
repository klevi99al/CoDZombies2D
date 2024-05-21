using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Collections;

public class KeyBinding : MonoBehaviour
{
    public GameObject controlsHolder;
    
    private bool shouldListenForKeyTyping = false;
    private GameObject currentActiveKey;
    private Key[] keys;
    
    public static KeyBinding Instance;

    private readonly float keyCooldown = 0.1f;
    private float cooldownTimer = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);

        cooldownTimer = keyCooldown;
    }

    private void LateUpdate()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (shouldListenForKeyTyping && cooldownTimer <= 0)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        shouldListenForKeyTyping = false;
                        TrySetKey(keyCode);
                        cooldownTimer = keyCooldown;
                        break;
                    }
                }
            }
        }
    }

    public void SetKey(GameObject option)
    {
        if (cooldownTimer > 0) return;

        currentActiveKey = option;
        TMP_Text text = option.transform.GetChild(0).GetComponent<TMP_Text>();
        text.text = StaticVariables.noKey;

        shouldListenForKeyTyping = true;
    }

    private void TrySetKey(KeyCode key)
    {
        for(int i = 0; i < keys.Length; i++) 
        {
            if (keys[i].keyCode == key)
            {
                GameObject theKey = keys[i].gameObject;
                if (theKey != currentActiveKey)
                {
                    theKey.transform.GetChild(0).GetComponent<TMP_Text>().text = StaticVariables.undefinedKey;
                    keys[i].keyCode = KeyCode.None;
                    break;
                }
            }
        }

        TMP_Text text = currentActiveKey.transform.GetChild(0).GetComponent<TMP_Text>();
        text.text = key.ToString();
        currentActiveKey.GetComponent<Key>().keyCode = key;
        currentActiveKey = null;

        ApplySettings();
        PlayerSettingsLoader.Instance.SaveSettings();
    }

    private void OnEnable()
    {
        keys = controlsHolder.GetComponentsInChildren<Key>();
    }

    public void RetrieveSettings()
    {
        PlayerSettingsLoader settingsLoader = PlayerSettingsLoader.Instance;
        if (settingsLoader != null && settingsLoader.keyBindings != null)
        {
            for (int i = 0; i < Mathf.Min(keys.Length, settingsLoader.keyBindings.Length); i++)
            {
                KeyCode keyCode = settingsLoader.keyBindings[i];
                string keyText = keyCode != KeyCode.None ? keyCode.ToString() : StaticVariables.undefinedKey;
                keys[i].transform.GetChild(0).GetComponent<TMP_Text>().text = keyText;
                keys[i].GetComponent<Key>().keyCode = keyCode;
            }
        }
        else
        {
            Debug.LogError("PlayerSettingsLoader instance or keyBindings array is null.");
        }
    }

    public void ApplySettings()
    {
        PlayerSettingsLoader settingsLoader = PlayerSettingsLoader.Instance;
        if (settingsLoader != null)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                settingsLoader.keyBindings[i] = keys[i].GetComponent<Key>().keyCode;
            }
            Debug.Log("Settings applied successfully.");
        }
        else
        {
            Debug.LogError("PlayerSettingsLoader instance is null.");
        }
    }

}
