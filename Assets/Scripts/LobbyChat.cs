using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyChat : MonoBehaviour
{
    [Header("Text Chat References")]
    public int maxNumberOfMessages;
    public Transform container;
    public GameObject chatTextPrefab;
    public TMP_InputField chatInputField;
    public RectTransform chatRect;
    public Scrollbar chatScrollbar;

    private LobbyManager lobbyManager;

    private void Start()
    {
        lobbyManager = GetComponentInParent<LobbyManager>();
    }

    public void SendMessage()
    {
        if (chatInputField.text == string.Empty || chatInputField.text == "") return;
        GameObject message = Instantiate(chatTextPrefab, container);
        message.GetComponent<TMP_Text>().text = lobbyManager.playerName.text + ": " + chatInputField.text;
        chatInputField.text = "";

        StopCoroutine(nameof(UpdateScroll));
        StartCoroutine(UpdateScroll());

    }

    private IEnumerator UpdateScroll()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        chatScrollbar.value = 0;
    }

    public void ClearChat()
    {
        for(int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }
        lobbyManager.DeleteLobby();
    }
}
