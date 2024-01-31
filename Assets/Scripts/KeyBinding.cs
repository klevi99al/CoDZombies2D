using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

public class KeyBinding : MonoBehaviour
{
    private readonly KeyCode moveRight = KeyCode.D;
    private readonly KeyCode moveLeft = KeyCode.A;
    private readonly KeyCode fireKey = KeyCode.Mouse0;
    private readonly KeyCode jumpKey = KeyCode.Space;
    private readonly KeyCode meleeKey = KeyCode.V;
    private readonly KeyCode reloadKey = KeyCode.R;
    private readonly KeyCode useKey = KeyCode.F;
    private readonly KeyCode weaponSwitchKey = KeyCode.Alpha1;
    private readonly KeyCode throwPrimaryGranadeKey = KeyCode.G;
    private readonly KeyCode scoreboardKey = KeyCode.CapsLock;
    private readonly KeyCode textChatKey = KeyCode.T;

    [Header("Action Names")]
    public TMP_Text left;
    public TMP_Text right;
    public TMP_Text fire;
    public TMP_Text jump;
    public TMP_Text melee;
    public TMP_Text reload;
    public TMP_Text use;
    public TMP_Text weapon_switch;
    public TMP_Text throw_Primary_GranadeKey;
    public TMP_Text scoreboard;
    public TMP_Text chat;

    [Header("Audio Clips")]
    public AudioClip clickSound;
    public AudioClip pressButtonSound;
    public AudioSource source;

    [Header("References")]
    public GameObject keys;
    public GameObject saveSettinsButton;

    public enum INSERT_STATE
    {
        CAN_INSERT,
        CAN_NOT_INSERT
    }

    public INSERT_STATE CURRENT_STATE = INSERT_STATE.CAN_INSERT;


    [HideInInspector] public Dictionary<string, KeyCode> keybinds = new();
    [HideInInspector] public TMP_Text text;

    private GameObject currentKey;
    private GameObject previousKey = null;
    private Color32 normal = new(39, 171, 249, 255);
    private Color32 selected = new(239, 116, 36, 255);

    private int keyCounter = 0;
    private List<string> defaultControls = new();
    private static readonly string settingsFilePath = Application.dataPath + "/Resources/SettingsData.txt";

    private void Start()
    {
        defaultControls.Add("A");
        defaultControls.Add("D");
        defaultControls.Add("Mouse0");
        defaultControls.Add("Space");
        defaultControls.Add("V");
        defaultControls.Add("R");
        defaultControls.Add("F");
        defaultControls.Add("Alpha1");
        defaultControls.Add("G");
        defaultControls.Add("Tab");
        defaultControls.Add("T");

        if (!File.Exists(settingsFilePath))
        {
            File.WriteAllText(settingsFilePath, string.Empty);      // EMPTY TEXT FILE
            SetDefaultControls();
        }

        var file = new FileInfo(settingsFilePath);
        if (file.Length == 0)
        {
            File.WriteAllLines(settingsFilePath, defaultControls.ToArray());  // WRITE DEFAULT KEY CODES
            for (int i = 0; i < keys.transform.childCount; i++)      // UPDATE MENU UI
            {
                keys.transform.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = defaultControls.ElementAt(i);
            }
        }

        keybinds.Add("left", moveLeft);
        keybinds.Add("right", moveRight);
        keybinds.Add("fire", fireKey);
        keybinds.Add("jump", jumpKey);
        keybinds.Add("melee", meleeKey);
        keybinds.Add("reload", reloadKey);
        keybinds.Add("use", useKey);
        keybinds.Add("weapon_switch", weaponSwitchKey);
        keybinds.Add("throw_Primary_GranadeKey", throwPrimaryGranadeKey);
        keybinds.Add("scoreboard", scoreboardKey);
        keybinds.Add("chat", textChatKey);

        keyCounter = keybinds.Count();

        left.text = keybinds["left"].ToString();
        right.text = keybinds["right"].ToString();
        fire.text = keybinds["fire"].ToString();
        jump.text = keybinds["jump"].ToString();
        melee.text = keybinds["melee"].ToString();
        reload.text = keybinds["reload"].ToString();
        use.text = keybinds["use"].ToString();
        weapon_switch.text = keybinds["weapon_switch"].ToString();
        throw_Primary_GranadeKey.text = keybinds["throw_Primary_GranadeKey"].ToString();
        scoreboard.text = keybinds["scoreboard"].ToString();
        chat.text = keybinds["chat"].ToString();

        if (previousKey != currentKey)
        {
            previousKey = currentKey;
        }

        LoadControlsFromFile(settingsFilePath);
        DontDestroyOnLoad(gameObject);
    }

    private void LoadControlsFromFile(string path)
    {
        int counter = 0;
        foreach (KeyValuePair<string, KeyCode> kvp in keybinds.ToList())
        {
            if (counter < keys.transform.childCount)
            {
                string line = File.ReadLines(path).Skip(counter).Take(1).FirstOrDefault(); // skip how many lines we have read(i) and get 1 line after that
                KeyCode code = (KeyCode)Enum.Parse(typeof(KeyCode), line, true);
                keybinds[kvp.Key] = code;
                keys.transform.GetChild(counter).GetChild(0).GetComponent<TMP_Text>().text = code.ToString();
                counter++;
            }
        }
    }

    // if the player has changed a value of the keybindings but he did not click the save button then do this
    public void ResetAllUnsavedData()
    {
        if (currentKey != null)
        {
            currentKey.GetComponent<Image>().color = normal;
        }
        LoadControlsFromFile(settingsFilePath);
        saveSettinsButton.SetActive(false);
    }

    public void SaveSettingsToFile()
    {
        string[] lines = new string[keys.transform.childCount];
        //Debug.Log(lines.Length);
        for (int i = 0; i < lines.Length; i++)
        {
            //Debug.Log(keys.transform.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text);
            lines[i] = keys.transform.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text;
        }
        FileSave(lines);
        saveSettinsButton.SetActive(false);
    }

    public async void FileSave(string[] lines)
    {
        await SaveSettingsToFile(lines);
    }
    public static async Task SaveSettingsToFile(string[] lines)
    {
        using StreamWriter file = new(settingsFilePath);
        foreach (string line in lines)
        {
            await file.WriteLineAsync(line);
        }
    }
    private void OnGUI()
    {
        if (currentKey != null)
        {
            Event e = Event.current;
            if ((e.isKey || e.isMouse) && CURRENT_STATE == INSERT_STATE.CAN_INSERT)
            {
                CURRENT_STATE = INSERT_STATE.CAN_NOT_INSERT;
                if (!saveSettinsButton.activeSelf)
                {
                    saveSettinsButton.SetActive(true);
                }

                if (e.isMouse == true)
                {
                    if (e.button == 0)
                    {
                        keybinds[currentKey.name] = KeyCode.Mouse0;
                        currentKey.transform.GetChild(0).GetComponent<TMP_Text>().text = "Mouse0";
                    }
                    else
                    {
                        keybinds[currentKey.name] = KeyCode.Mouse1;
                        currentKey.transform.GetChild(0).GetComponent<TMP_Text>().text = "Mouse1";
                    }
                }
                else
                {
                    keybinds[currentKey.name] = e.keyCode;
                    currentKey.transform.GetChild(0).GetComponent<TMP_Text>().text = e.keyCode.ToString();
                }
                int index = currentKey.transform.GetSiblingIndex();
                currentKey.GetComponent<Image>().color = normal;
                source.PlayOneShot(pressButtonSound);
                CheckForSameKeyUsedMultipleTimes(e, currentKey, index);
                currentKey.GetComponent<Button>().interactable = true;
                currentKey = null;
                StartCoroutine(WaitAndUpdate());
            }
        }
    }

    private IEnumerator WaitAndUpdate()
    {
        yield return new WaitForSeconds(0.2f);
        CURRENT_STATE = INSERT_STATE.CAN_INSERT;
    }

    private void CheckForSameKeyUsedMultipleTimes(Event pressedKey, GameObject currentKeyPressed, int index)
    {
        for (int i = 0; i < keyCounter - 1; i++)
        {
            if (i == index)
            {
                continue;
            }
            if (keybinds.ElementAt(i).Value == pressedKey.keyCode)
            {
                //Debug.Log(keys[i
                keybinds[keybinds.ElementAt(i).Key] = KeyCode.None;
                keys.transform.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = "none";
            }
        }
    }

    public void ChangeKey(GameObject clicked)
    {
        if (CURRENT_STATE == INSERT_STATE.CAN_INSERT)
        {
            source.PlayOneShot(clickSound);
            clicked.GetComponent<Button>().interactable = false;
            if (currentKey != null)
            {
                currentKey.GetComponent<Image>().color = normal;
            }
            currentKey = clicked;
            currentKey.GetComponent<Image>().color = selected;
        }
    }

    public void CheckForDublicates()
    {
        List<string> lines = new();
        //Debug.Log(lines.Length);
        for (int i = 0; i < keyCounter; i++)
        {
            //Debug.Log(keys.transform.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text);
            lines.Add(keys.transform.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text);
        }
        if (lines.Count != lines.Distinct().Count())
        {
            SetDefaultControls();
        }
    }


    public void SetDefaultControls()
    {
        File.WriteAllLines(settingsFilePath, defaultControls.ToArray());  // WRITE DEFAULT KEY CODES

        for (int i = 0; i < keys.transform.childCount; i++)      // UPDATE MENU UI
        {
            keys.transform.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = defaultControls.ElementAt(i);
        }
    }



    // TO DO
    // fix the issue where when i press the mouse button it does not take it as a button insert
    // no dublicated buttons
}
