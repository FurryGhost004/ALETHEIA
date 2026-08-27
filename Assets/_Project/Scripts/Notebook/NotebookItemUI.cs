using System;
using UnityEngine;
using UnityEngine.UI;

public class NotebookItemUI : MonoBehaviour
{
    [SerializeField] private Image _imgIcon;
    [SerializeField] private Image _imgSelectedBorder;
    [SerializeField] private Button _itemButton;

    private Action<KeywordData> _onSelectedCallback;

    public KeywordData Data { get; private set; }

    private void Awake()
    {
        if (_itemButton != null)
        {
            _itemButton.onClick.AddListener(OnItemClicked);
        }
    }

    public void Setup(KeywordData data, Action<KeywordData> onSelectedCallback)
    {
        Data = data;
        _onSelectedCallback = onSelectedCallback;

        if (_imgIcon != null && Data != null)
        {
            _imgIcon.sprite = Data.Icon;
            _imgIcon.enabled = Data.Icon != null;
        }

        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        if (_imgSelectedBorder != null)
        {
            _imgSelectedBorder.gameObject.SetActive(isSelected);
        }
    }

    private void OnItemClicked()
    {
        _onSelectedCallback?.Invoke(Data);
    }
}