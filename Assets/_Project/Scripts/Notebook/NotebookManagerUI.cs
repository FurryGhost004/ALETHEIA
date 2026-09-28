using System.Collections.Generic;
using UnityEngine;

public class NotebookManagerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform _contentEvidenceParent;
    [SerializeField] private NotebookItemUI _itemPrefab;
    [SerializeField] private NotebookDetailUI _detailUI;

    [Header("Court Trial Reference")]
    [SerializeField] private CourtTrialController _trialController;

    [Header("Double Click Setup")]
    [Tooltip("Thời gian tối đa giữa 2 lần nhấp để tính là Nhấp Đúp (giây)")]
    [SerializeField] private float _doubleClickThreshold = 0.3f;

    private readonly List<NotebookItemUI> _spawnedItems = new List<NotebookItemUI>();
    private KeywordData _currentlySelectedData;
    private float _lastClickTime = 0f;

    private void OnEnable()
    {
        if (PlayerInventory.Instance != null)
        {
            PopulateNotebook(PlayerInventory.Instance.CollectedEvidences);
        }
    }

    public void PopulateNotebook(IReadOnlyList<KeywordData> keywordList)
    {
        ClearList();

        if (keywordList == null || keywordList.Count == 0)
        {
            if (_detailUI != null) _detailUI.ClearDetails();
            _currentlySelectedData = null;
            return;
        }

        for (int i = 0; i < keywordList.Count; i++)
        {
            KeywordData data = keywordList[i];
            NotebookItemUI itemInstance = Instantiate(_itemPrefab, _contentEvidenceParent);
            itemInstance.Setup(data, OnItemClicked);
            _spawnedItems.Add(itemInstance);
        }

        // Mặc định chọn hiển thị bằng chứng đầu tiên
        if (keywordList.Count > 0)
        {
            SelectAndShowDetail(keywordList[0]);
        }
    }

    private void OnItemClicked(KeywordData clickedData)
    {
        float currentTime = Time.time;

        // KIỂM TRA ĐIỀU KIỆN NHẤP 2 LẦN (DOUBLE CLICK):
        // 1. Cùng một bằng chứng đang được chọn
        // 2. Khoảng cách thời gian giữa 2 lần nhấp <= 0.3s
        if (_currentlySelectedData == clickedData && (currentTime - _lastClickTime) <= _doubleClickThreshold)
        {
            // === NHẤP 2 LẦN -> NỘP BẰNG CHỨNG ===
            SubmitEvidence(clickedData);
        }
        else
        {
            // === NHẤP 1 LẦN -> XEM CHI TIẾT ===
            SelectAndShowDetail(clickedData);
        }

        _lastClickTime = currentTime;
    }

    private void SelectAndShowDetail(KeywordData selectedData)
    {
        _currentlySelectedData = selectedData;

        if (_detailUI != null)
        {
            _detailUI.DisplayDetails(selectedData);
        }

        for (int i = 0; i < _spawnedItems.Count; i++)
        {
            _spawnedItems[i].SetSelected(_spawnedItems[i].Data == selectedData);
        }
    }

    private void SubmitEvidence(KeywordData evidenceData)
    {
        if (_trialController != null && evidenceData != null)
        {
            _trialController.SubmitEvidence(evidenceData);
        }
    }

    private void ClearList()
    {
        for (int i = 0; i < _spawnedItems.Count; i++)
        {
            if (_spawnedItems[i] != null)
            {
                Destroy(_spawnedItems[i].gameObject);
            }
        }
        _spawnedItems.Clear();
    }
}