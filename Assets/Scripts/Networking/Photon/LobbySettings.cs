using System;
using TMPro;
using UnityEngine;

public class LobbySettings : MonoBehaviour
{
    [Header("Room Visibility Properties")]
    public bool roomVisibility = false;
    public TMP_Text roomVisibilityText;

    [Header("Plyers Number Properties")]
    public int currentPlayers = 2;
    public TMP_Text playersNumberText;

    public TMP_Text playerName;

    public TMP_Text lobbyName;

    private readonly int minPlayers = 2;
    private readonly int maxPlayers = 4;

    public Room selectedRoom;
    public Room previousSelectedRoom;
    public void SetRoomVisibility()
    {
        if(roomVisibilityText.text.ToLower().Equals("public"))
        {
            roomVisibilityText.text = "Private";
            roomVisibility = false;
        }
        else
        {
            roomVisibilityText.text = "Public";
            roomVisibility = true;
        }
    }

    public void SetPlayersNumber()
    {
        currentPlayers++;
        
        if(currentPlayers > maxPlayers)
        {
            currentPlayers = minPlayers;
        }
        
        playersNumberText.text = currentPlayers.ToString();
    }
}
