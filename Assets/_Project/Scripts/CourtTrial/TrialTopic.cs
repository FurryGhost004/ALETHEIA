using UnityEngine;

[CreateAssetMenu(fileName = "NewTrialTopic", menuName = "Dialogue/Trial Topic")]
public class TrialTopic : ScriptableObject
{
    [Header("Thông tin chủ đề")]
    public string topicName;            // Tên hiển thị trên nút (vd: "Thời gian ngoại phạm")

    [Header("Lời khai của nghi phạm")]
    [TextArea(2, 5)]
    public string introDialogue;        // Lời khai ban đầu khi chọn topic này

    [Header("Kiểm tra bằng chứng")]
    public KeywordData correctEvidence; // Bằng chứng đúng để bác bỏ lời khai

    [TextArea(2, 5)]
    public string winDialogue;          // Lời thoại khi người chơi đưa đúng bằng chứng

    [TextArea(2, 5)]
    public string failDialogue;         // Lời thoại khi người chơi đưa sai bằng chứng
}