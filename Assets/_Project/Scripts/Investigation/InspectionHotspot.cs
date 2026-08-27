using System;
using UnityEngine;

public class InspectionHotspot : MonoBehaviour
{
    [TextArea(2, 4)]
    [SerializeField] private string _innerMonologue;
    [SerializeField] private KeywordData _associatedKeyword;

    // Event thông báo khi điểm Hotspot được click
    public event Action OnHotspotClicked;

    public string InnerMonologue => _innerMonologue;
    public KeywordData AssociatedKeyword => _associatedKeyword;

    public void OnDiscovered()
    {
        // Hiển thị thoại suy nghĩ riêng của vết máu/điểm quan trọng
        if (!string.IsNullOrEmpty(_innerMonologue))
        {
            if (ThinkingFeatureUI.Instance != null)
            {
                ThinkingFeatureUI.Instance.ShowThinking(_innerMonologue);
            }
            else
            {
                Debug.Log($"[Hotspot Monologue]: {_innerMonologue}");
            }
        }

        // Mở khóa Keyword nếu Hotspot có chứa KeywordData
        if (_associatedKeyword != null && KeywordManager.Instance != null)
        {
            KeywordManager.Instance.UnlockKeyword(_associatedKeyword);
        }

        // Báo cho EvidenceObject mẹ biết đã click trúng
        OnHotspotClicked?.Invoke();
    }
}