using System.Collections.Generic;
using UnityEngine;

public class DayEvidenceEventManager : MonoBehaviour
{
    [Header("List of Day Events")]
    [SerializeField] private List<DayEvidenceEventData> _dayEvents = new List<DayEvidenceEventData>();

    private bool _isSubscribed = false;

    private void Start()
    {
        TrySubscribe();

        // Kiểm tra ngay ngày hiện tại khi bắt đầu Scene (nếu load vào game ở ngày > 1)
        if (TimeManager.Instance != null)
        {
            CheckAndTriggerEvent(TimeManager.Instance.CurrentDay);
        }
    }

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        if (TimeManager.Instance != null && _isSubscribed)
        {
            TimeManager.Instance.OnDayAdvanced -= CheckAndTriggerEvent;
            _isSubscribed = false;
        }
    }

    private void TrySubscribe()
    {
        if (!_isSubscribed && TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDayAdvanced += CheckAndTriggerEvent;
            _isSubscribed = true;
        }
    }

    private void CheckAndTriggerEvent(int currentDay)
    {
        foreach (var dayEvent in _dayEvents)
        {
            if (dayEvent != null && dayEvent.TargetDay == currentDay)
            {
                TriggerEvent(dayEvent);
                break; // Chỉ chạy 1 sự kiện tương ứng với ngày đó
            }
        }
    }

    private void TriggerEvent(DayEvidenceEventData eventData)
    {
        Debug.Log($"<color=cyan>[DayEvidenceEvent]</color> Kích hoạt sự kiện cho ngày: {eventData.TargetDay}");

        // 1. Hiển thị đoạn hội thoại đầu ngày
        if (eventData.EventDialogue != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(eventData.EventDialogue);
        }
        else if (ThinkingFeatureUI.Instance != null && eventData.EventDialogue == null)
        {
            ThinkingFeatureUI.Instance.ShowThinking($"Đã bước sang ngày mới. Có thông tin mới cần chú ý.");
        }

        // 2. Tự động thêm bằng chứng / Keyword vào Notebook (KeywordManager)
        if (eventData.RewardKeyword != null && KeywordManager.Instance != null)
        {
            KeywordManager.Instance.UnlockKeyword(eventData.RewardKeyword);
            Debug.Log($"<color=green>[DayEvidenceEvent]</color> Đã thêm bằng chứng vào Notebook: {eventData.RewardKeyword.name}");
        }
    }
}