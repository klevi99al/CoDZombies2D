using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkUi : NetworkBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TMP_Text playersCountText;

    private NetworkVariable<int> playersNum = new(0, NetworkVariableReadPermission.Everyone);

    private void Awake()
    {
        hostButton.onClick.AddListener(() =>
       {
           Debug.Log("Clicked Host");
           NetworkManager.Singleton.StartHost();
       });

        clientButton.onClick.AddListener(() =>
       {
           Debug.Log("Clicked client");
           NetworkManager.Singleton.StartClient();
       });
    }

    private void Update()
    {
        playersNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
        if (!IsServer) return;
        playersCountText.text = "Players: " + playersNum.Value.ToString();
    }
}
