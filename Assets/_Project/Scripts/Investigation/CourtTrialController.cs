using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private Slider _healthSlider;
    private int _currentHealth;

    [Header("--- RESULT PANELS ---")]
    [SerializeField] private GameObject _panelWin;
    [SerializeField] private GameObject _panelLose;

    [Header("--- TOPICS DATA ---")]
    [SerializeField] private List<TrialTopic> _topicList;
    private HashSet<TrialTopic> _completedTopics = new HashSet<TrialTopic>();
    private TrialTopic _selectedTopic;

    private GameObject _spawnedSuspect;
    private int _currentIntroIndex = 0;
    private int _currentResultIndex = 0;
    private TrialState _currentState;

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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

    private void ShowDialogueLine(DialogueDatabase database, int index, string defaultSpeaker = "")
    {
        if (database == null || database.Lines == null || index >= database.Lines.Count) return;

        DialogueLine currentLine = database.Lines[index];

        if (_txtSpeakerName != null)
            _txtSpeakerName.text = !string.IsNullOrEmpty(currentLine.CharacterName) ? currentLine.CharacterName : defaultSpeaker;

        if (_txtDialogueContent != null)
            _txtDialogueContent.text = currentLine.Content;

        if (_imgSuspectPortrait != null && currentLine.CharacterPortrait != null)
        {
            _imgSuspectPortrait.sprite = currentLine.CharacterPortrait;
        }
    }

    private void StartIntroDialogue()
    {
        _currentState = TrialState.IntroDialogue;
        _currentIntroIndex = 0;

        SwitchCamera(CameraView.Judge);

        if (_panelTopic != null) _panelTopic.SetActive(false);
        if (_panelDialogue != null) _panelDialogue.SetActive(true);

        if (_introDialogueDatabase != null && _introDialogueDatabase.Lines != null && _introDialogueDatabase.Lines.Count > 0)
        {
            ShowDialogueLine(_introDialogueDatabase, _currentIntroIndex, "Thẩm Phán");
        }
        else
        {
            ShowTopicSelection();
        }
    }

    public void ShowTopicSelection()
    {
        _currentState = TrialState.TopicSelection;

        SwitchCamera(CameraView.Player);

        if (_panelDialogue != null) _panelDialogue.SetActive(false);
        if (_panelEvidence != null) _panelEvidence.SetActive(false);
        if (_panelTopic != null) _panelTopic.SetActive(true);
    }

    public void OnTopicSelected(TrialTopic topic)
    {
        _selectedTopic = topic;

        if (_panelTopic != null) _panelTopic.SetActive(false);
        ShowEvidenceSelection();
    }

    public void ShowEvidenceSelection()
    {
        _currentState = TrialState.EvidenceSelection;

        if (_panelDialogue != null) _panelDialogue.SetActive(false);
        SwitchCamera(CameraView.Player);

        if (_panelEvidence != null) _panelEvidence.SetActive(true);
    }

    public void SubmitEvidence(KeywordData selectedEvidence)
    {
        if (selectedEvidence == null || _currentHealth <= 0) return;

        if (_panelEvidence != null) _panelEvidence.SetActive(false);
        if (_panelDialogue != null) _panelDialogue.SetActive(true);

        bool isCorrect = (_selectedTopic != null && selectedEvidence == _selectedTopic.correctEvidence);

        _currentResultIndex = 0;

        if (isCorrect)
        {
            _currentState = TrialState.ResultDialogueSuccess;
            SwitchCamera(CameraView.Judge);

            if (_selectedTopic != null && !_completedTopics.Contains(_selectedTopic))
            {
                _completedTopics.Add(_selectedTopic);
            }

            if (_selectedTopic != null && _selectedTopic.winDialogue != null && _selectedTopic.winDialogue.Lines.Count > 0)
            {
                ShowDialogueLine(_selectedTopic.winDialogue, _currentResultIndex, "Thẩm Phán");
            }
            else
            {
                ProceedAfterSuccessDialogue();
            }
        }
        else
        {
            _currentState = TrialState.ResultDialogueFail;
            SwitchCamera(CameraView.Suspect);

            TakeDamage(1);

            string suspectName = (ConvictedSuspect != null) ? ConvictedSuspect.NpcName : "Nghi phạm";

            if (_selectedTopic != null && _selectedTopic.failDialogue != null && _selectedTopic.failDialogue.Lines.Count > 0)
            {
                ShowDialogueLine(_selectedTopic.failDialogue, _currentResultIndex, suspectName);
            }
            else
            {
                ShowTopicSelection();
            }
        }
    }

    private void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth < 0) _currentHealth = 0;

        UpdateHealthUI();

        if (_currentHealth <= 0)
        {
            OnTrialLose();
        }
    }

    private void UpdateHealthUI()
    {
        if (_healthSlider != null)
        {
            _healthSlider.value = _currentHealth;
        }
    }

    public void OnClickNextDialogue()
    {
        if (_currentHealth <= 0) return;

        switch (_currentState)
        {
            case TrialState.IntroDialogue:
                _currentIntroIndex++;
                if (_introDialogueDatabase != null && _currentIntroIndex < _introDialogueDatabase.Lines.Count)
                {
                    ShowDialogueLine(_introDialogueDatabase, _currentIntroIndex, "Thẩm Phán");
                }
                else
                {
                    ShowTopicSelection();
                }
                break;

            case TrialState.ResultDialogueSuccess:
                _currentResultIndex++;
                if (_selectedTopic != null && _selectedTopic.winDialogue != null && _currentResultIndex < _selectedTopic.winDialogue.Lines.Count)
                {
                    ShowDialogueLine(_selectedTopic.winDialogue, _currentResultIndex, "Thẩm Phán");
                }
                else
                {
                    ProceedAfterSuccessDialogue();
                }
                break;

            case TrialState.ResultDialogueFail:
                _currentResultIndex++;
                if (_selectedTopic != null && _selectedTopic.failDialogue != null && _currentResultIndex < _selectedTopic.failDialogue.Lines.Count)
                {
                    string suspectName = (ConvictedSuspect != null) ? ConvictedSuspect.NpcName : "Nghi phạm";
                    ShowDialogueLine(_selectedTopic.failDialogue, _currentResultIndex, suspectName);
                }
                else
                {
                    ShowTopicSelection();
                }
                break;
        }
    }

    private void ProceedAfterSuccessDialogue()
    {
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

        // 1. Tự động cộng sang ngày hôm sau và set đồng hồ lúc 10:00 sáng
        if (TimeManager.Instance != null)
        {
            int nextDay = TimeManager.Instance.CurrentDay + 1;
            TimeManager.Instance.SetTimeAndDay(nextDay, 10, 0);
        }

        // 2. TỰ ĐỘNG DỊCH CHUYỂN VỀ SCENE TRƯỚC NGAY LẬP TỨC
        // (Hoặc bạn có thể dùng Invoke("ReturnToExplorationScene", 2f); nếu muốn đợi 2 giây cho người chơi đọc chữ rồi mới chuyển)
        ReturnToExplorationScene();
    }

    // --- HÀM GÁN VÀO NÚT BẤM TRÊN BẢNG THUA ĐỂ QUAY VỀ MÀN CHƠI CŨ ---
    public void ReturnToExplorationScene()
    {
        string targetScene = TimeManager.LastExplorationScene;

        // Kiểm tra xem biến lưu tên scene có bị rỗng hay không
        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogWarning("[CourtTrial] LastExplorationScene đang bị trống! Đang fallback về 'GameScene'. Hãy đảm bảo bạn đã gán TimeManager.LastExplorationScene trước khi vào tòa.");
            targetScene = "GameScene";
        }

        Debug.Log($"[CourtTrial] Đang thực hiện chuyển cảnh về lại: {targetScene}");
        SceneManager.LoadScene(targetScene);
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