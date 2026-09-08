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
    [Tooltip("Name of the Main Menu scene to load.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private UIState currentState = UIState.Gameplay;
    private UIState previousStateBeforeNotebook = UIState.Gameplay; // Lưu lại trạng thái trước khi mở Notebook

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
                OpenPauseFromGameplay();
            }
            else if (currentState == UIState.Notebook)
            {
                CloseNotebook();
            }
            else if (currentState == UIState.Pause)
            {
                OnContinuePressed();
            }
            else if (currentState == UIState.Save || currentState == UIState.Load)
            {
                OnSaveReturnPressed(); // Trở lại Menu Pause
            }
        }

        // Bấm Q để mở hoặc đóng Notebook
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentState == UIState.Gameplay)
            {
                OpenNotebookFromGameplay();
            }
            else if (currentState == UIState.Notebook)
            {
                CloseNotebook();
            }
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
        previousStateBeforeNotebook = UIState.Pause;
        currentState = UIState.Notebook;
        SetPanelsActive(pause: false, save: false, load: false, notebook: true);
        Time.timeScale = 0f;
    }

    public void OpenNotebookFromGameplay()
    {
        previousStateBeforeNotebook = UIState.Gameplay;
        currentState = UIState.Notebook;
        SetPanelsActive(pause: false, save: false, load: false, notebook: true);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Đóng Notebook và khôi phục đúng trạng thái trước đó (Gameplay hoặc Pause)
    /// </summary>
    public void CloseNotebook()
    {
        if (previousStateBeforeNotebook == UIState.Pause)
        {
            currentState = UIState.Pause;
            SetPanelsActive(pause: true, save: false, load: false, notebook: false);
            Time.timeScale = 0f;
        }
        else // Trở về Gameplay
        {
            currentState = UIState.Gameplay;
            SetPanelsActive(false, false, false, false);
            Time.timeScale = 1f; // Tiếp tục chạy Game
        }
    }

    // Giữ hàm này để gán vào Nút Back (Button UI) nếu có
    public void OnNotebookReturnPressed()
    {
        CloseNotebook();
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