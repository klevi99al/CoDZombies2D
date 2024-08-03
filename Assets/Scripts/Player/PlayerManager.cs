using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public static PlayerManager Instance;

    public List<NetworkedVariables> players = new List<NetworkedVariables>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterPlayer(NetworkedVariables player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }
    }

    public void UnregisterPlayer(NetworkedVariables player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
        }
    }

    public List<NetworkedVariables> GetAllPlayers()
    {
        return players;
    }

    public NetworkedVariables GetPlayerByActorNumber(int actorNumber)
    {
        foreach (var player in players)
        {
            if (player.photonView.Owner.ActorNumber == actorNumber)
            {
                return player;
            }
        }
        return null;
    }
}
