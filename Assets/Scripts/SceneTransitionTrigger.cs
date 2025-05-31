using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName; // Set this in the Inspector (e.g., "niveau2" or "finale")

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BootstrapLoader.NextSceneToLoad = nextSceneName;
            SceneManager.LoadScene("BootstrapScene");
        }
    }
}


