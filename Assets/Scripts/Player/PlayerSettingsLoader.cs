using UnityEngine;
using System.IO;

public class PlayerSettingsLoader : MonoBehaviour
{
    public KeyCode moveLeftKey;
    public KeyCode moveRightKey;
    public KeyCode jumpKey;
    public KeyCode fireKey;
    public KeyCode meleeKey;
    public KeyCode reloadKey;
    public KeyCode useKey;
    public KeyCode nextWeaponKey;
    public KeyCode throwPrimaryKey;
    public KeyCode scoreboardKey;
    public KeyCode chatKey;
    public KeyCode slideKey;

    public KeyCode[] keyBindings = new KeyCode[12];

    public static PlayerSettingsLoader Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadSettings();
    }

    void Start()
    {
        LoadSettings();
    }

    public void LoadSettings()
    {
        string jsonFilePath = Path.Combine(Application.streamingAssetsPath, "playerSettings.json");

        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            PlayerSettingsData settingsData = JsonUtility.FromJson<PlayerSettingsData>(json);

            moveLeftKey = GetKeyCodeFromString(settingsData.moveLeftKey);
            moveRightKey = GetKeyCodeFromString(settingsData.moveRightKey);
            jumpKey = GetKeyCodeFromString(settingsData.jumpKey);
            fireKey = GetKeyCodeFromString(settingsData.fireKey);
            meleeKey = GetKeyCodeFromString(settingsData.meleeKey);
            reloadKey = GetKeyCodeFromString(settingsData.reloadKey);
            useKey = GetKeyCodeFromString(settingsData.useKey);
            nextWeaponKey = GetKeyCodeFromString(settingsData.nextWeaponKey);
            throwPrimaryKey = GetKeyCodeFromString(settingsData.throwPrimaryKey);
            scoreboardKey = GetKeyCodeFromString(settingsData.scoreboardKey);
            chatKey = GetKeyCodeFromString(settingsData.chatKey);
            slideKey = GetKeyCodeFromString(settingsData.slideKey);

            keyBindings[0] = moveLeftKey;
            keyBindings[1] = moveRightKey;
            keyBindings[2] = jumpKey;
            keyBindings[3] = fireKey;
            keyBindings[4] = meleeKey;
            keyBindings[5] = reloadKey;
            keyBindings[6] = useKey;
            keyBindings[7] = nextWeaponKey;
            keyBindings[8] = throwPrimaryKey;
            keyBindings[9] = scoreboardKey;
            keyBindings[10] = chatKey;
            keyBindings[11] = slideKey;
        }
        else
        {
            Debug.LogError("Player settings file not found!");
        }
    }

    KeyCode GetKeyCodeFromString(string keyName)
    {
        if (string.IsNullOrEmpty(keyName))
        {
            return KeyCode.None;
        }

        try
        {
            return (KeyCode)System.Enum.Parse(typeof(KeyCode), keyName);
        }
        catch (System.ArgumentException)
        {
            Debug.LogError("Invalid key name: " + keyName);
            return KeyCode.None;
        }
    }

    public void SaveSettings()
    {
        PlayerSettingsData settingsData = new()
        {
            moveLeftKey = keyBindings[0].ToString(),
            moveRightKey = keyBindings[1].ToString(),
            jumpKey = keyBindings[2].ToString(),
            fireKey = keyBindings[3].ToString(),
            meleeKey = keyBindings[4].ToString(),
            reloadKey = keyBindings[5].ToString(),
            useKey = keyBindings[6].ToString(),
            nextWeaponKey = keyBindings[7].ToString(),
            throwPrimaryKey = keyBindings[8].ToString(),
            scoreboardKey = keyBindings[9].ToString(),
            chatKey = keyBindings[10].ToString(),
            slideKey = keyBindings[11].ToString()
        };

        string jsonData = JsonUtility.ToJson(settingsData, true);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "playerSettings.json"), jsonData);

        LoadSettings();
    }

    public void SetDefaultControlSettings()
    {
        string defaultSettingsFilePath = Path.Combine(Application.streamingAssetsPath, "playerDefaultSettings.json");

        if (File.Exists(defaultSettingsFilePath))
        {
            string json = File.ReadAllText(defaultSettingsFilePath);
            PlayerSettingsData defaultSettingsData = JsonUtility.FromJson<PlayerSettingsData>(json);

            moveLeftKey = GetKeyCodeFromString(defaultSettingsData.moveLeftKey);
            moveRightKey = GetKeyCodeFromString(defaultSettingsData.moveRightKey);
            jumpKey = GetKeyCodeFromString(defaultSettingsData.jumpKey);
            fireKey = GetKeyCodeFromString(defaultSettingsData.fireKey);
            meleeKey = GetKeyCodeFromString(defaultSettingsData.meleeKey);
            reloadKey = GetKeyCodeFromString(defaultSettingsData.reloadKey);
            useKey = GetKeyCodeFromString(defaultSettingsData.useKey);
            nextWeaponKey = GetKeyCodeFromString(defaultSettingsData.nextWeaponKey);
            throwPrimaryKey = GetKeyCodeFromString(defaultSettingsData.throwPrimaryKey);
            scoreboardKey = GetKeyCodeFromString(defaultSettingsData.scoreboardKey);
            chatKey = GetKeyCodeFromString(defaultSettingsData.chatKey);
            slideKey = GetKeyCodeFromString(defaultSettingsData.slideKey);

            keyBindings[0] = moveLeftKey;
            keyBindings[1] = moveRightKey;
            keyBindings[2] = jumpKey;
            keyBindings[3] = fireKey;
            keyBindings[4] = meleeKey;
            keyBindings[5] = reloadKey;
            keyBindings[6] = useKey;
            keyBindings[7] = nextWeaponKey;
            keyBindings[8] = throwPrimaryKey;
            keyBindings[9] = scoreboardKey;
            keyBindings[10] = chatKey;
            keyBindings[11] = slideKey;

            SaveSettings();
        }
        else
        {
            Debug.LogError("Default control settings file not found!");
        }
    }
}

[System.Serializable]
public class PlayerSettingsData
{
    public string moveLeftKey;
    public string moveRightKey;
    public string jumpKey;
    public string fireKey;
    public string meleeKey;
    public string reloadKey;
    public string useKey;
    public string nextWeaponKey;
    public string throwPrimaryKey;
    public string scoreboardKey;
    public string chatKey;
    public string slideKey;
}
