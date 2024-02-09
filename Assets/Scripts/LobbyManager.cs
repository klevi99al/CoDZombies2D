using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System;
using Unity.Tutorials.Core.Editor;

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

    [HideInInspector] public GameObject currentSelectedLobby;
    [HideInInspector] public GameObject lastSelectedLobby;

    private bool canRefreshLobbies = true;
    private float lobbyRefreshCountdown = 1.1f;
    private float hearbeatTimer;
    private float lobbyUpdateTimer;
    private string defaultLobbyName;
    private Lobby hostLobby;
    private Lobby joinedLobby;

    private readonly int minPlayers = 2;
    private readonly int maxPlayers = 4;
    private readonly float refreshCountdownLimit = 2f;

    private enum VISIBILITY
    {
        PUBLIC,
        PRIVATE
    }

    private VISIBILITY VISIBILITY_OPTION = VISIBILITY.PUBLIC;

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in: " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        defaultLobbyName = lobbyName.text;
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();
        if (!canRefreshLobbies)
        {
            if (lobbyRefreshCountdown > 0)
            {
                lobbyRefreshCountdown -= Time.deltaTime;
                Debug.Log("Counting down");
            }
            else
            {
                canRefreshLobbies = true;
                lobbyRefreshCountdown = refreshCountdownLimit;
                Debug.Log("Reseting");
            }
        }
    }

    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            hearbeatTimer -= Time.deltaTime;
            if (hearbeatTimer < 0)
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
                float lobbyUpdateTimerMax = 1.1f;
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
            if (lobbyName.text.IsNullOrWhiteSpace())
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
            joinedLobby = hostLobby;
            PrintPlayers(hostLobby);
            hostedLobbyView.SetActive(true);
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

            // Clear all lobbies
            for (int i = 0; i < lobbyContainer.transform.childCount; i++)
            {
                Destroy(lobbyContainer.transform.GetChild(i).gameObject);
            }

            // Reset the lobbies
            for (int i = 0; i < queryResponse.Results.Count; i++)
            {
                GameObject lobby = Instantiate(lobbyButton, lobbyContainer);
                lobby.name = queryResponse.Results[i].Name;

                lobby.transform.GetChild(0).GetComponent<TMP_Text>().text = lobby.name[maxLobbyCharacters..];
                lobby.transform.GetChild(1).GetComponent<TMP_Text>().text = mapName.text[maxLobbyCharacters..];
                lobby.transform.GetChild(2).GetComponent<TMP_Text>().text = playerCount.text;
            }
            canRefreshLobbies = false;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobby()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobbyByCode(string code)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCode = new()
            {
                Player = GetPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(code, joinLobbyByCode);
            joinedLobby = lobby;
            PrintPlayers(joinedLobby);
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
                        "GameMode", new DataObject(VISIBILITY_OPTION == VISIBILITY.PUBLIC ?  DataObject.VisibilityOptions.Public : DataObject.VisibilityOptions.Private, gameMode)
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

    public async void DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void PrintPlayers(Lobby lobby)
    {
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                {
                    "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName.text)
                }
            }
        };
    }
}
