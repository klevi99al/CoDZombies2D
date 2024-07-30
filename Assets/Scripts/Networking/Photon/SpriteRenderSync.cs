using Photon.Pun;
using UnityEngine;

public class SpriteRendererSync : MonoBehaviour, IPunObservable
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on this GameObject.");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send the active state of the SpriteRenderer
            stream.SendNext(spriteRenderer.enabled);
            Debug.Log("Sending SpriteRenderer state: " + spriteRenderer.enabled);
        }
        else
        {
            // Receive the active state of the SpriteRenderer
            bool state = (bool)stream.ReceiveNext();
            spriteRenderer.enabled = state;
            Debug.Log("Received SpriteRenderer state: " + state);
        }
    }
}
