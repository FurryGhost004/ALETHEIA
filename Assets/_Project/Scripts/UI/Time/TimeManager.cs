using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    // Lưu lại tên Scene khám phá trước đó để khi thua tòa có đường quay về đúng màn
    public static string LastExplorationScene { get; set; } = "";

    [Header("Time Config")]
    [SerializeField] private int _startHour = 6;
    [SerializeField] private int _endHour = 20; // 20:00 hết ngày
    [SerializeField] private int _minutesPerAction = 15;

    [Header("Current Status")]
    private int _currentDay = 1;
    private int _currentHour = 6;
    private int _currentMinute = 0;

    // --- TÍNH NĂNG MỚI: ĐIỂM SỐ & GIỚI HẠN THUA ---
    [Header("Game Over & Score Settings")]
    [SerializeField] private int _maxAllowedLosses = 3; // Giới hạn thua tối đa ở tòa án
    [SerializeField] private int _startingScore = 100;    // Điểm khởi điểm ban đầu
    [SerializeField] private int _scorePenaltyPerDay = 10;// Số điểm bị trừ mỗi khi sang ngày mới

    public int LossCount { get; private set; } = 0;
    public int CurrentScore { get; private set; }
    private int _lastRecordedDay = 1;

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
        DontDestroyOnLoad(gameObject); // Giữ nguyên dữ liệu thời gian khi đổi Scene
    }

    private void Start()
    {
        // Khởi tạo điểm số ban đầu nếu game mới bắt đầu
        if (CurrentScore == 0)
        {
            CurrentScore = _startingScore;
        }

        // Chỉ khởi tạo mặc định nếu mới bắt đầu game lần đầu
        if (_currentHour == 0)
        {
            _currentHour = _startHour;
            _currentMinute = 0;
            NotifyTimeChanged();
        }
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

        // Tự động trừ điểm khi sang ngày mới
        UpdateScoreOnDayAdvanced();

        Debug.Log($"[TimeManager] Đã hết ngày! Chuyển sang DAY {_currentDay}. Điểm hiện tại: {CurrentScore}");

        NotifyTimeChanged();

        // Đảm bảo dòng này được kích hoạt
        OnDayAdvanced?.Invoke(_currentDay);
    }

    /// <summary>
    /// Trừ điểm dựa trên số ngày trôi qua
    /// </summary>
    private void UpdateScoreOnDayAdvanced()
    {
        if (_currentDay > _lastRecordedDay)
        {
            int daysPassed = _currentDay - _lastRecordedDay;
            CurrentScore -= daysPassed * _scorePenaltyPerDay;
            if (CurrentScore < 0) CurrentScore = 0;

            _lastRecordedDay = _currentDay;
        }
    }

    /// <summary>
    /// Ghi nhận mỗi khi người chơi thua phiên tòa
    /// </summary>
    public bool RegisterLoss()
    {
        LossCount++;
        Debug.Log($"[TimeManager] Thua phiên tòa lần thứ: {LossCount}/{_maxAllowedLosses}");

        // Kiểm tra nếu thua quá giới hạn cho phép
        return LossCount >= _maxAllowedLosses;
    }

    private void NotifyTimeChanged()
    {
        OnTimeChanged?.Invoke(_currentDay, _currentHour, _currentMinute);
    }

    /// <summary>
    /// Đặt lại thời gian và ngày cụ thể (Dùng khi thua phiên tòa: reset ngày mới lúc 10:00 sáng)
    /// </summary>
    public void SetTimeAndDay(int targetDay, int targetHour, int targetMinute = 0)
    {
        _currentDay = targetDay;
        _currentHour = targetHour;
        _currentMinute = targetMinute;

        // Cập nhật lại điểm số nếu việc đổi ngày/giờ có sự chênh lệch lớn
        UpdateScoreOnDayAdvanced();

        Debug.Log($"[TimeManager] Đã đặt lại thời gian: Ngày {_currentDay}, {_currentHour:00}:{_currentMinute:00}");
        NotifyTimeChanged();
    }
}