using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkUi : NetworkBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button serverButton;

    public NetworkVariable<bool> shouldStartGame = new(false);

    private void Start()
    {
        shouldStartGame.Value = false;
    }

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

        serverButton.onClick.AddListener(() =>
        {
            Debug.Log("Clicked server");
            NetworkManager.Singleton.StartServer();
        });
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            NetworkManager.Singleton.StartHost();
        }
        if(Input.GetKeyDown(KeyCode.O))
        {
            NetworkManager.Singleton.StartClient();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            NetworkManager.Singleton.StartServer();
        }
        if(Input.GetKeyDown (KeyCode.M))
        {
            NetworkObject[] myItems = FindObjectsOfType(typeof(NetworkObject)) as NetworkObject[];
            Debug.Log("Found " + myItems.Length + " instances with this script attached");
            foreach (NetworkObject item in myItems)
            {
                Debug.Log(item.gameObject.name);
            }
        }
    }
}
