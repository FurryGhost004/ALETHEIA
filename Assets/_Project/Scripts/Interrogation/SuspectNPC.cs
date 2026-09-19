using UnityEngine;

public class SuspectNPC : Interactable
{
    [Header("Suspect Data")]
    [SerializeField] private string _npcId = "Suspect_01";
    [SerializeField] private string _npcName = "Nghi phạm 01";
    [SerializeField] private Sprite _npcPortrait;

    [Header("Dialogue Settings")]
    [SerializeField] private bool _isFirstTalk = true;
    [SerializeField] private DialogueDatabase _firstTalkDialogue;
    [SerializeField] private DialogueDatabase _repeatTalkDialogue;

    [Header("Interrogation Settings")]
    [SerializeField] private bool _canBeInterrogated = true;
    [SerializeField] private InterrogationUIController _interrogationUI;

    public string NpcId => _npcId;
    public string NpcName => _npcName;
    public Sprite NpcPortrait => _npcPortrait;

    private void Awake() { }

    private void Start() { }

    public override void Interact()
    {
        // 1. Lưu thông tin nghi phạm
        if (SuspectManager.Instance != null)
        {
            SuspectManager.Instance.AddSuspect(_npcId, _npcName, _npcPortrait);
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

        // Tăng thời gian game (nếu có)
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

        if (_interrogationUI != null)
        {
            // Truyền 3 tham số phù hợp với InterrogationUIController
            _interrogationUI.SetTargetNPC(_npcId, _npcName, _npcPortrait);
        }
    }
}