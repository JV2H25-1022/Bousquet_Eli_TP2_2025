using UnityEngine;

public class GameDataManager : PersistentManager<GameDataManager>
{
    public int Score { get; private set; }
    public float TimeRemaining { get; private set; }
    public float CountdownTime = 300f; // Or however long you want the total time to be

    public override void Initialize()
    {
        TimeRemaining = CountdownTime;
    }

    public void UpdateTimer(float deltaTime)
    {
        if (TimeRemaining > 0f)
        {
            TimeRemaining -= deltaTime;
            if (TimeRemaining < 0f)
                TimeRemaining = 0f;
        }
    }

    public void AddScore(int points)
    {
        Score += points;
        Debug.Log("Score is now: " + Score);
    }

    public void ResetScore()
    {
        Score = 0;
    }
    
    public void ResetTimer()
{
    TimeRemaining = CountdownTime;
}

}



