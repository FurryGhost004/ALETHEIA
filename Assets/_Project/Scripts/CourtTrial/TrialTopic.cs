using UnityEngine;

[CreateAssetMenu(fileName = "NewTrialTopic", menuName = "Dialogue/Trial Topic")]
public class TrialTopic : ScriptableObject
{
    [Header("Thông tin chủ đề")]
    public string topicName;            // Tên hiển thị trên nút (vd: "Thời gian ngoại phạm")

    [Header("Lời khai của nghi phạm")]
    public DialogueDatabase introDialogue;        // Database hội thoại lời khai ban đầu khi chọn topic này

    [Header("Kiểm tra bằng chứng")]
    public KeywordData correctEvidence; // Bằng chứng đúng để bác bỏ lời khai

    [Header("Kết quả phiên tòa")]
    public DialogueDatabase winDialogue;          // Database hội thoại khi người chơi đưa đúng bằng chứng
    public DialogueDatabase failDialogue;         // Database hội thoại khi người chơi đưa sai bằng chứng
}