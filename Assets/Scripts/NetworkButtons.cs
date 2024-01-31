using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class NetworkButtons : NetworkBehaviour
{
    private TMP_Text playersCounterText;
    private NetworkVariable<int> playersCounter = new();
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        }
        GUILayout.EndArea();
    }

    private void Update()
    {
        if (!IsServer) return;
        playersCounter.Value = NetworkManager.Singleton.ConnectedClients.Count;
        playersCounterText.text = ("Players: " + playersCounter.Value.ToString());
    }
}
 