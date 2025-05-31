using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUIManager : PersistentManager<GameUIManager>
{
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

    public bool HasMask { get; private set; }

    private bool isPaused = false;

    protected override void OnSingletonAwake()
    {
       

        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);

        // Initial fallback setup if already in a scene
        if (GameDataManager.Instance != null)
            UpdateScoreDisplay(GameDataManager.Instance.Score);
         if (GameDataManager.Instance != null)
            UpdateHealthDisplay(GameDataManager.Instance.Health);

        HideAll();    
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (isPersisted)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RebindUI();

        // Update UI with default or scene-relevant values
        SetPlayerName("Player"); // Replace with real player name if needed
        SetLevelName(scene.name); // Could map scene name to display name
        UpdateScoreDisplay(GameDataManager.Instance?.Score ?? 0);
        UpdateHealthDisplay(GameDataManager.Instance?.Health ?? 100f);
    }

    private void Update()
    {
        if (!isPaused && GameDataManager.Instance != null)
        {
            GameDataManager.Instance.UpdateTimer(Time.deltaTime);
            float timeRemaining = GameDataManager.Instance.TimeRemaining;

            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            int milliseconds = Mathf.FloorToInt(timeRemaining * 1000f % 1000);

            if (timerText != null)
                timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:000}";
        }
    }

    private void PauseGame()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    private void RebindUI()
    {
        Debug.Log("Rebinding UI elements...");

        pauseButton = pauseButton ?? GameObject.FindWithTag("PauseButton")?.GetComponent<Button>();
        playerNameText = playerNameText ?? GameObject.FindWithTag("PlayerNameText")?.GetComponent<TextMeshProUGUI>();
        healthBar = healthBar ?? GameObject.FindWithTag("HealthBar")?.GetComponent<Slider>();
        levelNameText = levelNameText ?? GameObject.FindWithTag("LevelNameText")?.GetComponent<TextMeshProUGUI>();
        timerText = timerText ?? GameObject.FindWithTag("TimerText")?.GetComponent<TextMeshProUGUI>();
        scoreText = scoreText ?? GameObject.FindWithTag("ScoreText")?.GetComponent<TextMeshProUGUI>();

        if (!pauseButton) Debug.LogWarning("Missing: PauseButton");
        if (!playerNameText) Debug.LogWarning("Missing: PlayerNameText");
        if (!healthBar) Debug.LogWarning("Missing: HealthBar");
        if (!levelNameText) Debug.LogWarning("Missing: LevelNameText");
        if (!timerText) Debug.LogWarning("Missing: TimerText");
        if (!scoreText) Debug.LogWarning("Missing: ScoreText");

        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);
    }

    public void SetPlayerName(string name)
    {
        if (playerNameText != null)
            playerNameText.text = name;
        else
            Debug.LogWarning("SetPlayerName called but playerNameText is null.");
    }

    public void SetLevelName(string level)
    {
        if (levelNameText != null)
            levelNameText.text = level;
        else
            Debug.LogWarning("SetLevelName called but levelNameText is null.");
    }

    public void UpdateScoreDisplay(int newScore)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + newScore;
        else
            Debug.LogWarning("UpdateScoreDisplay called but scoreText is null.");
    }
    
        public void UpdateHealthDisplay(float newHealth)
    {
        if (healthBar != null)
            healthBar.value = newHealth;
        else
            Debug.LogWarning("UpdateHealthDisplay called but newHealth is null.");
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

                if (tag == "Mask") HasMask = true;
                return;
            }
        }

        Debug.LogWarning($"No icon configured for tag: {tag}");
    }

public void HideAll()
{
    foreach (var item in itemIcons)
    {
        // Skip hiding icons for Flashlight or Mask
        if (item.tagName == "Flashlight" || item.tagName == "Mask")
            continue;

        if (item.iconObject != null)
        {
            item.iconObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"IconObject for tag '{item.tagName}' is null.");
        }
    }

    currentIcon = null;
}

}
