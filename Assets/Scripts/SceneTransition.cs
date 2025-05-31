using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "menu"; // Assign this in Inspector

    // Call this from your "Restart Game" button
    public void RestartGame()
    {
        // Reset all persistent gameplay data
        ScoreManager.Instance?.Initialize();      // Reset score
        GameDataManager.Instance?.Initialize();   // Reset name, time, etc.

        // Load the menu scene
        SceneManager.LoadScene(menuSceneName);
    }
}




