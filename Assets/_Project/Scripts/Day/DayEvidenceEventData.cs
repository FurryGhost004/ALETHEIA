using UnityEngine;

[CreateAssetMenu(fileName = "DayEvidenceEvent_", menuName = "Detective Game/Day Evidence Event Data")]
public class DayEvidenceEventData : ScriptableObject
{
    [Header("Trigger Day")]
    [SerializeField] private int _targetDay = 2; // Ngày mà sự kiện này sẽ xảy ra

    [Header("Event Content")]
    [SerializeField] private DialogueDatabase _eventDialogue; // Đoạn hội thoại hiện lên đầu ngày
    [SerializeField] private KeywordData _rewardKeyword;      // Bằng chứng/Keyword sẽ xuất hiện trong Notebook

    public int TargetDay => _targetDay;
    public DialogueDatabase EventDialogue => _eventDialogue;
    public KeywordData RewardKeyword => _rewardKeyword;
}