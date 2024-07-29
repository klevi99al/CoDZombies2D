using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Room : MonoBehaviour, IPointerDownHandler
{
    [Header("References")]
    public TMP_Text roomName;
    public TMP_Text mapName;
    public TMP_Text playerCount;

    private Color currentBackgroundColor;
    private Color currentTextColor;
    private LobbySettings lobbySettings;

    private void Awake()
    {
        lobbySettings = GetComponentInParent<LobbySettings>();
        currentBackgroundColor = GetComponent<Image>().color;
        currentTextColor = transform.GetChild(0).GetComponent<TMP_Text>().color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        lobbySettings.selectedRoom = this;
        lobbySettings.selectedRoom.GetComponent<Image>().color = Color.white;

        ChangeTextColor(lobbySettings.selectedRoom.transform, Color.black);

        if (lobbySettings.selectedRoom != lobbySettings.previousSelectedRoom)
        {
            if (lobbySettings.previousSelectedRoom != null)
            {

                lobbySettings.previousSelectedRoom.GetComponent<Image>().color = currentBackgroundColor;
                ChangeTextColor(lobbySettings.previousSelectedRoom.transform, currentTextColor);
            }
        }
    }

    private void ChangeTextColor(Transform container, Color color)
    {
        for (int i = 0; i < container.childCount; i++)
        {
            container.GetChild(i).GetComponent<TMP_Text>().color = color;
        }
    }
}
