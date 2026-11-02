using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    private enum UIState
    {
        Gameplay,
        Pause,
        Save,
        Load,
        Settings
    }

    [Header("Panels (assign in Inspector)")]
    [Tooltip("The Pause Panel GameObject.")]
    [SerializeField] private GameObject pausePanel;

    [Tooltip("The Save Panel GameObject.")]
    [SerializeField] private GameObject savePanel;

    [Tooltip("The Load Panel GameObject.")]
    [SerializeField] private GameObject loadPanel;

    [Tooltip("The Settings Panel GameObject.")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Main Menu")]
    [Tooltip("Name of the Main Menu scene to load.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Player Input")]
    [Tooltip("Player Input Handler used to control player movement and cursor state.")]
    [SerializeField] private PlayerInputHandler playerInputHandler;

    private UIState currentState = UIState.Gameplay;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SetPanelsActive(false, false, false, false);
        Time.timeScale = 1f;
        currentState = UIState.Gameplay;
    }

    private void Update()
    {
        // Bấm ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == UIState.Gameplay)
            {
                if (playerInputHandler != null && playerInputHandler.IsInteractingOrUILocked)
                {
                    return;
                }

                OpenPauseFromGameplay();
            }
            else if (currentState == UIState.Pause)
            {
                OnContinuePressed();
            }
            else if (currentState == UIState.Save || currentState == UIState.Load)
            {
                OnSaveReturnPressed(); // Trở lại Menu Pause
            }
            else if (currentState == UIState.Settings)
            {
                OnSettingsReturnPressed();
            }
        }

    }

    public void OpenPauseFromGameplay()
    {
        currentState = UIState.Pause;
        SetPanelsActive(pause: true, save: false, load: false, settings: false);
        Time.timeScale = 0f;

        Debug.Log("[PauseManager] Open Pause");

        if (playerInputHandler != null)
        {
            Debug.Log("[PauseManager] PlayerInputHandler FOUND → SetInterrogating(true)");
            playerInputHandler.SetInterrogating(true);
        }
        else
        {
            Debug.LogError("[PauseManager] PlayerInputHandler is NULL!");
        }
    }

    public void OnContinuePressed()
    {
        currentState = UIState.Gameplay;
        SetPanelsActive(false, false, false, false);
        Time.timeScale = 1f;

        if (playerInputHandler != null)
            playerInputHandler.SetInterrogating(false);
    }

    public void OnSaveButtonPressed()
    {
        currentState = UIState.Save;
        SetPanelsActive(pause: false, save: true, load: false, settings: false);
        Time.timeScale = 0f;

        if (playerInputHandler != null)
            playerInputHandler.SetInterrogating(true);
    }

    public void OnSaveReturnPressed()
    {
        currentState = UIState.Pause;
        SetPanelsActive(pause: true, save: false, load: false, settings: false);
        Time.timeScale = 0f;
    }

    public void OnLoadButtonPressed()
    {
        currentState = UIState.Load;
        SetPanelsActive(pause: false, save: false, load: true, settings: false);
        Time.timeScale = 0f;

        if (playerInputHandler != null)
            playerInputHandler.SetInterrogating(true);
    }

    public void OnLoadReturnPressed()
    {
        currentState = UIState.Pause;
        SetPanelsActive(pause: true, save: false, load: false, settings: false);
        Time.timeScale = 0f;
    }

    public void OnSettingsButtonPressed()
    {
        currentState = UIState.Settings;
        SetPanelsActive(pause: false, save: false, load: false, settings: true);

        Time.timeScale = 0f;

        if (playerInputHandler != null)
            playerInputHandler.SetInterrogating(true);
    }

    public void OnSettingsReturnPressed()
    {
        currentState = UIState.Pause;
        SetPanelsActive(pause: true, save: false, load: false, settings: false);
        Time.timeScale = 0f;

        if (playerInputHandler != null)
            playerInputHandler.SetInterrogating(true);
    }

    public void OnReturnToMainMenuPressed()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }

    private void SetPanelsActive(bool pause, bool save, bool load, bool settings)
    {
        if (pausePanel != null) pausePanel.SetActive(pause);
        if (savePanel != null) savePanel.SetActive(save);
        if (loadPanel != null) loadPanel.SetActive(load);
        if (settingsPanel != null) settingsPanel.SetActive(settings);
    }
}
