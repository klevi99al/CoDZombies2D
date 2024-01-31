using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    public void LoadLevel(int index)
    {
        StartCoroutine(LoadGame(index));
    }

    private IEnumerator LoadGame(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        while(!operation.isDone)
        {
            Debug.Log(operation.progress);
            yield return null;
        }
    }
}
