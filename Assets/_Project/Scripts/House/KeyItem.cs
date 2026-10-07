using UnityEngine;

// Đổi MonoBehaviour thành Interactable
public class KeyItem : Interactable
{
    [Header("Key Info")]
    [SerializeField] private string _keyId = "ChestKey_01";
    [SerializeField] private KeywordData _associatedKeyword;

    [Header("Dialogue Feedback")]
    [SerializeField] private DialogueDatabase _pickUpDialogue;

    public string KeyId => _keyId;

    // Thay đổi thành override nếu lớp cha Interactable có hàm virtual Interact()
    public override void Interact()
    {
        PickUp();
    }

    public void PickUp()
    {
        Debug.Log($"<color=green>[KEYITEM SUCCESS]</color> Đã tương tác và nhặt: {_keyId}");

        if (KeyManager.Instance != null)
        {
            KeyManager.Instance.AddKey(_keyId);
        }

        if (_associatedKeyword != null && KeywordManager.Instance != null)
        {
            KeywordManager.Instance.UnlockKeyword(_associatedKeyword);
        }

        if (_pickUpDialogue != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(_pickUpDialogue);
        }

        gameObject.SetActive(false);
    }
}