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

    public string lobbyId;

    private void Awake()
    {
        lobbyManager = GetComponentInParent<LobbyManager>();
        currentBackgroundColor = GetComponent<Image>().color;
        currentTextColor = transform.GetChild(0).GetComponent<TMP_Text>().color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        LobbyManager.selectedLobbyIndex = transform.GetSiblingIndex();
        Debug.Log(LobbyManager.selectedLobbyIndex + " is the selected lobby index");
        lobbyManager.currentSelectedLobby = gameObject;

        lobbyManager.currentSelectedLobby.GetComponent<Image>().color = Color.white;
        ChangeTextColor(lobbyManager.currentSelectedLobby.transform, Color.black);

        if (lobbyManager.lastSelectedLobby != null)
        {
            if (!lobbyManager.lastSelectedLobby.Equals(gameObject))
            {
                lobbyManager.lastSelectedLobby.GetComponent<Image>().color = currentBackgroundColor;
                ChangeTextColor(lobbyManager.lastSelectedLobby.transform, currentTextColor);
            }
        }
        lobbyManager.lastSelectedLobby = lobbyManager.currentSelectedLobby;
    }

    private void ChangeTextColor(Transform container, Color color)
    {
        for (int i = 0; i < container.childCount; i++)
        {
            container.GetChild(i).GetComponent<TMP_Text>().color = color;
        }
    }
}
