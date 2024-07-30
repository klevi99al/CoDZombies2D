using Photon.Pun;
using UnityEngine;

public class CameraScript : MonoBehaviourPunCallbacks
{
    [SerializeField] private Vector3 offset;
    public Transform playerTarget;
    public Transform playersHolder;

    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // Set the camera target for the master client
            if (PhotonNetwork.IsMasterClient)
            {
                SetCameraTarget(playersHolder.GetChild(0)); // Assuming master client is the first child
            }
        }
    }

    private void LateUpdate()
    {
        if (playerTarget != null)
        {
            transform.position = playerTarget.position + offset;
        }
        else
        {
            if (playersHolder.childCount > 0)
            {
                SetCameraTarget(playersHolder.GetChild(0));
            }
        }
    }

    private void SetCameraTarget(Transform target)
    {
        playerTarget = target;

        // Inform other clients of the camera target change
        photonView.RPC(nameof(UpdateCameraTarget), RpcTarget.Others, target.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    private void UpdateCameraTarget(int targetViewID)
    {
        PhotonView targetView = PhotonView.Find(targetViewID);
        if (targetView != null)
        {
            playerTarget = targetView.transform;
        }
    }
}
