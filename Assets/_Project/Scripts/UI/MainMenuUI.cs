using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject caseSelectPanel;
    [SerializeField] private GameObject loadGamePanel;
    [SerializeField] private GameObject settingsPanel;


    public void displayMenu()
    {
        AudioManager.Instance.PlaySFX("PanelClose");

        caseSelectPanel.SetActive(false);
        loadGamePanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    // mở case select
    public void selectStartNewCase()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        AudioManager.Instance.PlaySFX("PanelOpen");

        caseSelectPanel.SetActive(true);
        loadGamePanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    // mở load panel
    public void selectLoad()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        AudioManager.Instance.PlaySFX("PanelOpen");

        caseSelectPanel.SetActive(false);
        loadGamePanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    // mở setting panel
    public void openSettings()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        AudioManager.Instance.PlaySFX("PanelOpen");

        caseSelectPanel.SetActive(false);
        loadGamePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // button load trong load panel đưa qua Case 1
    public void loadToCase1()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");

        SceneManager.LoadScene("Investigation-Case 1");
    }

    // button return dùng chung cho tất cả quay lại main menu 
    public void backToMainMenu()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");

        displayMenu();
    }


    public void selectExit()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");

        Debug.Log("Exit Game");
        Application.Quit();
    }
}