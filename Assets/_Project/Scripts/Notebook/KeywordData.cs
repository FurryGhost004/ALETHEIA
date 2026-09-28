using UnityEngine;

[CreateAssetMenu(fileName = "NewKeywordData", menuName = "DetectiveGame/Keyword Data")]
public class KeywordData : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private string _keywordName; // Tên Từ khóa (Ví dụ: "CHIAKHOA" - Dùng để nối logic/so sánh)
    [SerializeField] private string _itemName;    // Tên Vật phẩm hiển thị (Ví dụ: "Chìa khóa thư phòng")

    [TextArea(2, 4)]
    [SerializeField] private string _description;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _playerNote;

    public string Id => _id;
    public string KeywordName => _keywordName;

    // Thuộc tính lấy Tên vật phẩm (Nếu để trống _itemName sẽ tự động lấy _keywordName)
    public string ItemName => string.IsNullOrEmpty(_itemName) ? _keywordName : _itemName;

    public string Description => _description;
    public Sprite Icon => _icon;

    // Getter & Setter cho Player Note
    public string PlayerNote
    {
        get => _runtimeNote ?? _playerNote;
        set => _runtimeNote = value;
    }

    [System.NonSerialized]
    private string _runtimeNote; // Lưu ghi chú tạm thời trong phiên chơi
}