using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("References")]
    public GameObject lobbyRoom;
    public GameObject startGameButton;
    public Transform roomListContainer;
    public GameObject roomPrefab;
    public GameObject lobbyClosedScreen;
    public GameObject connectedPlayerPrefab;
    public GameObject connectedPlayersHolder;


    private readonly string hostClosedLobby = "Disconnected from the lobby. Reason: Host Closed the room!";
    private readonly string hostKickedPlayer = "Disconnected from the lobby. Reason: You were kicked from the room";

    private LobbySettings lobbySettings;
    private Dictionary<string, RoomInfo> cachedRoomList = new();

    private void Awake()
    {
        lobbySettings = GetComponentInParent<LobbySettings>();
    }

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            SetNickname();
        }
        else
        {
            SetNickname();
        }
    }

    public void CreateRoom()
    {
        SetNickname();

        RoomOptions roomOptions = new()
        {
            IsVisible = true,
            MaxPlayers = (byte)lobbySettings.currentPlayers,
            IsOpen = lobbySettings.roomVisibility,
        };

        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.JoinOrCreateRoom(lobbySettings.lobbyName.text.ToString(), roomOptions, TypedLobby.Default);
        lobbyRoom.SetActive(true);
        startGameButton.SetActive(true);
    }

    public void QuickJoin()
    {
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinRandomOrCreateRoom();
            lobbyRoom.SetActive(true);
        }
        else
        {
            Debug.LogError("Not connected to Master Server or not in Lobby. Wait for OnConnectedToMaster or OnJoinedLobby.");
        }
    }

    public void JoinRoomInList(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(lobbySettings.selectedRoom.roomName.text.ToString());
        lobbyRoom.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        SetNickname();
        Debug.Log(PhotonNetwork.CountOfPlayersInRooms);
        startGameButton.SetActive(false);
        UpdateConnectedPlayersList();

        //PhotonNetwork.LoadLevel(1); // Uncomment this line to load a scene
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();
        UpdateCachedRoomList(roomList);
        //UpdateRoomListView();
    }

    private void ClearRoomListView()
    {
        foreach (Transform roomObject in roomListContainer)
        {
            Destroy(roomObject.gameObject);
        }
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }
                continue;
            }

            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }

    public void UpdateRoomListView()
    {
        foreach (Transform child in roomListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject roomListing = Instantiate(roomPrefab, roomListContainer);
            Room room = roomListing.GetComponent<Room>();
            room.roomName.text = info.Name;
            room.mapName.text = "Abandoned Village"; // Replace with actual map name if needed
            room.playerCount.text = info.PlayerCount + " / " + info.MaxPlayers;
        }
    }

    public void CloseRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // If the player is the master client, close the room and inform other players
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                Debug.Log("Room closed successfully.");
                InformPlayersToLeave();
            }
            else
            {
                // If the player is not the master client, just leave the room
                PhotonNetwork.LeaveRoom();
                Debug.Log("Left the room.");
            }
        }
        else
        {
            Debug.LogError("Not currently in a room to close.");
        }
    }

    private void InformPlayersToLeave()
    {
        GetComponent<PhotonView>().RPC(nameof(LeaveRoom), RpcTarget.Others);
        PhotonNetwork.LeaveRoom(); // The host also leaves the room
    }

    [PunRPC]
    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        lobbyClosedScreen.SetActive(true);
        lobbyClosedScreen.GetComponentInChildren<TMP_Text>().text = hostClosedLobby;
        Debug.Log("Player has been asked to leave the room.");
    }

    public override void OnConnectedToMaster()
    {
        //PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby successfully.");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(PhotonNetwork.PlayerList.Length > 1);
        }
        else
        {
            startGameButton.SetActive(false);
        }
        UpdateConnectedPlayersList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(PhotonNetwork.PlayerList.Length > 1);
        }
        else
        {
            startGameButton.SetActive(false);
        }
        UpdateConnectedPlayersList();
    }

    private void UpdateConnectedPlayersList()
    {
        for (int i = 1; i < connectedPlayersHolder.transform.childCount; i++)
        {
            Destroy(connectedPlayersHolder.transform.GetChild(i).gameObject);
        }

        // Instantiate a new entry for each player in the room
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListing = Instantiate(connectedPlayerPrefab, connectedPlayersHolder.transform);
            TMP_Text playerNameText = playerListing.GetComponentInChildren<TMP_Text>();
            if (playerNameText != null)
            {
                playerNameText.text = !string.IsNullOrEmpty(player.NickName) ? player.NickName : "Unknown Player";
            }
        }
    }

    public void SetNickname()
    {
        if (lobbySettings != null && lobbySettings.playerName != null)
        {
            PhotonNetwork.NickName = lobbySettings.playerName.text;
        }
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
