using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [Header("Key Info")]
    [SerializeField] private string _keyId = "ChestKey_01"; // ID duy nhất của chìa khóa này
    [SerializeField] private KeywordData _associatedKeyword; // Tùy chọn: Mở khóa Keyword nếu có

    [Header("Dialogue Feedback")]
    [SerializeField] private DialogueDatabase _pickUpDialogue; // Thoại phát ra khi nhặt chìa khóa

    public string KeyId => _keyId;

    public void PickUp()
    {
        // 1. Lưu chìa khóa vào KeyManager
        if (KeyManager.Instance != null)
        {
            KeyManager.Instance.AddKey(_keyId);
        }

        // 2. Mở khóa Keyword trong sổ tay (nếu có gán KeywordData)
        if (_associatedKeyword != null && KeywordManager.Instance != null)
        {
            KeywordManager.Instance.UnlockKeyword(_associatedKeyword);
        }

        // 3. Phát thoại khi nhặt (nếu có)
        if (_pickUpDialogue != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(_pickUpDialogue);
        }

        // 4. Ẩn chìa khóa khỏi Scene
        gameObject.SetActive(false);
    }
}