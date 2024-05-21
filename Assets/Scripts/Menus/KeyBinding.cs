using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyBinding : MonoBehaviour
{
    public GameObject controlsHolder;
    
    private bool shouldListenForKeyTyping = false;
    private GameObject currentActiveKey;
    private Key[] keys;
    
    public static KeyBinding Instance;

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
    }

    private void Start()
    {
        keys = controlsHolder.GetComponentsInChildren<Key>();
    }

    private void Update()
    {
        if (shouldListenForKeyTyping)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        shouldListenForKeyTyping = false;
                        TrySetKey(keyCode);
                        break;
                    }
                }
            }
        }
    }

    public void SetKey(GameObject option)
    {

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
    }
}
