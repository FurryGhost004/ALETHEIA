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
        caseSelectPanel.SetActive(false);
        loadGamePanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    // mở case select
    public void selectStartNewCase()
    {
        caseSelectPanel.SetActive(true);
        loadGamePanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    // mở load panel
    public void selectLoad()
    {
        caseSelectPanel.SetActive(false);
        loadGamePanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    // mở setting panel
    public void openSettings()
    {
        caseSelectPanel.SetActive(false);
        loadGamePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // button load trong load panel đưa qua Case 1
    public void loadToCase1()
    {
        SceneManager.LoadScene("Investigation-Case 1");
    }

    // button return dùng chung cho tất cả quay lại main menu 
    public void backToMainMenu()
    {
        displayMenu();
    }


    public void selectExit()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }
}