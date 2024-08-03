using Photon.Pun;
using UnityEngine;

public class NetworkedVariables : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool isSliding = false; // Networked variable
    public SpriteRenderer[] playerSprites;
    public SpriteRenderer slidingSprite;

    private void Start()
    {
        playerSprites = GetComponent<PlayerMovement>().allSprites;
        slidingSprite = GetComponent<PlayerMovement>().slidingSprite;

        PlayerManager.Instance.RegisterPlayer(this);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send the sliding state of the player
            stream.SendNext(isSliding);
        }
        else
        {
            // Receive the sliding state of the player
            isSliding = (bool)stream.ReceiveNext();
            UpdateSlideState(isSliding);
        }
    }

    [PunRPC]
    public void UpdateSlideState(bool sliding)
    {
        isSliding = sliding;
        SetPlayerSlideVariables(!sliding); // Adjust based on the sliding state
    }

    public void SetSlidingState(bool sliding)
    {
        isSliding = sliding;
        photonView.RPC("UpdateSlideState", RpcTarget.All, sliding);
    }

    private void SetPlayerSlideVariables(bool state)
    {
        // Update the SpriteRenderer state across the network
        foreach (var sprite in playerSprites)
        {
            Color color = sprite.color;
            if (sprite != slidingSprite)
            {

                color.a = state ? 255f : 0f;
            }
            else
            {
                color.a = state ? 0f : 255f;
            }
            sprite.color = color;
        }
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.UnregisterPlayer(this);
    }
}
