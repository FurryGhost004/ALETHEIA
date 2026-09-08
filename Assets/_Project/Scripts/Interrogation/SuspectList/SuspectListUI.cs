using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuspectListUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform _contentSuspectParent; // Content_Suspect trong ScrollView
    [SerializeField] private SuspectItemUI _suspectItemPrefab; // PF_SuspectList Prefab

    [Header("Detail Panel References (Bảng bên phải)")]
    [SerializeField] private TextMeshProUGUI _txtName;
    

    private readonly List<SuspectItemUI> _spawnedItems = new List<SuspectItemUI>();

    private void OnEnable()
    {
        // Tự động làm mới danh sách khi bật Panel UI nghi phạm
        RefreshList();
    }

    public void RefreshList()
    {
        ClearList();

        if (SuspectManager.Instance == null) return;

        IReadOnlyList<SuspectData> list = SuspectManager.Instance.DiscoveredSuspects;

        if (_suspectItemPrefab == null || _contentSuspectParent == null)
        {
            Debug.LogError("[SuspectListUI] Thiếu Prefab hoặc Content parent!");
            return;
        }

        for (int i = 0; i < list.Count; i++)
        {
            SuspectData data = list[i];
            SuspectItemUI itemInstance = Instantiate(_suspectItemPrefab, _contentSuspectParent);
            itemInstance.Setup(data, OnSuspectSelected);
            _spawnedItems.Add(itemInstance);
        }

        // Chọn nghi phạm đầu tiên mặc định
        if (list.Count > 0)
        {
            OnSuspectSelected(list[0]);
        }
        else
        {
            ClearDetails();
        }
    }

    private void OnSuspectSelected(SuspectData selectedData)
    {
        // Cập nhật thông tin chi tiết bảng bên phải
        if (_txtName != null) _txtName.text = selectedData.NpcName;

        // Cập nhật trạng thái Highlight cho danh sách
        for (int i = 0; i < _spawnedItems.Count; i++)
        {
            if (_spawnedItems[i] != null)
            {
                _spawnedItems[i].SetSelected(_spawnedItems[i].Data == selectedData);
            }
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

    private void ClearDetails()
    {
        if (_txtName != null) _txtName.text = string.Empty;
    }
}