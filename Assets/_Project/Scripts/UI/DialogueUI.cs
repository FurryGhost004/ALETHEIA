using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [Header("UI Panels & Text")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private Image _characterPortrait;
    [SerializeField] private GameObject _continueIcon;

    private DialogueLine _currentLine;
    private bool _isTyping;
    private Coroutine _typingCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[DialogueUI] Bị trùng Singleton! Script trên '{gameObject.name}' đang bị Destroy vì đã có Instance trên '{Instance.gameObject.name}'");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void DisplayDialogue(DialogueLine line, string overrideName = null, Sprite overridePortrait = null)
    {
        if (_dialoguePanel != null) _dialoguePanel.SetActive(true);

        // Ưu tiên dùng overrideName từ SuspectNPC, nếu trống mới dùng dữ liệu trong DialogueLine
        if (_nameText != null)
        {
            _nameText.text = !string.IsNullOrEmpty(overrideName) ? overrideName : line.CharacterName;
        }

        // Ưu tiên dùng overridePortrait từ SuspectNPC
        if (_characterPortrait != null)
        {
            Sprite targetPortrait = (overridePortrait != null) ? overridePortrait : line.CharacterPortrait;
            if (targetPortrait != null)
            {
                _characterPortrait.sprite = targetPortrait;
                _characterPortrait.gameObject.SetActive(true);
            }
        }

        if (_dialogueText != null)
        {
            _dialogueText.text = line.Content;
        }
    }

    public void DisplayVagueReply(string npcName = "Nghi phạm", Sprite npcPortrait = null)
    {
        if (_dialoguePanel != null) _dialoguePanel.SetActive(true);

        if (_nameText != null)
            _nameText.text = npcName; // Hiển thị tên thực tế của NPC

        if (_characterPortrait != null && npcPortrait != null)
            _characterPortrait.sprite = npcPortrait;

        if (_dialogueText != null)
            _dialogueText.text = "Tôi không biết hoặc không muốn trả lời về điều đó...";
    }

    public void CloseDialogue()
    {
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        if (_dialoguePanel != null)
            _dialoguePanel.SetActive(false);
    }

    private IEnumerator TypeText(string text)
    {
        _isTyping = true;

        if (_dialogueText != null)
            _dialogueText.text = string.Empty;

        if (_continueIcon != null)
            _continueIcon.SetActive(false);

        if (string.IsNullOrEmpty(text))
        {
            text = "...";
        }

        foreach (char c in text)
        {
            if (_dialogueText != null)
                _dialogueText.text += c;

            yield return new WaitForSecondsRealtime(0.03f);
        }

        _isTyping = false;

        if (_continueIcon != null)
            _continueIcon.SetActive(true);
    }
}