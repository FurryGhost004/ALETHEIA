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
        Notebook
    }

    [Header("Panels (assign in Inspector)")]
    [Tooltip("The Pause Panel GameObject.")]
    [SerializeField] private GameObject pausePanel;

    [Tooltip("The Save Panel GameObject.")]
    [SerializeField] private GameObject savePanel;

    [Tooltip("The Load Panel GameObject.")]
    [SerializeField] private GameObject loadPanel;

    [Tooltip("The Notebook Panel GameObject.")]
    [SerializeField] private GameObject notebookPanel;

    [Header("Main Menu")]
    [Tooltip("Name of the Main Menu scene to load. Only used if no existing scene loader is hooked up below.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == UIState.Gameplay)
            {
                OpenPauseFromGameplay();
            }
            else if (currentState == UIState.Notebook)
            {
                OnNotebookReturnPressed();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && currentState == UIState.Gameplay)
        {
            OpenNotebookFromGameplay();
        }
    }

    public void OpenPauseFromGameplay()
    {
        currentState = UIState.Pause;
        SetPanelsActive(pause: true, save: false, load: false, notebook: false);
        Time.timeScale = 0f;
    }

    public void OnContinuePressed()
    {
        currentState = UIState.Gameplay;
        SetPanelsActive(false, false, false, false);
        Time.timeScale = 1f;
    }

    public void OnSaveButtonPressed()
    {
        currentState = UIState.Save;
        SetPanelsActive(pause: false, save: true, load: false, notebook: false);
        Time.timeScale = 0f;
    }

    public void OnSaveReturnPressed()
    {
        currentState = UIState.Pause;
        SetPanelsActive(pause: true, save: false, load: false, notebook: false);
        Time.timeScale = 0f;
    }
    public void OnLoadButtonPressed()
    {
        currentState = UIState.Load;
        SetPanelsActive(pause: false, save: false, load: true, notebook: false);
        Time.timeScale = 0f;
    }

    public void OnLoadReturnPressed()
    {
        currentState = UIState.Pause;
        SetPanelsActive(pause: true, save: false, load: false, notebook: false);
        Time.timeScale = 0f;
    }

    public void OnNotebookButtonPressedFromPause()
    {
        currentState = UIState.Notebook;
        SetPanelsActive(pause: false, save: false, load: false, notebook: true);
        Time.timeScale = 0f;
    }

    public void OpenNotebookFromGameplay()
    {
        currentState = UIState.Notebook;
        SetPanelsActive(pause: false, save: false, load: false, notebook: true);
        Time.timeScale = 0f;
    }
    public void OnNotebookReturnPressed()
    {
        currentState = UIState.Pause;
        SetPanelsActive(pause: true, save: false, load: false, notebook: false);
        Time.timeScale = 0f;
    }

    public void OnReturnToMainMenuPressed()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }

    private void SetPanelsActive(bool pause, bool save, bool load, bool notebook)
    {
        if (pausePanel != null) pausePanel.SetActive(pause);
        if (savePanel != null) savePanel.SetActive(save);
        if (loadPanel != null) loadPanel.SetActive(load);
        if (notebookPanel != null) notebookPanel.SetActive(notebook);
    }
}