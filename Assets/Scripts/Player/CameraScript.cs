using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [HideInInspector] public Transform playerTarget;
    [SerializeField] private GameObject playersHolder;

    private void Start()
    {
        CameraCheck();
    }

    private void CameraCheck()
    {
        GameObject currentPlayer;
        for (int i = 0; i < playersHolder.transform.childCount; i++)
        {
            currentPlayer = playersHolder.transform.GetChild(i).gameObject;
            if (i != StaticVariables.selectedCharacterIndex)
            {
                currentPlayer.SetActive(false);
            }
            else
            {
                currentPlayer.SetActive(true);
                currentPlayer.transform.SetSiblingIndex(0);
                playerTarget = currentPlayer.transform;
                Debug.Log("Spawned from solo");
            }
        }
    }

    private void Update()
    {
        if (playersHolder.transform.childCount > 0)
        {
            playerTarget = playersHolder.transform.GetChild(0);
            if (playerTarget != null) transform.position = playerTarget.position + offset;
        }
    }
}
