using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InterrogationUIController : SingletonBase<InterrogationUIController>
{
    [Header("Current Target NPC Data")]
    private string _currentNpcId;
    private string _currentNpcName;
    private Sprite _currentNpcPortrait;

    [Header("UI - Input Group")]
    [SerializeField] private GameObject _inputGroupPanel; // Panel chứa Dropdown, InputField và nút Ask
    [SerializeField] private TMP_Dropdown _dropdownWH;
    [SerializeField] private TMP_InputField _inputKeyword;
    [SerializeField] private Button _btnAsk;
    [SerializeField] private Button _btnClose;

    [Header("UI - Response Display Inside Interrogation Panel")]
    [SerializeField] private TextMeshProUGUI _npcNameText;
    [SerializeField] private TextMeshProUGUI _responseText;
    [SerializeField] private Image _npcPortraitImage;
    [SerializeField] private float _typewriterSpeed = 0.03f;

    [Header("Fallback Settings")]
    [SerializeField] private DialogueDatabase _defaultVagueDialogue;

    [Header("Player References")]
    [SerializeField] private PlayerInputHandler _playerInputHandler;

    // Trạng thái hiển thị lời thoại thẩm vấn
    private DialogueDatabase _currentResponseDialogue;
    private int _currentLineIndex;
    private bool _isShowingResponse;
    private bool _isTyping;
    private Coroutine _typeCoroutine;

    private void Awake()
    {
        if (_btnAsk != null) _btnAsk.onClick.AddListener(SubmitQuestion);
        if (_btnClose != null) _btnClose.onClick.AddListener(CloseInterrogation);
        if (_inputKeyword != null) _inputKeyword.onSubmit.AddListener(OnInputSubmit);
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        // 1. Phím ESC để đóng giao diện
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseInterrogation();
            return;
        }

        // 2. Nếu đang hiển thị lời thoại trả lời -> Bấm Space hoặc Click chuột để chuyển dòng / hoàn thành chữ
        if (_isShowingResponse)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
            {
                AdvanceResponseDialogue();
            }
            return;
        }

        // 3. Bắt phím Enter khi đang gõ Keyword trong ô InputField
        if (_inputKeyword != null && _inputKeyword.isFocused &&
            (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
        {
            SubmitQuestion();
        }
    }

    private void OnDestroy()
    {
        if (_btnAsk != null) _btnAsk.onClick.RemoveListener(SubmitQuestion);
        if (_btnClose != null) _btnClose.onClick.RemoveListener(CloseInterrogation);
        if (_inputKeyword != null) _inputKeyword.onSubmit.RemoveListener(OnInputSubmit);
    }

    public void SetTargetNPC(string npcId, string npcName, Sprite npcPortrait)
    {
        _currentNpcId = npcId;
        _currentNpcName = npcName;
        _currentNpcPortrait = npcPortrait;

        if (_playerInputHandler == null)
        {
            _playerInputHandler = Object.FindFirstObjectByType<PlayerInputHandler>();
        }

        gameObject.SetActive(true);

        // Cập nhật thông tin NPC lên Panel Thẩm vấn
        if (_npcNameText != null) _npcNameText.text = _currentNpcName;
        if (_npcPortraitImage != null)
        {
            _npcPortraitImage.sprite = _currentNpcPortrait;
            _npcPortraitImage.gameObject.SetActive(_currentNpcPortrait != null);
        }
        if (_responseText != null) _responseText.text = string.Empty;

        _isShowingResponse = false;
        SetInputGroupActive(true);

        if (_playerInputHandler != null)
        {
            _playerInputHandler.SetInterrogating(true);
        }

        if (_inputKeyword != null)
        {
            _inputKeyword.text = string.Empty;
            _inputKeyword.ActivateInputField();
        }
    }

    public void CloseInterrogation()
    {
        if (_playerInputHandler == null)
        {
            _playerInputHandler = Object.FindFirstObjectByType<PlayerInputHandler>();
        }

        if (_playerInputHandler != null)
        {
            _playerInputHandler.SetInterrogating(false);
        }

        if (_typeCoroutine != null) StopCoroutine(_typeCoroutine);
        _isShowingResponse = false;

        gameObject.SetActive(false);
    }

    private void OnInputSubmit(string text)
    {
        SubmitQuestion();
    }

    public void SubmitQuestion()
    {
        if (_dropdownWH == null || _inputKeyword == null) return;

        string enteredKeywordStr = _inputKeyword.text.Trim();
        if (string.IsNullOrEmpty(enteredKeywordStr)) return;

        WHType selectedWH = (WHType)_dropdownWH.value;

        KeywordData targetKeyword = null;
        if (KeywordManager.Instance != null)
        {
            targetKeyword = KeywordManager.Instance.GetKeyword(enteredKeywordStr);
        }

        InterrogationResponseEntry responseEntry = null;
        if (InterrogationManager.Instance != null)
        {
            responseEntry = InterrogationManager.Instance.LookupResponse(_currentNpcId, selectedWH, targetKeyword);
        }

        DialogueDatabase dialogueToPlay = (responseEntry != null && responseEntry.ResponseDialogue != null)
            ? responseEntry.ResponseDialogue
            : _defaultVagueDialogue;

        if (dialogueToPlay != null && dialogueToPlay.Lines.Count > 0)
        {
            StartResponseDialogue(dialogueToPlay);
        }
    }

    private void StartResponseDialogue(DialogueDatabase responseDialogue)
    {
        _currentResponseDialogue = responseDialogue;
        _currentLineIndex = 0;
        _isShowingResponse = true;

        // Ẩn thanh nhập liệu (Dropdown/InputField)
        SetInputGroupActive(false);

        // Hiển thị câu trả lời dòng đầu tiên
        ShowResponseLine(_currentResponseDialogue.Lines[_currentLineIndex]);
    }

    private void AdvanceResponseDialogue()
    {
        // Nếu đang trong quá trình gõ chữ -> Hoàn thành dòng chữ ngay lập tức
        if (_isTyping)
        {
            if (_typeCoroutine != null) StopCoroutine(_typeCoroutine);
            if (_responseText != null) _responseText.text = _currentResponseDialogue.Lines[_currentLineIndex].Content;
            _isTyping = false;
            return;
        }

        // Chuyển sang dòng kế tiếp
        _currentLineIndex++;
        if (_currentLineIndex < _currentResponseDialogue.Lines.Count)
        {
            ShowResponseLine(_currentResponseDialogue.Lines[_currentLineIndex]);
        }
        else
        {
            // Kết thúc thoại -> Bật lại thanh nhập liệu để tiếp tục hỏi
            EndResponseDialogue();
        }
    }

    private void ShowResponseLine(DialogueLine line)
    {
        if (_npcNameText != null)
        {
            _npcNameText.text = !string.IsNullOrEmpty(line.CharacterName) ? line.CharacterName : _currentNpcName;
        }

        if (_npcPortraitImage != null)
        {
            Sprite portrait = line.CharacterPortrait != null ? line.CharacterPortrait : _currentNpcPortrait;
            _npcPortraitImage.sprite = portrait;
            _npcPortraitImage.gameObject.SetActive(portrait != null);
        }

        if (_typeCoroutine != null) StopCoroutine(_typeCoroutine);
        _typeCoroutine = StartCoroutine(TypewriterRoutine(line.Content));
    }

    private IEnumerator TypewriterRoutine(string text)
    {
        _isTyping = true;

        if (_responseText != null)
        {
            _responseText.text = string.Empty;
            foreach (char c in text)
            {
                _responseText.text += c;
                yield return new WaitForSeconds(_typewriterSpeed);
            }
        }

        _isTyping = false;
    }

    private void EndResponseDialogue()
    {
        _isShowingResponse = false;

        // Bật lại thanh nhập liệu
        SetInputGroupActive(true);

        if (_inputKeyword != null)
        {
            _inputKeyword.text = string.Empty;
            _inputKeyword.ActivateInputField();
        }
    }

    private void SetInputGroupActive(bool isActive)
    {
        if (_inputGroupPanel != null)
        {
            _inputGroupPanel.SetActive(isActive);
        }
        else
        {
            if (_dropdownWH != null) _dropdownWH.gameObject.SetActive(isActive);
            if (_inputKeyword != null) _inputKeyword.gameObject.SetActive(isActive);
            if (_btnAsk != null) _btnAsk.gameObject.SetActive(isActive);
        }
    }
}