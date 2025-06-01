using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataManager : PersistentManager<GameDataManager>
{
    public int Score { get; private set; }
    public float Health { get; private set; }
    public float TimeRemaining { get; private set; }
    public float CountdownTime = 300f; // Or however long you want the total time to be

    private FinJeu endGame;

    public override void Initialize()
    {
        TimeRemaining = CountdownTime;
        Health = 100f;
        Score = 0;
    }

    public void UpdateTimer(float deltaTime)
    {
        if (TimeRemaining > 0f)
        {
            TimeRemaining -= deltaTime;
            if (TimeRemaining < 0f)
                TimeRemaining = 0f;
        }

        if (TimeRemaining == 0)
        {

            SceneManager.LoadScene("finale");
            endGame.EndingScene(false);
        }
    }

    public void AddScore(int points)
    {
        Score += points;
        Debug.Log("Score is now: " + Score);
    }

    public void SetHealth(float value)
    {
        Health = Mathf.Clamp(value, 0f, 100f);
    }



    public void ResetScore()
    {
        Score = 0;
    }

    public void ResetTimer()
    {
        TimeRemaining = CountdownTime;
    }

     public void ResetHealth()
    {
        Health = 100f;
    }

}



