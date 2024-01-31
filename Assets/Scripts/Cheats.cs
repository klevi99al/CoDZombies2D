using UnityEngine;
using TMPro;

public class Cheats : MonoBehaviour
{
    public KeyCode console;
    public KeyCode executeCommand;
    public TMP_Text cheatsCode;
    public GameObject cheats;
    public bool gameIsPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(console))
        {
            if(cheats.activeSelf)
            {
                cheats.SetActive(false);
            }
            else
            {
                cheats.SetActive(true);
            }
        }

        if(Input.GetKeyDown(executeCommand))
        {
            ExecuteCommand();
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            if(gameIsPaused)
            {
                Time.timeScale = 1;
                gameIsPaused = false;
            }
            else
            {
                Time.timeScale = 0;
                gameIsPaused = true;
            }
        }
    }

    private void ExecuteCommand()
    {
        Debug.Log(cheatsCode.text);
        string cheatCode = cheatsCode.text;
        switch(cheatCode.ToLower())
        {
            case "god":
                break;
            default:
                break;
        }
        cheatsCode.text = string.Empty;
    }

    public void ShowPressedButton()
    {
        //if (Input.anyKeyDown)
        //{
        //    foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        //    {
        //        if (Input.GetKeyDown(keyCode))
        //        {
        //            Debug.Log("Pressed key: " + keyCode.ToString());
        //            break;
        //        }
        //    }
        //}
    }
}
