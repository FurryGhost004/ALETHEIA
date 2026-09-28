using System;
using UnityEngine;

public class InspectionHotspot : MonoBehaviour
{
    [Header("Hotspot Dialogue & Keyword")]
    [SerializeField] private DialogueDatabase _hotspotDialogue;
    [SerializeField] private KeywordData _associatedKeyword;

    // Event thông báo khi điểm Hotspot được click
    public event Action OnHotspotClicked;

    public DialogueDatabase HotspotDialogue => _hotspotDialogue;
    public KeywordData AssociatedKeyword => _associatedKeyword;

    public void OnDiscovered()
    {
        // Hiển thị hội thoại / suy nghĩ từ DialogueDatabase
        if (_hotspotDialogue != null)
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(_hotspotDialogue);
            }
            else if (ThinkingFeatureUI.Instance != null)
            {
                // Nếu ThinkingFeatureUI đã cập nhật nhận DialogueDatabase
                // ThinkingFeatureUI.Instance.ShowThinking(_hotspotDialogue);
            }
            else
            {
                Debug.Log($"[Hotspot Dialogue]: Kích hoạt DialogueDatabase '{_hotspotDialogue.name}'");
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