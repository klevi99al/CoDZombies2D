using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour, IPointerDownHandler
{
    private LobbyManager lobbyManager;

    private Color currentBackgroundColor;
    private Color currentTextColor;

    private void Awake()
    {
        lobbyManager = GetComponentInParent<LobbyManager>();
        currentBackgroundColor = GetComponent<Image>().color;
        currentTextColor = transform.GetChild(0).GetComponent<TMP_Text>().color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        lobbyManager.currentSelectedLobby = gameObject;

        lobbyManager.currentSelectedLobby.GetComponent<Image>().color = Color.white;
        lobbyManager.currentSelectedLobby.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.black;

        if (lobbyManager.lastSelectedLobby != null)
        {
            if (!lobbyManager.lastSelectedLobby.Equals(gameObject))
            {
                lobbyManager.lastSelectedLobby.GetComponent<Image>().color = currentBackgroundColor;
                lobbyManager.lastSelectedLobby.transform.GetChild(0).GetComponent<TMP_Text>().color = currentTextColor;
            }
        }
        lobbyManager.lastSelectedLobby = lobbyManager.currentSelectedLobby;
    }
}
