using TMPro;
using UnityEngine;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _txtDay;
    [SerializeField] private TextMeshProUGUI _txtTime;

    private void OnEnable()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeChanged += UpdateTimeDisplay;
            // Cập nhật giao diện ngay khi bật
            UpdateTimeDisplay(TimeManager.Instance.CurrentDay, TimeManager.Instance.CurrentHour, TimeManager.Instance.CurrentMinute);
        }
    }

    private void OnDisable()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeChanged -= UpdateTimeDisplay;
        }
    }

    private void UpdateTimeDisplay(int day, int hour, int minute)
    {
        if (_txtDay != null)
        {
            _txtDay.text = $"DAY {day}";
        }

        if (_txtTime != null)
        {
            // Định dạng chuỗi hiển thị ví dụ: 06:00, 11:15
            _txtTime.text = $"{hour:D2}:{minute:D2}";
        }
    }
}