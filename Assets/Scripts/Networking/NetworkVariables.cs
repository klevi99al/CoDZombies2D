using Unity.Netcode;

public class NetworkVariables : NetworkBehaviour
{
    public NetworkVariable<int> playersNumber = new(0);
    public NetworkVariable<bool> gameShouldStart = new(true);
}
