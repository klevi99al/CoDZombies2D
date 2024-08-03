using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonConnectionHandler : MonoBehaviourPunCallbacks
{
    public static PhotonConnectionHandler Instance;

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

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"Disconnected from Photon. Reason: {cause}");
        // Attempt to reconnect after a delay
        Invoke(nameof(Reconnect), 5f);
    }

    private void Reconnect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Reconnect();
        }
    }
}
