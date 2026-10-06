using UnityEngine;
using UnityEngine.Events;

public class LockedChest : MonoBehaviour
{
    [Header("Lock Settings")]
    [SerializeField] private string _requiredKeyId = "ChestKey_01"; // Cần đúng Key ID này mới mở được
    [SerializeField] private bool _isLocked = true;

    [Header("Dialogues")]
    [SerializeField] private DialogueDatabase _lockedDialogue;   // Thoại khi chưa có chìa khóa ("Nó bị khóa rồi...")
    [SerializeField] private DialogueDatabase _unlockedDialogue; // Thoại khi mở khóa thành công

    [Header("Chest Events")]
    [SerializeField] private UnityEvent _onChestOpened; // Gọi Animation mở nắp, bật item bên trong...

    public bool IsLocked => _isLocked;

    public void Interact()
    {
        // Nếu đã mở rồi thì không xử lý lại
        if (!_isLocked)
        {
            Debug.Log("[Chest] Rương đã được mở trước đó.");
            return;
        }

        // Kiểm tra xem người chơi đã có chìa khóa tương ứng chưa
        if (KeyManager.Instance != null && KeyManager.Instance.HasKey(_requiredKeyId))
        {
            OpenChest();
        }
        else
        {
            // Chưa có chìa khóa -> Báo thoại khóa
            if (_lockedDialogue != null && DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(_lockedDialogue);
            }
            else
            {
                Debug.Log($"[Chest] Rương đang bị khóa! Cần chìa khóa có ID: {_requiredKeyId}");
            }
        }
    }

    private void OpenChest()
    {
        _isLocked = false;

        // Phát thoại mở thành công
        if (_unlockedDialogue != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(_unlockedDialogue);
        }

        // Kích hoạt các hiệu ứng/Animation mở rương
        _onChestOpened?.Invoke();
    }
}