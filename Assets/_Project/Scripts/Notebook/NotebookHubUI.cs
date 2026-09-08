using UnityEngine;
using UnityEngine.UI;

public class NotebookHubUI : MonoBehaviour
{
    [Header("Main Hub Panel")]
    [SerializeField] private GameObject _mainHubPanel;

    [Header("Sub Panels")]
    [SerializeField] private GameObject _panelEvidenceList;
    [SerializeField] private GameObject _panelSuspectList;

    [Header("Evidence Manager Reference")]
    [SerializeField] private NotebookManagerUI _notebookManagerUI; // Bổ sung Manager này

    [Header("Tab Buttons")]
    [SerializeField] private Button _btnEvidence;
    [SerializeField] private Button _btnSuspect;
    [SerializeField] private Button _btnClose;

    private PlayerInputHandler _playerInputHandler;

    private void Awake()
    {
        if (_btnEvidence != null) _btnEvidence.onClick.AddListener(() => SwitchTab(_panelEvidenceList));
        if (_btnSuspect != null) _btnSuspect.onClick.AddListener(() => SwitchTab(_panelSuspectList));
        if (_btnClose != null) _btnClose.onClick.AddListener(CloseHub);
    }

    private void Start()
    {
        _playerInputHandler = FindFirstObjectByType<PlayerInputHandler>();
        CloseHub();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && IsAnyNotebookUIOpen())
        {
            CloseHub();
        }
    }

    public void ToggleHub()
    {
        if (IsAnyNotebookUIOpen())
        {
            CloseHub();
        }
        else
        {
            OpenHub();
        }
    }

    public void OpenHub()
    {
        if (_panelEvidenceList != null) _panelEvidenceList.SetActive(false);
        if (_panelSuspectList != null) _panelSuspectList.SetActive(false);
        if (_mainHubPanel != null) _mainHubPanel.SetActive(true);

        if (_playerInputHandler != null)
        {
            _playerInputHandler.SetInterrogating(true);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseHub()
    {
        if (_mainHubPanel != null) _mainHubPanel.SetActive(false);
        if (_panelEvidenceList != null) _panelEvidenceList.SetActive(false);
        if (_panelSuspectList != null) _panelSuspectList.SetActive(false);

        if (_playerInputHandler != null)
        {
            _playerInputHandler.SetInterrogating(false);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void SwitchTab(GameObject targetTab)
    {
        if (_mainHubPanel != null) _mainHubPanel.SetActive(false);
        if (_panelEvidenceList != null) _panelEvidenceList.SetActive(false);
        if (_panelSuspectList != null) _panelSuspectList.SetActive(false);

        if (targetTab != null)
        {
            targetTab.SetActive(true);

            // --- ĐÂY LÀ ĐOẠN TẢI BẰNG CHỨNG BẠN CẦN ---
            if (targetTab == _panelEvidenceList)
            {
                if (_notebookManagerUI != null && KeywordManager.Instance != null)
                {
                    _notebookManagerUI.PopulateNotebook(KeywordManager.Instance.UnlockedKeywords);
                }
            }
            // ----------------------------------------
            else if (targetTab == _panelSuspectList)
            {
                SuspectListUI suspectUI = targetTab.GetComponent<SuspectListUI>();
                if (suspectUI != null) suspectUI.RefreshList();
            }
        }
    }

    private bool IsAnyNotebookUIOpen()
    {
        bool isHubOpen = _mainHubPanel != null && _mainHubPanel.activeSelf;
        bool isEvidenceOpen = _panelEvidenceList != null && _panelEvidenceList.activeSelf;
        bool isSuspectOpen = _panelSuspectList != null && _panelSuspectList.activeSelf;

        return isHubOpen || isEvidenceOpen || isSuspectOpen;
    }
}