using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using Unity.Services.Relay;

public class LobbyManager : MonoBehaviour
{
    [Header("References")]
    public int maxLobbyCharacters;
    public TMP_Text playerCount;
    public TMP_Text voiceChatState;
    public TMP_Text mapName;
    public TMP_Text visibility;
    public TMP_InputField playerName;
    public TMP_InputField lobbyName;
    public GameObject lobbyButton;
    public GameObject hostedLobbyView;
    public Transform lobbyContainer;
    public Relay relay;
    public Image isReadyImage;
    public CONNECTION_TYPE CONNECTION;


    [Header("Hosted Lobby References")]
    public GameObject connectedPlayer;
    public Transform connectedPlayersContainer;
    public GameObject startGameButton;

    [Header("Testing Variables")]
    public TMP_Text roomName;
    public TMP_Text roomCode;

    [Header("Player Variables")]
    public int playerIndex = -1;
    public bool isReady = false;


    [HideInInspector] public GameObject currentSelectedLobby;
    [HideInInspector] public GameObject lastSelectedLobby;

    private bool canRefreshLobbies = true;
    private float lobbyRefreshCountdown = 2f;
    private float hearbeatTimer;
    private float lobbyUpdateTimer;
    private float roomUpdateTimer = 2f;
    private string playerID;
    private Lobby hostLobby;
    private Lobby joinedLobby;
    private Lobby previousLobby;

    public static int selectedLobbyIndex = -1;

    private readonly int minPlayers = 2;
    private readonly int maxPlayers = 4;
    private readonly float refreshCountdownLimit = 2f;
    private readonly string defaultLobbyName = "Zombie Host";

    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";

    public static LobbyManager Instance { get; private set; }

    private enum VISIBILITY
    {
        PUBLIC,
        PRIVATE
    }

    public enum CONNECTION_TYPE
    {
        DTLS,
        UDP,
        WSS
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private VISIBILITY VISIBILITY_OPTION = VISIBILITY.PUBLIC;

    private void Start()
    {
        //await AuthenticationService.Instance.SignInAnonymouslyAsync();
        //relay.CreateRelay();

        playerName.onValueChanged.AddListener(delegate
        {
            PlayerPrefs.SetString("PlayerName", playerName.text);
        });

        playerName.text = PlayerPrefs.GetString("PlayerName");
    }

    private void Update()
    {
        if (previousLobby != hostLobby)
        {
            previousLobby = hostLobby;
        }
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();
        HandleRoomUpdate();
        if (!canRefreshLobbies)
        {
            if (lobbyRefreshCountdown > 0)
            {
                lobbyRefreshCountdown -= Time.deltaTime;
            }
            else
            {
                canRefreshLobbies = true;
                lobbyRefreshCountdown = refreshCountdownLimit;
            }
        }
    }

    private bool IsPlayerHost()
    {
        if (hostLobby != null && hostLobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            return true;
        }
        return false;
    }

    public void ResetLobbyIndex()
    {
        for (int i = 0; i < connectedPlayersContainer.childCount; i++)
        {
            Destroy(connectedPlayersContainer.GetChild(i).gameObject);
        }
        selectedLobbyIndex = -1;
    }

    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby != null && IsPlayerHost())
        {
            hearbeatTimer -= Time.deltaTime;
            if (hearbeatTimer <= 0)
            {
                float hearbeatTimerMax = 15f;
                hearbeatTimer = hearbeatTimerMax;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }



    public async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0)
            {
                float lobbyUpdateTimerMax = 15f;
                lobbyUpdateTimer = lobbyUpdateTimerMax;
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
            }
        }
    }

    public void SetPlayerCount()
    {
        int playersNumber = Convert.ToInt32(playerCount.text) + 1;
        if (playersNumber > maxPlayers)
        {
            playersNumber = minPlayers;
        }
        playerCount.text = playersNumber.ToString();
    }

    public void EnableVoiceChat()
    {
        string voiceChat = voiceChatState.text.ToLower();
        if (voiceChat.Equals("on"))
        {
            voiceChatState.text = "OFF";
        }
        else
        {
            voiceChatState.text = "ON";
        }
    }

    public void SetVisibility()
    {
        string currentVisibility = visibility.text.ToLower();
        if (currentVisibility.Equals("public"))
        {
            visibility.text = "Private";
            VISIBILITY_OPTION = VISIBILITY.PRIVATE;
        }
        else
        {
            visibility.text = "Public";
            VISIBILITY_OPTION = VISIBILITY.PUBLIC;
        }
    }

    public void CreateLobby()
    {
        CreateLobbyAsync();
    }

    public void ListLobbies()
    {
        ListLobbiesAsync();
    }

    public void SetLobbyScene()
    {
        SceneManager.LoadScene("Lobby");
    }

    private async void CreateLobbyAsync()
    {
        try
        {
            if (lobbyName.text == string.Empty || lobbyName.text == "")
            {
                lobbyName.text = defaultLobbyName;
            }
            int maxPlayers = Convert.ToInt32(playerCount.text);
            CreateLobbyOptions createLobbyOptions = new()
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {
                        "GameMode", new DataObject(VISIBILITY_OPTION == VISIBILITY.PUBLIC ?  DataObject.VisibilityOptions.Public : DataObject.VisibilityOptions.Private, "Zombiemode")
                    }
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName.text, maxPlayers, createLobbyOptions);
            hostLobby = lobby;
            joinedLobby = lobby;

            OpenLobbyWindow(hostLobby);
            if (NetworkManager.Singleton.IsConnectedClient)
            {
                NetworkManager.Singleton.Shutdown();
            }
            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {
                        KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)
                    }
                }
            });


            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, CONNECTION.ToString().ToLower()));
            //NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.StartHost();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }


    private async void ListLobbiesAsync()
    {
        if (!canRefreshLobbies) return;
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new()
            {
                Count = 25,
                Filters = new List<QueryFilter> { new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            },
                Order = new List<QueryOrder>
                {
                    new(false, QueryOrder.FieldOptions.Created)
                }
            };
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            VisualizeLobbies(queryResponse);
            canRefreshLobbies = false;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobby()
    {
        if (selectedLobbyIndex < 0) return;
        try
        {
            startGameButton.SetActive(false);
            JoinLobbyByIdOptions options = new()
            {
                Player = GetPlayer()
            };
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[selectedLobbyIndex].Id, options);
            joinedLobby = lobby;
            OpenLobbyWindow(joinedLobby);
            if (NetworkManager.Singleton.IsConnectedClient)
            {
                NetworkManager.Singleton.Shutdown();
            }

            // Run these 3 lines ALWAYS before starting a client or a host
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, CONNECTION.ToString().ToLower()));

            NetworkManager.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }


    public void ClearLobbies()
    {
        for (int i = 0; i < lobbyContainer.transform.childCount; i++)
        {
            Destroy(lobbyContainer.transform.GetChild(i).gameObject);
        }
    }

    private void VisualizeLobbies(QueryResponse queryResponse)
    {
        ClearLobbies();

        for (int i = 0; i < queryResponse.Results.Count; i++)
        {
            GameObject lobby = Instantiate(lobbyButton, lobbyContainer);
            lobby.name = queryResponse.Results[i].Name;
            lobby.GetComponent<LobbyPanel>().lobbyId = queryResponse.Results[i].Id;
            if (lobby.transform.GetChild(0).GetComponent<TMP_Text>().text.Length > maxLobbyCharacters)
            {
                lobby.transform.GetChild(0).GetComponent<TMP_Text>().text = lobby.name[maxLobbyCharacters..];
            }
            else
            {
                lobby.transform.GetChild(0).GetComponent<TMP_Text>().text = lobby.name;
            }
            if (lobby.transform.GetChild(1).GetComponent<TMP_Text>().text.Length > maxLobbyCharacters)
            {
                lobby.transform.GetChild(1).GetComponent<TMP_Text>().text = mapName.text[maxLobbyCharacters..];
            }
            else
            {
                lobby.transform.GetChild(1).GetComponent<TMP_Text>().text = mapName.text;
            }
            int currentPLayersNumber = Convert.ToInt32(playerCount.text) - queryResponse.Results[i].AvailableSlots;
            lobby.transform.GetChild(2).GetComponent<TMP_Text>().text = currentPLayersNumber + " / " + playerCount.text;
        }
    }

    public async void JoinLobbyByCode(string code)
    {
        code = lobbyContainer.transform.GetChild(selectedLobbyIndex).GetComponent<LobbyPanel>().lobbyId;
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCode = new()
            {
                Player = GetPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(code, joinLobbyByCode);
            joinedLobby = lobby;
            OpenLobbyWindow(joinedLobby);

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, CONNECTION.ToString().ToLower()));

            NetworkManager.Singleton.StartClient();
            //PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void QuickJoinLobby()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void UpdateLobbyGamemode(string gameMode)
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {
                        "GameMode", new DataObject(VISIBILITY_OPTION == VISIBILITY.PUBLIC ? DataObject.VisibilityOptions.Public : DataObject.VisibilityOptions.Private, gameMode)
                    }
                }
            });
            joinedLobby = hostLobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void UpdatePlayerName(string newPlayerName)
    {
        try
        {
            playerName.text = newPlayerName;

            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {
                        "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, newPlayerName)
                    }
                }
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void UpdatePlayerReadyState(bool state)
    {
        try
        {
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {
                        "IsReady", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, state.ToString())
                    }
                }
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    // TODO: Replace the 1 and kick the player by their real ID
    public async void KickPlayer()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async void MigrateLobbyHost()
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                HostId = joinedLobby.Players[1].Id
            });
            joinedLobby = hostLobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void DeleteLobby()
    {
        DeleteLobbyAsync();
    }

    private async void DeleteLobbyAsync()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void PrintPlayers(Lobby lobby)
    {
        foreach (Player player in lobby.Players)
        {
            Debug.Log("PRINTING PLAYER DETAILS: " + player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    private Player GetPlayer()
    {
        string playerName = PlayerPrefs.GetString("PlayerName");
        if (playerName == null && playerName == "")
        {
            playerName = "UnknownPlayer";
        }
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                {
                    "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)
                }
            }
        };
    }

    private void OpenLobbyWindow(Lobby lobby)
    {
        hostedLobbyView.SetActive(true);
        roomName.text = lobby.Name;
        roomCode.text = lobby.Id;
        for (int i = 0; i < connectedPlayersContainer.transform.childCount; i++)
        {
            DestroyImmediate(connectedPlayersContainer.transform.GetChild(i).gameObject);
        }

        //PrintPlayers(lobby);

        StaticVariables.connectedPlayersNumber = lobby.Players.Count;

        for (int i = 0; i < lobby.Players.Count; i++)
        {
            GameObject player = Instantiate(connectedPlayer, connectedPlayersContainer);
            UpdatePlayerName(playerName.text);
            player.transform.GetChild(0).GetComponent<TMP_Text>().text = lobby.Players[i].Data["PlayerName"].Value;
        }
    }

    private async void HandleRoomUpdate()
    {
        if (hostLobby != null)
        {
            roomUpdateTimer -= Time.deltaTime;
            if (roomUpdateTimer <= 0)
            {
                roomUpdateTimer = 1.1f;
                try
                {
                    Lobby lobby = await LobbyService.Instance.GetLobbyAsync(hostLobby.Id);
                    hostLobby = lobby;
                    foreach (var player in lobby.Players)
                    {

                    }

                    OpenLobbyWindow(hostLobby);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
            }
        }
    }

    public void SetPlayerReadyState(bool state)
    {
        if (state)
        {
            isReadyImage.color = Color.green;
        }
        else
        {
            isReadyImage.color = Color.red;
        }
    }

    public void LoadScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("MainGame", LoadSceneMode.Single);
    }

    /// Relay stuff
    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }


    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }
}

