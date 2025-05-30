using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private string firstSceneToLoad = "Level1";
    [SerializeField] private Slider loadingBar;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Prevent duplicates of other root objects
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            if (obj != gameObject)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }

    void Start()
    {
        StartCoroutine(LoadSceneAsync(firstSceneToLoad));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            if (loadingBar) loadingBar.value = progress;
            yield return null;
        }

        // Finish loading bar
        if (loadingBar) loadingBar.value = 1f;
        yield return new WaitForSeconds(0.3f); // Small delay for polish

        // Now allow scene to activate
        op.allowSceneActivation = true;
    }
}
