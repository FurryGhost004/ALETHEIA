using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Time Config")]
    [SerializeField] private int _startHour = 6;
    [SerializeField] private int _endHour = 20; // 20:00 hết ngày
    [SerializeField] private int _minutesPerAction = 15;

    [Header("Current Status")]
    private int _currentDay = 1;
    private int _currentHour = 6;
    private int _currentMinute = 0;

    // Properties cho các script khác đọc dữ liệu
    public int CurrentDay => _currentDay;
    public int CurrentHour => _currentHour;
    public int CurrentMinute => _currentMinute;

    // Events để các hệ thống khác lắng nghe (UI, NPC Spawner, Bằng chứng)
    public event Action<int, int, int> OnTimeChanged; // (Day, Hour, Minute)
    public event Action<int> OnDayAdvanced;           // Báo hiệu khi sang ngày mới (Day)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _currentHour = _startHour;
        _currentMinute = 0;
        NotifyTimeChanged();
    }

    /// <summary>
    /// Gọi hàm này mỗi khi người chơi thực hiện tương tác (15 phút)
    /// </summary>
    public void AdvanceTime()
    {
        AdvanceTimeMinutes(_minutesPerAction);
    }

    /// <summary>
    /// Cộng thêm số phút tùy chỉnh
    /// </summary>
    public void AdvanceTimeMinutes(int minutes)
    {
        _currentMinute += minutes;

        // Xử lý tràn phút sang giờ
        while (_currentMinute >= 60)
        {
            _currentMinute -= 60;
            _currentHour++;
        }

        // Kiểm tra xem đã đến 20:00 chưa -> Chuyển sang ngày tiếp theo
        if (_currentHour >= _endHour)
        {
            NextDay();
        }
        else
        {
            NotifyTimeChanged();
        }
    }

    private void NextDay()
    {
        _currentDay++;
        _currentHour = _startHour;
        _currentMinute = 0;

        Debug.Log($"[TimeManager] Đã hết ngày! Chuyển sang DAY {_currentDay}");

        NotifyTimeChanged();
        OnDayAdvanced?.Invoke(_currentDay);
    }

    private void NotifyTimeChanged()
    {
        OnTimeChanged?.Invoke(_currentDay, _currentHour, _currentMinute);
    }
}