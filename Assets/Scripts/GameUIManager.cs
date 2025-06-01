using System.Collections.Generic;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUIManager : PersistentManager<GameUIManager>
{
    public Button pauseButton;
    public GameObject pausePannel;
    public TextMeshProUGUI playerNameText;
    public GameObject playerNameScene;
    
    public Slider healthBar;
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;

    public TMP_InputField playerNameInputField;


    public Slider globalSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [System.Serializable]
    public class ItemIcon
    {
        public string tagName;
        public GameObject iconObject;
    }

    public List<ItemIcon> itemIcons;
    private GameObject currentIcon;

    public GameObject introPopupPanel;
    public Button startGameButton;

    private bool introShown = false;

    public bool HasMask { get; private set; }

    private bool isPaused = false;

    protected override void OnSingletonAwake()
    {

      
        // UI never changes, so we don't need to rebind it
        pauseButton.onClick.AddListener(PauseGame);

        // Load saved volume settings
        float savedVolume = PlayerPrefs.GetFloat("GlobalVolume", 1f);
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);

        globalSlider.value = savedVolume;
        musicSlider.value = savedMusic;
        sfxSlider.value = savedSFX;

        SoundManager.Instance.SetGlobalVolume(savedVolume);
        SoundManager.Instance.SetMusicVolume(savedMusic);
        SoundManager.Instance.SetSFXVolume(savedSFX);

        globalSlider.onValueChanged.AddListener(val =>
        {
            SoundManager.Instance.SetGlobalVolume(val);
            PlayerPrefs.SetFloat("GlobalVolume", val);
        });

        musicSlider.onValueChanged.AddListener(val =>
        {
            SoundManager.Instance.SetMusicVolume(val);
            PlayerPrefs.SetFloat("MusicVolume", val);
        });

        sfxSlider.onValueChanged.AddListener(val =>
        {
            SoundManager.Instance.SetSFXVolume(val);
            PlayerPrefs.SetFloat("SFXVolume", val);
        });

        startGameButton.onClick.AddListener(() =>
        {
            string enteredName = playerNameInputField.text;
            if (string.IsNullOrWhiteSpace(enteredName))
                enteredName = "Player";

            SetPlayerName(GameDataManager.Instance?.PlayerName ?? "Player");

            SetPlayerName(enteredName); // Display it in the HUD
            StartCoroutine(HideIntroPopup());

        });

      




        SceneManager.sceneLoaded += OnSceneLoaded;
        HideAll();
    }

    

    private void OnDestroy()
    {
        if (isPersisted)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetPlayerName("Player");
        SetLevelName(scene.name);
        UpdateScoreDisplay(GameDataManager.Instance?.Score ?? 0);
        UpdateHealthDisplay(GameDataManager.Instance?.Health ?? 100f);

        bool isGameplay = scene.name.StartsWith("niveau");
        if (gameObject.tag == "Canvas")
            gameObject.SetActive(isGameplay);

        Cursor.lockState = CursorLockMode.Locked;

        if (scene.name == "niveau1" && !introShown)
        {
            Time.timeScale = 0;
            introShown = true;
            StartCoroutine(ShowIntroPopup());
        }

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

            timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:000}";
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            PauseGame();

            
        }
    }

    public void PauseGame()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;


        if (pausePannel != null)
            pausePannel.SetActive(isPaused);

        // Lock or unlock cursor
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void SetPlayerName(string name)
    {

        playerNameText.text = name;

        StartCoroutine(SetNameScene(name));
        
    }

    public IEnumerator SetNameScene(string name)
    {
         yield return new WaitForSeconds(3f);
        
        TextMeshPro SceneName = playerNameScene.GetComponentInChildren<TextMeshPro>();

        SceneName.text = name;

        yield return null;
        
    }

    public void SetLevelName(string level)
    {
        levelNameText.text = level;
    }

    public void UpdateScoreDisplay(int newScore)
    {
        scoreText.text = "Score: " + newScore;
    }

    public void UpdateHealthDisplay(float newHealth)
    {
        healthBar.value = newHealth;
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

                if (tag == "Mask")
                    HasMask = true;
                return;
            }
        }

        Debug.LogWarning($"No icon configured for tag: {tag}");
    }

    public void HideAll()
    {
        foreach (var item in itemIcons)
        {
            if (item.tagName == "Flashlight" || item.tagName == "Mask")
                continue;

            if (item.iconObject != null)
                item.iconObject.SetActive(false);
        }

        currentIcon = null;
    }

    private IEnumerator ShowIntroPopup()
    {
        introPopupPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        RectTransform popupTransform = introPopupPanel.GetComponent<RectTransform>();
        popupTransform.localScale = Vector3.zero;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            popupTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            elapsed += Time.unscaledDeltaTime; // use unscaled time because Time.timeScale = 0
            yield return null;
        }

        popupTransform.localScale = Vector3.one;
    }

    private IEnumerator HideIntroPopup()
    {
        RectTransform popupTransform = introPopupPanel.GetComponent<RectTransform>();
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            popupTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        introPopupPanel.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


}
