using UnityEngine;
using UnityEngine.UI;
using System;

public class SuspectItemUI : MonoBehaviour
{
    [SerializeField] private Image _avatarImage;
    [SerializeField] private Button _itemButton;
    [SerializeField] private GameObject _selectedHighlight; // (Tùy ch?n) Highlight khi ???c ch?n

    private SuspectData _data;
    private Action<SuspectData> _onSelectedCallback;

    public SuspectData Data => _data;

    public void Setup(SuspectData data, Action<SuspectData> onSelected)
    {
        _data = data;
        _onSelectedCallback = onSelected;

        if (_avatarImage != null && data.Portrait != null)
        {
            _avatarImage.sprite = data.Portrait;
        }

        if (_itemButton != null)
        {
            _itemButton.onClick.RemoveAllListeners();
            _itemButton.onClick.AddListener(() =>
            {
                _onSelectedCallback?.Invoke(_data);
            });
        }
    }

    public void SetSelected(bool isSelected)
    {
        if (_selectedHighlight != null)
        {
            _selectedHighlight.SetActive(isSelected);
        }
    }
}