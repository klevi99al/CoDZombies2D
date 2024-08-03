using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyChat : MonoBehaviourPunCallbacks
{
    [Header("Text Chat References")]
    public int maxNumberOfMessages = 50;
    public Transform container;
    public GameObject chatTextPrefab;
    public TMP_InputField chatInputField;
    public RectTransform chatRect;
    public Scrollbar chatScrollbar;

    private void Start()
    {
        chatInputField.onEndEdit.AddListener(delegate { OnEnterPress(); });
    }

    private void OnEnterPress()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendMessageToChat();
        }
    }

    public void SendMessageToChat()
    {
        if (string.IsNullOrWhiteSpace(chatInputField.text)) return;

        string message = PhotonNetwork.NickName + ": " + chatInputField.text;
        photonView.RPC(nameof(ReceiveMessage), RpcTarget.All, message);

        chatInputField.text = "";
    }

    [PunRPC]
    public void ReceiveMessage(string message)
    {
        GameObject messageObject = Instantiate(chatTextPrefab, container);
        messageObject.GetComponent<TMP_Text>().text = message;

        StopCoroutine(nameof(UpdateScroll));
        StartCoroutine(UpdateScroll());

        // Ensure we don't exceed the maximum number of messages
        if (container.childCount > maxNumberOfMessages)
        {
            Destroy(container.GetChild(0).gameObject);
        }
    }

    private IEnumerator UpdateScroll()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        chatScrollbar.value = 0;
    }

    public void ClearChat()
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
}
