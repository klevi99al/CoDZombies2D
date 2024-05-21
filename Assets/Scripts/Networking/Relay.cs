using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

using UnityEngine;

public class Relay : Singleton<Relay>
{
    private bool isRelayHost = false;

    private byte[] key;
    private byte[] connectionData;
    private byte[] hostConnectionData;
    private byte[] allocationIDBytes;
    private System.Guid allocationID;

    private string ip;
    private int port;

    private async void Start()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized) return;
        DontDestroyOnLoad(gameObject);

        InitializationOptions options = new();
        options.SetProfile(Random.Range(0, 1000).ToString());

        await UnityServices.InitializeAsync(options);
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        await CreateRelay();
    }

    public bool IsHost 
    {
        get { return isRelayHost; }
    }

    public async Task<Allocation> CreateRelay()
    {
        try
        {
            await UnityServices.InitializeAsync();
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Join code: " + joinCode);
            RelayServerData relayServerData = new(allocation,"dtls");

            ip = relayServerData.Endpoint.Address;
            port = relayServerData.Endpoint.Port;

            allocationID = allocation.AllocationId;
            key = allocation.Key;
            connectionData = allocation.ConnectionData;
            allocationIDBytes = allocation.AllocationIdBytes;

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            
            isRelayHost = true;
            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            Debug.Log("Joined Relay with code: " + joinCode);

            RelayServerData relayServerData = new(joinAllocation, "dtls");

            ip = relayServerData.Endpoint.Address;
            port = relayServerData.Endpoint.Port;

            allocationID = joinAllocation.AllocationId;
            key = joinAllocation.Key;
            connectionData = joinAllocation.ConnectionData;
            allocationIDBytes = joinAllocation.AllocationIdBytes;
            hostConnectionData = joinAllocation.HostConnectionData;

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e) 
        {
            Debug.Log(e);
        }
    }

    public (byte[] AllocationId, byte[] Key, byte[] ConnectionData, string dtlsAddress, int dtlsPort) GetHostConnectionInfo()
    {
        return (allocationIDBytes, key, connectionData, ip, port);
    }

    public (byte[] AllocationId, byte[] Key, byte[] ConnectionData, byte[] HostConnectionData, string dtlsAddress, int dtlsPort) GetClientConnectionInfo()
    {
        return (allocationIDBytes, key, hostConnectionData, connectionData, ip, port);
    }
}
