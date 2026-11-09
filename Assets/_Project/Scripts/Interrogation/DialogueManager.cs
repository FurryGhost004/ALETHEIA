using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : SingletonBase<DialogueManager>
{
    [Header("UI References")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _contentText;
    [SerializeField] private Image _portraitImage;

    [Header("Settings")]
    [SerializeField] private float _typewriterSpeed = GameConstants.DEFAULT_TYPEWRITER_SPEED;

    private DialogueDatabase _currentDialogue;
    private int _currentLineIndex;
    private bool _isTyping;
    private Coroutine _typeCoroutine;
    private Action _onCompleteCallback;

    // Lưu tham chiếu Animator của NPC đang nói hiện tại
    private Animator _currentSpeakerAnimator;

    // Tham chiếu tới PlayerMovement và PlayerLook để khóa/mở điều khiển
    private PlayerMovement _playerMovement;
    private PlayerLook _playerLook;

    public bool IsInDialogue { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        // Tự động tìm PlayerMovement và PlayerLook trong Scene khi khởi động
        _playerMovement = UnityEngine.Object.FindFirstObjectByType<PlayerMovement>();
        _playerLook = UnityEngine.Object.FindFirstObjectByType<PlayerLook>();
    }

    private void Start() { }

    private void Update()
    {
        if (!IsInDialogue) return;

        if (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame))
        {
            AdvanceDialogue();
        }
    }

    /// <summary>
    /// Bắt đầu hội thoại, có thể truyền thêm Animator của nhân vật nói
    /// </summary>
    public void StartDialogue(DialogueDatabase dialogue, Action onComplete = null, Animator speakerAnimator = null, Transform speakerTransform = null)
    {
        if (dialogue == null || dialogue.Lines.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        _currentDialogue = dialogue;
        _currentLineIndex = 0;
        _onCompleteCallback = onComplete;
        _currentSpeakerAnimator = speakerAnimator;

        // 1. Khóa di chuyển và khóa góc nhìn của Player ngay lập tức khi bấm tương tác
        if (_playerMovement == null) _playerMovement = UnityEngine.Object.FindFirstObjectByType<PlayerMovement>();
        if (_playerLook == null) _playerLook = UnityEngine.Object.FindFirstObjectByType<PlayerLook>();

        if (_playerMovement != null) _playerMovement.SetCanMove(false);
        if (_playerLook != null) _playerLook.SetCanLook(false);

        // 2. Hàm nội bộ để bắt đầu bật thoại sau khi NPC đã quay xong
        Action startLineAction = () =>
        {
            IsInDialogue = true;
            if (_dialoguePanel != null) _dialoguePanel.SetActive(true);
            ShowLine(_currentDialogue.Lines[_currentLineIndex]);
        };

        // 3. Nếu có NPC, xoay từ từ trước rồi mới gọi startLineAction
        if (speakerTransform != null)
        {
            SuspectNPC suspectNPC = speakerTransform.GetComponent<SuspectNPC>();
            if (suspectNPC != null && _playerMovement != null)
            {
                suspectNPC.FacePlayerSmooth(_playerMovement.transform, startLineAction);
            }
            else
            {
                startLineAction();
            }
        }
        else
        {
            startLineAction();
        }
    }

    private void AdvanceDialogue()
    {
        if (_isTyping)
        {
            if (_typeCoroutine != null) StopCoroutine(_typeCoroutine);
            _contentText.text = _currentDialogue.Lines[_currentLineIndex].Content;
            _isTyping = false;
            return;
        }

        _currentLineIndex++;
        if (_currentLineIndex < _currentDialogue.Lines.Count)
        {
            ShowLine(_currentDialogue.Lines[_currentLineIndex]);
        }
        else
        {
            EndDialogue();
        }
    }

    private void ShowLine(DialogueLine line)
    {
        if (_nameText != null) _nameText.text = line.CharacterName;

        if (_portraitImage != null)
        {
            if (line.CharacterPortrait != null)
            {
                _portraitImage.sprite = line.CharacterPortrait;
                _portraitImage.gameObject.SetActive(true);
            }
            else
            {
                _portraitImage.gameObject.SetActive(false);
            }
        }

        // --- KÍCH HOẠT TẤT CẢ CÁC ANIMATION TRIGGERS TRONG CÂU THOẠI ---
        if (_currentSpeakerAnimator != null && line.AnimationTriggers != null)
        {
            foreach (string trigger in line.AnimationTriggers)
            {
                if (!string.IsNullOrEmpty(trigger))
                {
                    _currentSpeakerAnimator.SetTrigger(trigger);
                }
            }
        }

        if (_typeCoroutine != null) StopCoroutine(_typeCoroutine);
        _typeCoroutine = StartCoroutine(TypewriterRoutine(line.Content));
    }

    private IEnumerator TypewriterRoutine(string text)
    {
        _isTyping = true;
        _contentText.text = string.Empty;

        foreach (char c in text)
        {
            _contentText.text += c;
            yield return new WaitForSeconds(_typewriterSpeed);
        }

        _isTyping = false;
    }

    private void EndDialogue()
    {
        IsInDialogue = false;
        if (_dialoguePanel != null) _dialoguePanel.SetActive(false);

        _currentSpeakerAnimator = null; // Xóa tham chiếu animator khi kết thúc hội thoại

        // --- MỞ LẠI QUYỀN DI CHUYỂN VÀ XOAY CAMERA CHO PLAYER KHI KẾT THÚC ---
        if (_playerMovement != null)
        {
            _playerMovement.SetCanMove(true);
        }

        if (_playerLook != null)
        {
            _playerLook.SetCanLook(true);
        }

        Action callback = _onCompleteCallback;
        _onCompleteCallback = null;
        callback?.Invoke();

        EventBus.Publish(new DialogueEndedEvent());
    }
}