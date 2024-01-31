using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private GameObject playersHolder;

    private void Start()
    {
        CameraCheck();
    }

    private void CameraCheck()
    {
        if(!StaticVariables.isCoopGame)
        {
            GameObject currentPlayer;
            for(int i = 0; i < playersHolder.transform.childCount; i++)
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
                }
            }
        }
    }

    private void Update()
    {
        transform.position = playerTarget.position + offset;
    }
}
