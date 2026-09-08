using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CaseSelectUI : MonoBehaviour
{
    [System.Serializable]
    public class CaseData
    {
        [Tooltip("1. Change the case name here.")]
        public string caseName;

        [TextArea(2, 5)]
        [Tooltip("2. Change the case description here.")]
        public string caseDescription;

        [Tooltip("3. Assign this case's preview Sprite in the Inspector.")]
        public Sprite previewImage;

        [Tooltip("4. Change the scene name to load for this case. TEMPORARY placeholder — update when real scene names are finalized.")]
        public string sceneName;
    }

    [Header("Case Data (edit placeholder text/images/scenes here)")]
    [SerializeField]
    private CaseData[] cases = new CaseData[]
    {
        new CaseData
        {
            caseName = "CASE 1",
            caseDescription = "An old case involving a mysterious incident in the city.", // placeholder text, replace later
            sceneName = "UI_Prototype" // placeholder scene name, replace later
        },
        new CaseData
        {
            caseName = "CASE 2",
            caseDescription = "A new investigation leads to another suspicious location.", // placeholder text, replace later
            sceneName = "Investigation-Case 2" // placeholder scene name, replace later
        },
        new CaseData
        {
            caseName = "CASE 3",
            caseDescription = "The final case reveals a deeper mystery.", // placeholder text, replace later
            sceneName = "Investigation-Case 3" // placeholder scene name, replace later
        }
    };

    [Header("Text Display")]
    [SerializeField] private TMP_Text Txt_CaseName;
    [SerializeField] private TMP_Text Txt_CaseDescription;

    [Header("Preview Image")]
    [Tooltip("The Image component on Img_CasePreview_Image (child of Img_CasePreview).")]
    [SerializeField] private Image Img_CasePreview_Image;

    [Header("Buttons")]
    [SerializeField] private GameObject Btn_PreviousCase;
    [SerializeField] private GameObject Btn_NextCase;
    [SerializeField] private GameObject Btn_StartCase;


    private int currentCaseIndex = 0;

    private void OnEnable()
    {
        currentCaseIndex = 0;
        RefreshUI();
    }

    public void NextCase()
    {
        if (currentCaseIndex < cases.Length - 1)
        {
            currentCaseIndex++;
            RefreshUI();
        }
    }

    public void PreviousCase()
    {
        if (currentCaseIndex > 0)
        {
            currentCaseIndex--;
            RefreshUI();
        }
    }

    public void StartSelectedCase()
    {
        string sceneToLoad = cases[currentCaseIndex].sceneName;
        SceneManager.LoadScene(sceneToLoad);
    }

    private void RefreshUI()
    {
        CaseData current = cases[currentCaseIndex];

        if (Txt_CaseName != null) Txt_CaseName.text = current.caseName;
        if (Txt_CaseDescription != null) Txt_CaseDescription.text = current.caseDescription;

        if (Img_CasePreview_Image != null) Img_CasePreview_Image.sprite = current.previewImage;

        bool isFirstCase = currentCaseIndex == 0;
        bool isLastCase = currentCaseIndex == cases.Length - 1;

        if (Btn_PreviousCase != null) Btn_PreviousCase.SetActive(!isFirstCase);
        if (Btn_NextCase != null) Btn_NextCase.SetActive(!isLastCase);

        if (Btn_StartCase != null) Btn_StartCase.SetActive(true);
    }
}