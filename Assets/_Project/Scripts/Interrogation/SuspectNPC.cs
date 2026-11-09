using System;
using System.Collections;
using UnityEngine;

public class SuspectNPC : Interactable
{
    [Header("Suspect Data Asset")]
    [SerializeField] private SuspectData _suspectData;

    [Header("Dialogue Settings")]
    [SerializeField] private bool _isFirstTalk = true;
    [SerializeField] private DialogueDatabase _firstTalkDialogue;
    [SerializeField] private DialogueDatabase _repeatTalkDialogue;

    [Header("Interrogation Settings")]
    [SerializeField] private bool _canBeInterrogated = true;
    [SerializeField] private InterrogationUIController _interrogationUI;

    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSpeed = 5f; // Tốc độ xoay mượt
    [SerializeField] private float _rotationOffset = 0f; // Góc bù (dùng nếu model bị lệch 90 hoặc 180 độ)

    private Coroutine _rotateCoroutine;

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

        // 2. Chọn DialogueDatabase tương ứng
        DialogueDatabase dialogueToPlay = _isFirstTalk ? _firstTalkDialogue : _repeatTalkDialogue;

        // 3. Phát thoại và truyền đầy đủ: Callback, Animator của NPC, và Transform của NPC
        if (dialogueToPlay != null && DialogueManager.Instance != null)
        {
            // Lấy Animator đang gắn trên NPC này (hoặc các đối tượng con bên trong)
            Animator npcAnimator = GetComponentInChildren<Animator>();

            DialogueManager.Instance.StartDialogue(dialogueToPlay, OnDialogueFinished, npcAnimator, transform);
        }
        else
        {
            OnDialogueFinished();
        }
    }


    private void OnDialogueFinished()
    {
        if (_isFirstTalk)
        {
            _isFirstTalk = false;
        }

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
            _interrogationUI = UnityEngine.Object.FindFirstObjectByType<InterrogationUIController>(FindObjectsInactive.Include);
        }

        if (_interrogationUI != null && _suspectData != null)
        {
            _interrogationUI.SetTargetNPC(_suspectData.NpcId, _suspectData.NpcName, _suspectData.Portrait);
        }
    }

    /// <summary>
    /// Xoay NPC từ từ đối mặt với Player, hoàn tất xong mới gọi callback
    /// </summary>
    public void FacePlayerSmooth(Transform player, Action onComplete)
    {
        if (_rotateCoroutine != null)
        {
            StopCoroutine(_rotateCoroutine);
        }
        _rotateCoroutine = StartCoroutine(RotateTowardsPlayerRoutine(player, onComplete));
    }

    private IEnumerator RotateTowardsPlayerRoutine(Transform player, Action onComplete)
    {
        if (player == null)
        {
            onComplete?.Invoke();
            yield break;
        }

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            // Tính toán hướng nhìn cơ bản và cộng thêm góc offset tuỳ chỉnh cho từng NPC
            Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, _rotationOffset, 0);

            // Xoay dần cho tới khi góc lệch nhỏ hơn 1 độ
            while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
                yield return null;
            }

            transform.rotation = targetRotation; // Cố định góc chuẩn xác
        }

        // Xoay xong mới cho phép hội thoại bắt đầu
        onComplete?.Invoke();
    }
}