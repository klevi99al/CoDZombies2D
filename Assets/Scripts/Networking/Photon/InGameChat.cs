using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Collections;

public class InGameChat : MonoBehaviourPun
{
    public Transform messagesTransform;
    private const int maxMessages = 3;
    public TMP_Text typingText;
    private KeyCode chatKey;
    private KeyCode endChatKey = KeyCode.Return;
    private KeyCode escapeKey = KeyCode.Escape;
    [SerializeField] private int maxCharactersPerChat = 70;
    private bool isTyping = false;
    private string currentMessage = "";

    private void Start()
    {
        chatKey = PlayerSettingsLoader.Instance.chatKey;

        foreach (Transform message in messagesTransform)
        {
            message.gameObject.SetActive(false);
            message.gameObject.AddComponent<NetworkChat>().text = message.GetComponentInChildren<TMP_Text>();
        }

        typingText.transform.parent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isTyping && Input.GetKeyDown(chatKey))
        {
            StartTyping();
        }
        else if (isTyping)
        {
            if (Input.GetKeyDown(endChatKey))
            {
                SendMsg(currentMessage);
                StopTyping();
            }
            else if (Input.GetKeyDown(escapeKey))
            {
                StopTyping();
            }
            else
            {
                foreach (char c in Input.inputString)
                {
                    if (c == '\b' && currentMessage.Length > 0)
                    {
                        currentMessage = currentMessage[..^1];
                    }
                    else if (!char.IsControl(c))
                    {
                        currentMessage += c;
                    }

                    if ((PhotonNetwork.NickName + ": " + currentMessage).Length < maxCharactersPerChat)
                    {
                        UpdateTypingMessage(PhotonNetwork.NickName + ": " + currentMessage);
                    }
                }
            }
        }
    }

    private void StartTyping()
    {
        isTyping = true;
        currentMessage = "";
        typingText.transform.parent.gameObject.SetActive(true);
        typingText.text = PhotonNetwork.NickName + ": ";
    }

    private void StopTyping()
    {
        isTyping = false;
        typingText.text = "";
        typingText.transform.parent.gameObject.SetActive(false);
    }

    private void UpdateTypingMessage(string message)
    {
        if (typingText != null)
        {
            typingText.text = message;
        }
    }

    public void SendMsg(string message)
    {
        string nickname = PhotonNetwork.NickName;
        string formattedMessage = $"{nickname}: {message}";

        if (formattedMessage.Length > 70)
        {
            formattedMessage = formattedMessage[..67] + "...";
        }

        photonView.RPC(nameof(ReceiveMessage), RpcTarget.All, formattedMessage);
    }

    [PunRPC]
    private void ReceiveMessage(string message)
    {
        StartCoroutine(ShowMessage(message));
    }

    private IEnumerator ShowMessage(string message)
    {
        // Shift all active messages up by one
        for (int i = 0; i < messagesTransform.childCount - 1; i++)
        {
            Transform currentMessage = messagesTransform.GetChild(i);
            Transform nextMessage = messagesTransform.GetChild(i + 1);

            if (nextMessage.gameObject.activeSelf)
            {
                NetworkChat currentChat = currentMessage.GetComponent<NetworkChat>();
                NetworkChat nextChat = nextMessage.GetComponent<NetworkChat>();
                currentChat.SetMessage(nextChat.text.text);
            }
        }

        // Set the last message slot with the new message
        Transform lastMessage = messagesTransform.GetChild(messagesTransform.childCount - 1);
        NetworkChat lastChat = lastMessage.GetComponent<NetworkChat>();
        lastChat.SetMessage(message);

        yield return null;
    }
}
