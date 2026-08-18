using UnityEngine;

public class NotebookToggleUI : MonoBehaviour
{
    [SerializeField] private GameObject _notebookPanel;
    [SerializeField] private NotebookManagerUI _notebookManagerUI;

    private bool _isOpen;

    private void Awake()
    {
        if (_notebookPanel == null)
        {
            _notebookPanel = gameObject;
        }

        SetNotebookState(false);
    }

    public void ToggleNotebook()
    {
        SetNotebookState(!_isOpen);
    }

    private void SetNotebookState(bool isOpen)
    {
        _isOpen = isOpen;

        if (_notebookPanel != null)
        {
            _notebookPanel.SetActive(_isOpen);
        }

        // Khi mở UI Notebook, tải danh sách bằng chứng mới nhất
        if (_isOpen && _notebookManagerUI != null && KeywordManager.Instance != null)
        {
            _notebookManagerUI.PopulateNotebook(KeywordManager.Instance.UnlockedKeywords);
        }

        Cursor.lockState = _isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _isOpen;
    }
}