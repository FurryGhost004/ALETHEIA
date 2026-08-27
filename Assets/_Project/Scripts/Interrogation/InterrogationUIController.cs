using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterrogationUIController : MonoBehaviour
{
    public static InterrogationUIController Instance { get; private set; }

    [Header("Current Target NPC Data")]
    private string _currentNpcId;
    private string _currentNpcName;
    private Sprite _currentNpcPortrait;
    private DialogueDatabase _dialogueDatabase;

    [Header("UI References")]
    [SerializeField] private TMP_Dropdown _dropdownWH;
    [SerializeField] private TMP_InputField _inputKeyword;
    [SerializeField] private Button _btnAsk;
    [SerializeField] private Button _btnClose;

    [Header("Player References")]
    [SerializeField] private PlayerInputHandler _playerInputHandler;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Đăng ký sự kiện nút bấm
        if (_btnAsk != null)
        {
            _btnAsk.onClick.AddListener(SubmitQuestion);
        }

        if (_btnClose != null)
        {
            _btnClose.onClick.AddListener(CloseInterrogation);
        }

        if (_inputKeyword != null)
        {
            _inputKeyword.onSubmit.AddListener(OnInputSubmit);
        }
    }

    private void Update()
    {
        // 1. Phím ESC để đóng giao diện
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInterrogation();
            return;
        }

        // 2. Bắt phím Enter trực tiếp khi đang tập trung gõ trong InputField
        if (_inputKeyword != null && _inputKeyword.isFocused && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            SubmitQuestion();
        }
    }

    private void OnDestroy()
    {
        if (_inputKeyword != null)
        {
            _inputKeyword.onSubmit.RemoveListener(OnInputSubmit);
        }
    }

    /// <summary>
    /// Kích hoạt bảng thẩm vấn và nhận đầy đủ thông tin từ NPC được tương tác.
    /// </summary>
    public void SetTargetNPC(string npcId, string npcName, Sprite npcPortrait, DialogueDatabase npcDatabase)
    {
        _currentNpcId = npcId;
        _currentNpcName = npcName;
        _currentNpcPortrait = npcPortrait;
        _dialogueDatabase = npcDatabase;

        // Tự động tìm PlayerInputHandler nếu chưa gán ở Inspector
        if (_playerInputHandler == null)
        {
            _playerInputHandler = FindFirstObjectByType<PlayerInputHandler>();
        }

        // Bật UI và Khóa di chuyển của Player
        gameObject.SetActive(true);
        if (_playerInputHandler != null)
        {
            _playerInputHandler.SetInterrogating(true);
        }

        // Focus sẵn vào ô nhập keyword để gõ ngay không cần click chuột
        if (_inputKeyword != null)
        {
            _inputKeyword.ActivateInputField();
        }

        Debug.Log($"[Interrogate] Bắt đầu thẩm vấn NPC: '{_currentNpcName}' (ID: {_currentNpcId})");
    }

    /// <summary>
    /// Đóng bảng thẩm vấn, khôi phục di chuyển và con trỏ chuột cho Player.
    /// </summary>
    public void CloseInterrogation()
    {
        if (_playerInputHandler == null)
        {
            _playerInputHandler = FindFirstObjectByType<PlayerInputHandler>();
        }

        // Mở lại di chuyển và ẩn con trỏ chuột
        if (_playerInputHandler != null)
        {
            _playerInputHandler.SetInterrogating(false);
        }

        // Tắt bảng thoại đang hiển thị
        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.CloseDialogue();
        }

        gameObject.SetActive(false);
        Debug.Log("[Interrogate] Đã thoát thẩm vấn.");
    }

    private void OnInputSubmit(string text)
    {
        SubmitQuestion();
    }

    /// <summary>
    /// Tra cứu câu trả lời dựa trên thông tin NPC, từ hỏi WH và Từ khóa nhập vào.
    /// </summary>
    public void SubmitQuestion()
    {
        if (_dialogueDatabase == null)
        {
            Debug.LogError("[Interrogate] LỖI: Chưa có DialogueDatabase cho NPC này!");
            return;
        }

        if (_dropdownWH == null || _inputKeyword == null)
        {
            Debug.LogError("[Interrogate] LỖI: Thiếu DropdownWH hoặc InputKeyword trong Inspector!");
            return;
        }

        string enteredKeyword = _inputKeyword.text.Trim();
        string selectedWH = _dropdownWH.options[_dropdownWH.value].text.Trim();

        if (string.IsNullOrEmpty(enteredKeyword))
        {
            Debug.LogWarning("[Interrogate] Ô nhập keyword bị trống!");
            return;
        }

        DialogueLine response = _dialogueDatabase.LookupResponse(_currentNpcId, selectedWH, enteredKeyword);

        if (response != null)
        {
            Debug.Log($"[Interrogate] -> TÌM THẤY THOẠI: {response.Content}");

            if (DialogueUI.Instance != null)
            {
                // Ép buộc hiển thị Tên và Ảnh của SuspectNPC
                DialogueUI.Instance.DisplayDialogue(response, _currentNpcName, _currentNpcPortrait);
            }
        }
        else
        {
            Debug.LogWarning("[Interrogate] -> KHÔNG tìm thấy thoại, hiển thị câu trả lời mặc định.");

            if (DialogueUI.Instance != null)
            {
                DialogueUI.Instance.DisplayVagueReply(_currentNpcName, _currentNpcPortrait);
            }
        }

        _inputKeyword.text = string.Empty;
        _inputKeyword.ActivateInputField();
    }
}