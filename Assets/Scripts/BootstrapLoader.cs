using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootstrapLoader : MonoBehaviour
{
    public static string NextSceneToLoad = "niveau1";

    [SerializeField] private Slider loadingBar;
    [SerializeField] private GameObject[] objectsToPersist;
    [SerializeField] private GameObject loadingCanvas;

    

    private static bool gameInitialized = false;

    void Awake()
    {
        foreach (var obj in objectsToPersist)
        {
            if (obj.transform.parent != null)
            {
                obj.transform.SetParent(null); // Detach to make root
            }

            DontDestroyOnLoad(obj);
        }

        loadingBar.value = 0f;
    }

    void Start()
    {
        if (!gameInitialized)
        {
            GameDataManager.Instance?.Initialize();
            gameInitialized = true;
        }

        StartCoroutine(LoadSceneAsync(NextSceneToLoad));
        Debug.Log("BootstrapLoader loading: " + NextSceneToLoad);
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        // Fill progress bar up to 90%
        while (op.progress < 0.9f)
        {
            loadingBar.value = op.progress;
            yield return null;
        }

        loadingBar.value = 1f;

        // Optional: Wait a bit to show full loading bar
        yield return new WaitForSeconds(0.3f);

        // Wait until critical scene UI exists (Canvas or tagged UI root)
        while (GameObject.FindWithTag("Canvas") == null)
        {
            yield return null;
        }

        // Optional: Wait for next frame to ensure all UI elements are fully spawned
        yield return new WaitForEndOfFrame();

        // Fade out the loading canvas
        CanvasGroup group = loadingCanvas.GetComponent<CanvasGroup>();
        while (group.alpha > 0)
        {
            group.alpha -= Time.deltaTime * 2f;
            yield return null;
        }

        op.allowSceneActivation = true;
    }
}
