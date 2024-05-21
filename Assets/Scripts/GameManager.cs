using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //void Start()
    //{
    //    NetworkManager.Singleton.NetworkConfig.ConnectionApproval = false;
    //    if(Relay.Instance.IsHost)
    //    {
    //        NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionApproval;
    //        (byte[] AllocationId, byte[] Key, byte[] ConnectionData, string ip, int port) = Relay.Instance.GetHostConnectionInfo();
    //        NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(ip, (ushort)port, AllocationId, Key, ConnectionData, true);
    //        //NetworkManager.Singleton.StartHost();
    //    }
    //    else
    //    {
    //        NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionApproval;
    //        (byte[] AllocationId, byte[] Key, byte[] ConnectionData, byte[] HostConnectionData , string ip, int port) = Relay.Instance.GetClientConnectionInfo();
    //        NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(ip, (ushort)port, AllocationId, Key, ConnectionData, HostConnectionData, true);
    //        //NetworkManager.Singleton.StartClient();
    //    }
    //}

    //private void ConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    //{
    //    response.Approved = true;
    //    response.CreatePlayerObject = true;
    //    response.Pending = false;
    //}
}
