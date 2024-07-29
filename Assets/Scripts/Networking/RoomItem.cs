using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomItem : MonoBehaviour
{
    public TMP_Text roomName;
    LobbyManagerOld manager;

    private void Start()
    {
        manager = FindObjectOfType<LobbyManagerOld>();
    }

    public void SetRoomName(string room_name)
    {
        roomName.text = room_name;
    }
}
