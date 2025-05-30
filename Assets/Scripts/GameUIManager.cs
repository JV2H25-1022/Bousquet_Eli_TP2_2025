using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    public Button pauseButton;
    public TextMeshProUGUI playerNameText;
    public Slider healthBar;
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;

    [System.Serializable]
    public class ItemIcon
    {
        public string tagName;
        public GameObject iconObject;
    }

    public List<ItemIcon> itemIcons;

    private GameObject currentIcon;
    public float countdownTime = 300f;

    private float timeRemaining;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        pauseButton.onClick.AddListener(PauseGame);
        playerNameText.text = "Player";
        levelNameText.text = "Level 1";
        timeRemaining = countdownTime;

        if (GameDataManager.Instance != null)
            UpdateScoreDisplay(GameDataManager.Instance.Score);
    }

    void Update()
    {
        if (!isPaused && timeRemaining > 0f)
        {
            timeRemaining -= Time.unscaledDeltaTime;
            if (timeRemaining < 0f) timeRemaining = 0f;

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

    public void SetHealth(float value) => healthBar.value = value;
    public void SetPlayerName(string name) => playerNameText.text = name;
    public void SetLevelName(string level) => levelNameText.text = level;

    public void UpdateScoreDisplay(int newScore)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + newScore;
    }

    public void ShowIconForTag(string tag)
    {
        HideAll();

        foreach (var item in itemIcons)
        {
            if (item.tagName == tag)
            {
                item.iconObject.SetActive(true);
                currentIcon = item.iconObject;
                return;
            }
        }

        Debug.LogWarning($"No icon configured for tag: {tag}");
    }

    public void HideAll()
    {
        foreach (var item in itemIcons)
        {
            item.iconObject.SetActive(false);
        }

        currentIcon = null;
    }
}
