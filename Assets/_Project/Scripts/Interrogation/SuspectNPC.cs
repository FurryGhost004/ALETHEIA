using UnityEngine;

public class SuspectNPC : Interactable
{
    [Header("Suspect Data Asset")]
    [SerializeField] private SuspectData _suspectData; // Gán ScriptableObject SuspectData vào đây

    [Header("Dialogue Settings")]
    [SerializeField] private bool _isFirstTalk = true;
    [SerializeField] private DialogueDatabase _firstTalkDialogue;
    [SerializeField] private DialogueDatabase _repeatTalkDialogue;

    [Header("Interrogation Settings")]
    [SerializeField] private bool _canBeInterrogated = true;
    [SerializeField] private InterrogationUIController _interrogationUI;

    // Getter lấy dữ liệu trực tiếp từ SuspectData Asset
    public string NpcId => _suspectData != null ? _suspectData.NpcId : string.Empty;
    public string NpcName => _suspectData != null ? _suspectData.NpcName : string.Empty;
    public Sprite NpcPortrait => _suspectData != null ? _suspectData.Portrait : null;

    public override void Interact()
    {
        // 1. Lưu thông tin nghi phạm vào SuspectManager
        if (SuspectManager.Instance != null && _suspectData != null)
        {
            SuspectManager.Instance.AddSuspect(_suspectData);
        }

        // 2. Chọn DialogueDatabase tương ứng dựa theo _isFirstTalk
        DialogueDatabase dialogueToPlay = _isFirstTalk ? _firstTalkDialogue : _repeatTalkDialogue;

        // 3. Phát thoại trước, truyền OnDialogueFinished làm callback sau khi đọc xong
        if (dialogueToPlay != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueToPlay, OnDialogueFinished);
        }
        else
        {
            OnDialogueFinished();
        }
    }

    private void OnDialogueFinished()
    {
        // Đánh dấu đã qua lần nói chuyện đầu tiên
        if (_isFirstTalk)
        {
            _isFirstTalk = false;
        }

        // Tăng thời gian game
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.AdvanceTime();
        }

        // Bật thẩm vấn sau khi hoàn tất hội thoại
        if (_canBeInterrogated)
        {
            StartInterrogation();
        }
    }

    public void StartInterrogation()
    {
        EventBus.Publish(new StartInterrogationEvent(this));

        if (_interrogationUI == null)
        {
            _interrogationUI = Object.FindFirstObjectByType<InterrogationUIController>(FindObjectsInactive.Include);
        }

        if (_interrogationUI != null && _suspectData != null)
        {
            _interrogationUI.SetTargetNPC(_suspectData.NpcId, _suspectData.NpcName, _suspectData.Portrait);
        }
    }
}