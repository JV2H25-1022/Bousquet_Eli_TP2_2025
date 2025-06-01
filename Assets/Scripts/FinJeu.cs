using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinJeu : MonoBehaviour
{
    private float totalScore;
    private float displayedTotalScore;
    private float healthScore;
    private float timeScore;

    public TextMeshProUGUI gameResult;
    public Image goodEndingBackground;
    public Image badEndingBackground;
    public TextMeshProUGUI endingText;
    public TextMeshProUGUI scoreText;

    private bool hasWon;

    [System.Serializable]
    public class Result
    {
        public GameObject resultImage;
        public string resultTag;
    }

    public List<Result> scoreResults = new List<Result>();

    [SerializeField] private float _ResultS;
    [SerializeField] private float _ResultA;
    [SerializeField] private float _ResultB;
    [SerializeField] private float _ResultC;
    [SerializeField] private float _ResultD;
    [SerializeField] private float _ResultE;
    [SerializeField] private float _ResultF;

    void Start()
    {
        Time.timeScale = 0;
        displayedTotalScore = 0;
        CalculateScore();
    }

    public void EndingScene(bool resultOfEnding)
    {
        hasWon = resultOfEnding;

        if (hasWon)
        {
            goodEndingBackground.gameObject.SetActive(true);
            gameResult.text = "You won,";
            endingText.text = "You have completed good actions and survived.";
        }
        else
        {
            badEndingBackground.gameObject.SetActive(true);
            gameResult.text = "You lost,";
             endingText.text = "You lost all your health or ran out of time.";

        }
    }

    private void CalculateScore()
    {
        healthScore = GameDataManager.Instance.Health*10;
        timeScore = Mathf.Floor(GameDataManager.Instance.TimeRemaining * 10);

        totalScore = /* healthScore + timeScore + */ GameDataManager.Instance.Score;

        StartCoroutine(DisplayScore());
    }

    private IEnumerator DisplayScore()
    {
        displayedTotalScore = 0;

        while (displayedTotalScore < totalScore)
        {
            displayedTotalScore++;
            scoreText.text = "Score: " + displayedTotalScore;
            yield return new WaitForSecondsRealtime(0.01f);
        }

        ScoreResult();

    }

    private void ScoreResult()
    {

       
        float score = displayedTotalScore;

        Result result = null;

        if (score >= _ResultS) result = scoreResults.Find(r => r.resultTag == "S");
        else if (score >= _ResultA) result = scoreResults.Find(r => r.resultTag == "A");
        else if (score >= _ResultB) result = scoreResults.Find(r => r.resultTag == "B");
        else if (score >= _ResultC) result = scoreResults.Find(r => r.resultTag == "C");
        else if (score >= _ResultD) result = scoreResults.Find(r => r.resultTag == "D");
        else if (score >= _ResultE) result = scoreResults.Find(r => r.resultTag == "E");
        else result = scoreResults.Find(r => r.resultTag == "F");

        if (result != null)
        {
            result.resultImage.SetActive(true);
            EndingScene(score >= _ResultF);
        }
    }

    public void EndGame()
    {
        GameDataManager.Instance.ResetTimer();
        GameDataManager.Instance.ResetHealth();
        GameDataManager.Instance.ResetScore();
    }
}
