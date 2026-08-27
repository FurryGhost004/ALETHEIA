using System.Collections.Generic;
using UnityEngine;

public class NotebookManagerUI : MonoBehaviour
{
    [SerializeField] private Transform _contentEvidenceParent;
    [SerializeField] private NotebookItemUI _itemPrefab;
    [SerializeField] private NotebookDetailUI _detailUI;

    private readonly List<NotebookItemUI> _spawnedItems = new List<NotebookItemUI>();

    public void PopulateNotebook(IReadOnlyList<KeywordData> keywordList)
    {
        ClearList();

        if (keywordList == null || keywordList.Count == 0)
        {
            if (_detailUI != null) _detailUI.ClearDetails();
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
            OnItemClicked(keywordList[0]);
        }
    }

    private void OnItemClicked(KeywordData selectedData)
    {
        if (_detailUI != null)
        {
            _detailUI.DisplayDetails(selectedData);
        }

        for (int i = 0; i < _spawnedItems.Count; i++)
        {
            _spawnedItems[i].SetSelected(_spawnedItems[i].Data == selectedData);
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