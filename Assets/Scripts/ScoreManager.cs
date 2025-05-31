using UnityEngine;

public class ScoreManager : PersistentManager<ScoreManager>
{

    public override void Initialize()
    {
         GameUIManager.Instance.UpdateScoreDisplay(GameDataManager.Instance.Score);
    }

    public void AddPoint()
    {
        GameDataManager.Instance?.AddScore(1);
        Debug.Log("Correct!");
        GameUIManager.Instance?.UpdateScoreDisplay(GameDataManager.Instance.Score);
    }

    public void SubtractPoint()
    {
        GameDataManager.Instance?.AddScore(-1);
        Debug.Log("Wrong!");
        GameUIManager.Instance?.UpdateScoreDisplay(GameDataManager.Instance.Score);
    }
}


