using UnityEngine;

public class ScoreManager : PersistentManager<ScoreManager>
{

    public override void Initialize()
    {
         GameUIManager.Instance.UpdateScoreDisplay(GameDataManager.Instance.Score);
    }

    public void AddPoint(int point)
    {
        GameDataManager.Instance?.AddScore(point);
        Debug.Log("Correct!");
        GameUIManager.Instance?.UpdateScoreDisplay(GameDataManager.Instance.Score);
    }

    public void SubtractPoint(int point)
    {
        GameDataManager.Instance?.AddScore(point);
        Debug.Log("Wrong!");
        GameUIManager.Instance?.UpdateScoreDisplay(GameDataManager.Instance.Score);
    }
}


