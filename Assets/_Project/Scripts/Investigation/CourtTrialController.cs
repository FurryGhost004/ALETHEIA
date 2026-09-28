using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CourtTrialController : MonoBehaviour
{
    public enum CameraView { Judge, Player, Suspect }

    private enum TrialState
    {
        IntroDialogue,
        TopicSelection,
        EvidenceSelection,
        ResultDialogueSuccess,
        ResultDialogueFail
    }

    // --- PROPERTY ĐƯỢC GỌI TỪ TOPICUICONTROLLER / KHỦNG TRUYỀN DỮ LIỆU ---
    public static SuspectData ConvictedSuspect { get; set; }

    [Header("Spawn Points")]
    [SerializeField] private Transform _suspectSpawnPoint;

    [Header("Court Cameras")]
    [SerializeField] private Camera _camJudge;
    [SerializeField] private Camera _camPlayer;
    [SerializeField] private Camera _camSuspect;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _txtSuspectName;
    [SerializeField] private Image _imgSuspectPortrait;
    [SerializeField] private GameObject _panelTopic;
    [SerializeField] private GameObject _panelDialogue;
    [SerializeField] private TextMeshProUGUI _txtSpeakerName;
    [SerializeField] private TextMeshProUGUI _txtDialogueContent;

    [Header("Evidence UI")]
    [SerializeField] private GameObject _panelEvidence;

    [Header("Intro Dialogue Database")]
    [SerializeField] private DialogueDatabase _introDialogueDatabase;

    [Header("--- HEALTH / PENALTY CONFIG (SLIDER) ---")]
    [SerializeField] private int _maxHealth = 3;            // Mặc định 3 mạng (mỗi lần sai trừ 1/3)
    [SerializeField] private Slider _healthSlider;          // Dùng Slider UI làm thanh máu/lỗi
    private int _currentHealth;

    [Header("--- RESULT PANELS ---")]
    [SerializeField] private GameObject _panelWin;          // Bảng Thắng
    [SerializeField] private GameObject _panelLose;         // Bảng Thua

    [Header("--- TOPICS DATA ---")]
    [SerializeField] private List<TrialTopic> _topicList;    // Danh sách Topic phiên tòa
    private HashSet<TrialTopic> _completedTopics = new HashSet<TrialTopic>();
    private TrialTopic _selectedTopic;

    private GameObject _spawnedSuspect;
    private int _currentIntroIndex = 0;
    private TrialState _currentState;

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 1. Khởi tạo thanh máu Slider
        _currentHealth = _maxHealth;
        if (_healthSlider != null)
        {
            _healthSlider.minValue = 0;
            _healthSlider.maxValue = _maxHealth;
            _healthSlider.value = _currentHealth;
        }

        _completedTopics.Clear();

        if (_panelEvidence != null) _panelEvidence.SetActive(false);
        if (_panelWin != null) _panelWin.SetActive(false);
        if (_panelLose != null) _panelLose.SetActive(false);

        SpawnSuspectModel();
        StartIntroDialogue();
    }

    private void Update()
    {
        // Nhấn phím SPACE để chuyển thoại
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_currentState == TrialState.IntroDialogue ||
                _currentState == TrialState.ResultDialogueSuccess ||
                _currentState == TrialState.ResultDialogueFail)
            {
                OnClickNextDialogue();
            }
        }
    }

    // --- BƯỚC 1: HỘI THOẠI MỞ ĐẦU ---
    private void StartIntroDialogue()
    {
        _currentState = TrialState.IntroDialogue;
        _currentIntroIndex = 0;

        SwitchCamera(CameraView.Judge);

        if (_panelTopic != null) _panelTopic.SetActive(false);
        if (_panelDialogue != null) _panelDialogue.SetActive(true);

        if (_introDialogueDatabase != null && _introDialogueDatabase.Lines != null && _introDialogueDatabase.Lines.Count > 0)
        {
            ShowIntroSentence(_currentIntroIndex);
        }
        else
        {
            ShowTopicSelection();
        }
    }

    private void ShowIntroSentence(int index)
    {
        if (_introDialogueDatabase == null || index >= _introDialogueDatabase.Lines.Count) return;

        DialogueLine currentLine = _introDialogueDatabase.Lines[index];

        if (_txtSpeakerName != null) _txtSpeakerName.text = currentLine.CharacterName;
        if (_txtDialogueContent != null) _txtDialogueContent.text = currentLine.Content;
        if (_imgSuspectPortrait != null && currentLine.CharacterPortrait != null)
        {
            _imgSuspectPortrait.sprite = currentLine.CharacterPortrait;
        }
    }

    // --- BƯỚC 2: BẢNG CHỌN TOPIC ---
    public void ShowTopicSelection()
    {
        _currentState = TrialState.TopicSelection;

        SwitchCamera(CameraView.Player);

        if (_panelDialogue != null) _panelDialogue.SetActive(false);
        if (_panelEvidence != null) _panelEvidence.SetActive(false);

        if (_panelTopic != null) _panelTopic.SetActive(true);
    }

    // --- HÀM CỤ THỂ CHO TOPICUICONTROLLER GỌI ---
    public void OnTopicSelected(TrialTopic topic)
    {
        _selectedTopic = topic;

        // Tắt bảng Topic
        if (_panelTopic != null) _panelTopic.SetActive(false);

        // Mở trực tiếp bảng chọn Bằng chứng
        ShowEvidenceSelection();
    }

    // --- BƯỚC 3: BẢNG CHỌN BẰNG CHỨNG ---
    public void ShowEvidenceSelection()
    {
        _currentState = TrialState.EvidenceSelection;

        if (_panelDialogue != null) _panelDialogue.SetActive(false);

        SwitchCamera(CameraView.Player);

        if (_panelEvidence != null) _panelEvidence.SetActive(true);
    }

    // --- BƯỚC 4: NỘP BẰNG CHỨNG & XỬ LÝ MÁU / THẮNG THUA ---
    public void SubmitEvidence(KeywordData selectedEvidence)
    {
        if (selectedEvidence == null || _currentHealth <= 0) return;

        // Tắt bảng Bằng chứng, Mở Khung Dialogue
        if (_panelEvidence != null) _panelEvidence.SetActive(false);
        if (_panelDialogue != null) _panelDialogue.SetActive(true);

        bool isCorrect = (_selectedTopic != null && selectedEvidence == _selectedTopic.correctEvidence);

        if (isCorrect)
        {
            // === ĐÚNG BẰNG CHỨNG ===
            _currentState = TrialState.ResultDialogueSuccess;
            SwitchCamera(CameraView.Judge);

            if (_selectedTopic != null && !_completedTopics.Contains(_selectedTopic))
            {
                _completedTopics.Add(_selectedTopic);
            }

            if (_txtSpeakerName != null) _txtSpeakerName.text = "Thẩm Phán";
            if (_txtDialogueContent != null && _selectedTopic != null)
            {
                _txtDialogueContent.text = _selectedTopic.winDialogue;
            }
        }
        else
        {
            // === SAI BẰNG CHỨNG -> TRỪ MÁU ===
            _currentState = TrialState.ResultDialogueFail;
            SwitchCamera(CameraView.Suspect);

            TakeDamage(1); // Trừ 1 nấc trên Slider

            if (_txtSpeakerName != null && ConvictedSuspect != null)
                _txtSpeakerName.text = ConvictedSuspect.NpcName;

            if (_txtDialogueContent != null && _selectedTopic != null)
            {
                _txtDialogueContent.text = _selectedTopic.failDialogue;
            }
        }
    }

    // Trừ máu khi chọn sai
    private void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth < 0) _currentHealth = 0;

        UpdateHealthUI();

        // KIỂM TRA ĐIỀU KIỆN THUA
        if (_currentHealth <= 0)
        {
            OnTrialLose();
        }
    }

    // Cập nhật giá trị Slider
    private void UpdateHealthUI()
    {
        if (_healthSlider != null)
        {
            _healthSlider.value = _currentHealth;
        }
    }

    // --- BƯỚC 5: CHUYỂN CÂU THOẠI KHI BẤM SPACE ---
    public void OnClickNextDialogue()
    {
        if (_currentHealth <= 0) return; // Đã thua thì dừng luồng thoại

        switch (_currentState)
        {
            case TrialState.IntroDialogue:
                _currentIntroIndex++;
                if (_introDialogueDatabase != null && _currentIntroIndex < _introDialogueDatabase.Lines.Count)
                {
                    ShowIntroSentence(_currentIntroIndex);
                }
                else
                {
                    ShowTopicSelection();
                }
                break;

            case TrialState.ResultDialogueSuccess:
                // KIỂM TRA ĐIỀU KIỆN THẮNG: Đã hoàn thành tất cả Topic (hoặc đủ số lượng)
                int totalTopicsCount = (_topicList != null && _topicList.Count > 0)
                    ? _topicList.Count
                    : (ConvictedSuspect != null && ConvictedSuspect.TrialTopics != null ? ConvictedSuspect.TrialTopics.Count : 0);

                if (_completedTopics.Count >= totalTopicsCount && totalTopicsCount > 0)
                {
                    OnTrialWin();
                }
                else
                {
                    ShowTopicSelection();
                }
                break;

            case TrialState.ResultDialogueFail:
                // Trả lời sai -> Tự động quay lại bảng chọn Topic
                ShowTopicSelection();
                break;
        }
    }

    private void OnTrialWin()
    {
        if (_panelDialogue != null) _panelDialogue.SetActive(false);
        if (_panelWin != null) _panelWin.SetActive(true);
    }

    private void OnTrialLose()
    {
        if (_panelDialogue != null) _panelDialogue.SetActive(false);
        if (_panelLose != null) _panelLose.SetActive(true);
    }

    public void SwitchCamera(CameraView view)
    {
        if (_camJudge != null) _camJudge.gameObject.SetActive(view == CameraView.Judge);
        if (_camPlayer != null) _camPlayer.gameObject.SetActive(view == CameraView.Player);
        if (_camSuspect != null) _camSuspect.gameObject.SetActive(view == CameraView.Suspect);
    }

    private void SpawnSuspectModel()
    {
        if (ConvictedSuspect == null)
        {
            Debug.LogWarning("[CourtTrial] Chưa gán ConvictedSuspect!");
            return;
        }

        if (ConvictedSuspect.Suspect3DPrefab != null && _suspectSpawnPoint != null)
        {
            _spawnedSuspect = Instantiate(ConvictedSuspect.Suspect3DPrefab, _suspectSpawnPoint.position, _suspectSpawnPoint.rotation);
            _spawnedSuspect.name = ConvictedSuspect.NpcName;
        }

        if (_txtSuspectName != null) _txtSuspectName.text = ConvictedSuspect.NpcName;
        if (_imgSuspectPortrait != null && ConvictedSuspect.Portrait != null)
        {
            _imgSuspectPortrait.sprite = ConvictedSuspect.Portrait;
        }
    }
}