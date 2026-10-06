using UnityEngine;
using UnityEngine.Events;

public class LockedChest : Interactable
{
    [Header("Lock Settings")]
    [SerializeField] private string _requiredKeyId = "ChestKey_01";
    [SerializeField] private bool _isLocked = true;

    [Header("Dialogues")]
    [SerializeField] private DialogueDatabase _lockedDialogue;
    [SerializeField] private DialogueDatabase _unlockedDialogue;

    [Header("Chest Events")]
    [SerializeField] private UnityEvent OnChestOpened;

    public override void Interact()
    {
        // 1. Nếu rương đang bị khóa
        if (_isLocked)
        {
            // Kiểm tra chìa khóa trong KeyManager
            if (KeyManager.Instance != null && KeyManager.Instance.HasKey(_requiredKeyId))
            {
                Debug.Log($"<color=green>[CHEST UNLOCKED]</color> Đã dùng chìa {_requiredKeyId} để mở rương!");
                _isLocked = false;

                // Kích hoạt Event mở cánh cửa
                OnChestOpened?.Invoke();

                // Phát thoại khi mở khóa thành công
                if (_unlockedDialogue != null && DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.StartDialogue(_unlockedDialogue);
                }
            }
            else
            {
                Debug.LogWarning($"<color=yellow>[CHEST LOCKED]</color> Rương bị khóa! Chưa có chìa khóa: {_requiredKeyId}");

                // Phát thoại báo thiếu chìa khóa
                if (_lockedDialogue != null && DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.StartDialogue(_lockedDialogue);
                }
            }
        }
        else
        {
            // 2. Nếu rương đã được mở khóa từ trước
            Debug.Log("<color=cyan>[CHEST INTERACT]</color> Rương đã mở, kích hoạt xoay cửa.");
            OnChestOpened?.Invoke();
        }
    }
}