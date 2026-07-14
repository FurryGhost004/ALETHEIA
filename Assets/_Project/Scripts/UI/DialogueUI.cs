using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class DialogueUI : SingletonBase<DialogueUI>
{
    // ── Serialized Fields ─────────────────────────────
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private Image _characterPortrait;
    [SerializeField] private GameObject _continueIcon;

    // ── Private Fields ────────────────────────────────
    private DialogueLine _currentLine;
    private bool _isTyping;
    private Coroutine _typingCoroutine;

    // ── Public Methods ────────────────────────────────

    /// <summary>
    /// Sequence Diagram: startDialogue(npcId) → displayDialogue(text)
    /// NPCController gọi method này sau khi checkCoopStatus() = true
    /// </summary>
    public void DisplayDialogue(DialogueLine line)
    {
        _currentLine = line;
        _nameText.text = line.CharacterName;
        _characterPortrait.sprite = line.CharacterPortrait;
        _dialogueText.text = "";
        _continueIcon.SetActive(false);
        _dialoguePanel.SetActive(true);

        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        _typingCoroutine = StartCoroutine(TypeText(line.Content));
    }

    /// <summary>
    /// Sequence Diagram: displayVagueReply()
    /// NPCController gọi khi LookupResponse() trả về null
    /// </summary>
    public void DisplayVagueReply()
    {
        // TODO: điền nội dung câu trả lời mơ hồ mặc định
        // khi Duy hoàn thành task 3.7 NPCDatabase
        DisplayDialogue(new DialogueLine());
    }

    /// <summary>
    /// Sequence Diagram: openNotebookUI() — đóng DialogueUI
    /// </summary>
    public void CloseDialogue()
    {
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        _dialoguePanel.SetActive(false);
        EventBus.Publish(new DialogueEndedEvent());
    }

    // ── Private Methods ───────────────────────────────

    private void Update()
    {
        if (!_dialoguePanel.activeSelf) return;

        // New Input System — không dùng Input.GetKeyDown()
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            NextLine();
    }

    private void NextLine()
    {
        if (_isTyping)
        {
            // Skip typing — hiện hết chữ ngay
            StopCoroutine(_typingCoroutine);
            _dialogueText.text = _currentLine.Content;
            _isTyping = false;
            _continueIcon.SetActive(true);
            return;
        }

        // Hết dòng → đóng DialogueUI
        CloseDialogue();
    }

    private IEnumerator TypeText(string text)
    {
        _isTyping = true;
        foreach (char c in text)
        {
            _dialogueText.text += c;
            yield return new WaitForSecondsRealtime(0.03f);
        }
        _isTyping = false;
        _continueIcon.SetActive(true);
    }
}