using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SuspectListUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform _contentSuspectParent; // Content_Suspect trong ScrollView
    [SerializeField] private SuspectItemUI _suspectItemPrefab; // PF_SuspectList Prefab

    [Header("Detail Panel References (Bảng bên phải)")]
    [SerializeField] private TextMeshProUGUI _txtName;
    [SerializeField] private TextMeshProUGUI _txtAddress;
    [SerializeField] private TextMeshProUGUI _txtHeight;
    [SerializeField] private TextMeshProUGUI _txtDescription;
    [SerializeField] private Image _imgPortrait;

    [Header("Conviction Settings")]
    [SerializeField] private Button _btnConvict; // Kéo Btn_Convict vào đây

    private readonly List<SuspectItemUI> _spawnedItems = new List<SuspectItemUI>();
    private SuspectData _selectedSuspect; // Quản lý nghi phạm đang được chọn

    private void Awake()
    {
        if (_btnConvict != null)
        {
            _btnConvict.onClick.AddListener(OnConvictClicked);
            _btnConvict.interactable = false; // Mặc định khóa nút khi chưa chọn nghi phạm
        }
    }

    private void OnEnable()
    {
        // Tự động làm mới danh sách khi bật Panel UI nghi phạm
        RefreshList();
    }

    private void OnDestroy()
    {
        if (_btnConvict != null)
        {
            _btnConvict.onClick.RemoveListener(OnConvictClicked);
        }
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

        // Chọn nghi phạm đầu tiên mặc định nếu danh sách không rỗng
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
        _selectedSuspect = selectedData;

        if (_selectedSuspect == null)
        {
            ClearDetails();
            return;
        }

        // 1. Cập nhật thông tin chi tiết bảng bên phải
        if (_txtName != null) _txtName.text = _selectedSuspect.NpcName;
        if (_txtAddress != null) _txtAddress.text = $"ADDRESS: {_selectedSuspect.Address}";
        if (_txtHeight != null) _txtHeight.text = $"HEIGHT: {_selectedSuspect.Height}";
        if (_txtDescription != null) _txtDescription.text = $"DESCRIPTION:\n{_selectedSuspect.Description}";

        if (_imgPortrait != null)
        {
            _imgPortrait.sprite = _selectedSuspect.Portrait;
            _imgPortrait.gameObject.SetActive(_selectedSuspect.Portrait != null);
        }

        // 2. Kích hoạt nút buộc tội Btn_Convict
        if (_btnConvict != null)
        {
            _btnConvict.interactable = true;
        }

        // 3. Cập nhật trạng thái Highlight cho danh sách bên trái
        for (int i = 0; i < _spawnedItems.Count; i++)
        {
            if (_spawnedItems[i] != null)
            {
                _spawnedItems[i].SetSelected(_spawnedItems[i].Data == selectedData);
            }
        }
    }

    private void OnConvictClicked()
    {
        if (_selectedSuspect == null) return;

        // 1. Lưu nghi phạm bị buộc tội vào bộ nhớ tĩnh để dùng ở Scene CourtTrial
        CourtTrialController.ConvictedSuspect = _selectedSuspect;

        // 2. Đóng Notebook
        NotebookHubUI notebookHub = Object.FindFirstObjectByType<NotebookHubUI>();
        if (notebookHub != null)
        {
            notebookHub.CloseHub();
        }

        // 3. Chuyển sang Scene phiên tòa
        SceneManager.LoadScene("CourtTrial");
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
        _selectedSuspect = null;

        if (_txtName != null) _txtName.text = string.Empty;
        if (_txtAddress != null) _txtAddress.text = string.Empty;
        if (_txtHeight != null) _txtHeight.text = string.Empty;
        if (_txtDescription != null) _txtDescription.text = string.Empty;
        if (_imgPortrait != null) _imgPortrait.gameObject.SetActive(false);

        if (_btnConvict != null) _btnConvict.interactable = false;
    }
}