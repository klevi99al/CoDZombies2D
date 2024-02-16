using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkUi : NetworkBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button serverButton;

    private NetworkVariable<int> playersNum = new(0, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<int> shouldStartGame = new(0, NetworkVariableReadPermission.Everyone);

    //private void Awake()
    //{
    //    hostButton.onClick.AddListener(() =>
    //    {
    //       Debug.Log("Clicked Host");
    //       NetworkManager.Singleton.StartHost();
    //    });

    //    clientButton.onClick.AddListener(() =>
    //    {
    //       Debug.Log("Clicked client");
    //       NetworkManager.Singleton.StartClient();
    //    });

    //    serverButton.onClick.AddListener(() =>
    //    {
    //        Debug.Log("Clicked server");
    //        NetworkManager.Singleton.StartServer();
    //    });
    //}
}
