using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotebookDetailUI : MonoBehaviour
{
    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI _txtEvidenceName;
    [SerializeField] private TextMeshProUGUI _txtLocationValue;
    [SerializeField] private TextMeshProUGUI _txtSizeValue;
    [SerializeField] private TextMeshProUGUI _txtKeywordValue; // TextMeshPro hiển thị ở mục Relate Keyword
    [SerializeField] private TextMeshProUGUI _txtDescriptionValue;

    [Header("Player Note Customization")]
    [SerializeField] private TMP_InputField _inputNoteValue; // Kéo Txt_Note_Value (InputField) vào đây
    [SerializeField] private Button _btnSaveNote;            // Kéo Btn_Re-Inspect vào đây

    private KeywordData _currentSelectedKeyword;

    private void Awake()
    {
        if (_btnSaveNote != null)
        {
            _btnSaveNote.onClick.AddListener(SavePlayerNote);
        }
    }

    public void DisplayDetails(KeywordData data)
    {
        _currentSelectedKeyword = data;

        if (_currentSelectedKeyword == null)
        {
            ClearDetails();
            return;
        }

        // 1. Tiêu đề lớn: Lấy ItemName ("Chìa Khóa Phòng")
        if (_txtEvidenceName != null) _txtEvidenceName.text = data.ItemName;

        // 2. Ô Relate Keyword: Lấy KeywordName ("ChiaKhoa")
        if (_txtKeywordValue != null) _txtKeywordValue.text = data.KeywordName;

        // 3. Mô tả vật phẩm
        if (_txtDescriptionValue != null) _txtDescriptionValue.text = data.Description;

        // Hiển thị ghi chú của người chơi đã nhập trước đó
        if (_inputNoteValue != null)
        {
            _inputNoteValue.text = data.PlayerNote;
        }
    }

    public void SavePlayerNote()
    {
        if (_currentSelectedKeyword != null && _inputNoteValue != null)
        {
            _currentSelectedKeyword.PlayerNote = _inputNoteValue.text;
            Debug.Log($"[Notebook] Đã lưu ghi chú cho {_currentSelectedKeyword.KeywordName}: '{_inputNoteValue.text}'");
        }
    }

    public void ClearDetails()
    {
        _currentSelectedKeyword = null;
        if (_txtEvidenceName != null) _txtEvidenceName.text = string.Empty;
        if (_txtKeywordValue != null) _txtKeywordValue.text = string.Empty;
        if (_txtLocationValue != null) _txtLocationValue.text = string.Empty;
        if (_txtSizeValue != null) _txtSizeValue.text = string.Empty;
        if (_txtDescriptionValue != null) _txtDescriptionValue.text = string.Empty;
        if (_inputNoteValue != null) _inputNoteValue.text = string.Empty;
    }
}