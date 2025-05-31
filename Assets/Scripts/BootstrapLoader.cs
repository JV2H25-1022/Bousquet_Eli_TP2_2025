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
                obj.transform.SetParent(null);

            DontDestroyOnLoad(obj);
        }
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

        while (op.progress < 0.9f)
        {
            loadingBar.value = op.progress;
            yield return null;
        }

        loadingBar.value = 1f;
        yield return new WaitForSeconds(0.3f);

        CanvasGroup group = loadingCanvas.GetComponent<CanvasGroup>();
        while (group.alpha > 0)
        {
            group.alpha -= Time.deltaTime * 2f;
            yield return null;
        }

        op.allowSceneActivation = true;
    }
}
