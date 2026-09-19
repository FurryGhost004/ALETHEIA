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

    public bool IsInDialogue { get; private set; }

    private void Awake() { }

    private void Start() { }

    private void Update()
    {
        if (!IsInDialogue) return;

        if (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame))
        {
            AdvanceDialogue();
        }
    }

    public void StartDialogue(DialogueDatabase dialogue, Action onComplete = null)
    {
        if (dialogue == null || dialogue.Lines.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        _currentDialogue = dialogue;
        _currentLineIndex = 0;
        _onCompleteCallback = onComplete;
        IsInDialogue = true;

        if (_dialoguePanel != null) _dialoguePanel.SetActive(true);

        ShowLine(_currentDialogue.Lines[_currentLineIndex]);
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

        Action callback = _onCompleteCallback;
        _onCompleteCallback = null;
        callback?.Invoke();

        EventBus.Publish(new DialogueEndedEvent());
    }
}