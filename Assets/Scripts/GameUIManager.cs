using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public Button pauseButton;
    public TextMeshProUGUI playerNameText;
    public Slider healthBar;
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI timerText;

    public float countdownTime = 300f; // 5 minutes

    private float timeRemaining;
    private bool isPaused = false;

    void Start()
    {
        pauseButton.onClick.AddListener(PauseGame);
        playerNameText.text = "Player";
        levelNameText.text = "Level 1";

        timeRemaining = countdownTime;
    }

    void Update()
    {
        if (!isPaused && timeRemaining > 0f)
        {
            timeRemaining -= Time.unscaledDeltaTime;

            if (timeRemaining < 0f)
                timeRemaining = 0f;

            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            int milliseconds = Mathf.FloorToInt(timeRemaining * 1000f % 1000);

            timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:000}";
        }
    }

    void PauseGame()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void SetHealth(float value)
    {
        healthBar.value = value;
    }

    public void SetPlayerName(string name)
    {
        playerNameText.text = name;
    }

    public void SetLevelName(string level)
    {
        levelNameText.text = level;
    }
}

