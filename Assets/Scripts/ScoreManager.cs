using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddPoint()
    {
        GameDataManager.Instance.AddScore(1);
        Debug.Log("Correct!");
        GameUIManager.Instance?.UpdateScoreDisplay(GameDataManager.Instance.Score);
    }

    public void SubtractPoint()
    {
        GameDataManager.Instance.AddScore(-1);
        Debug.Log("Wrong!");
        GameUIManager.Instance?.UpdateScoreDisplay(GameDataManager.Instance.Score);
    }
}


