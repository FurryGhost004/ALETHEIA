// Path: _Project/Scripts/Core/EventBus.cs

using System;
using System.Collections.Generic;

/// <summary>
/// Hệ thống event trung tâm — các Manager giao tiếp với nhau qua đây
/// thay vì gọi trực tiếp Instance của nhau (tránh tight coupling).
///
/// Ví dụ:
///   KHÔNG làm:  CrossExamManager.Instance.CheckEvidence();
///   NÊN làm:    EventBus.Publish(new EvidenceSelectedEvent(evidenceId));
/// </summary>
public static class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();

    /// <summary>Đăng ký lắng nghe 1 loại event.</summary>
    public static void Subscribe<T>(Action<T> listener)
    {
        Type eventType = typeof(T);

        if (!_subscribers.ContainsKey(eventType))
        {
            _subscribers[eventType] = new List<Delegate>();
        }

        _subscribers[eventType].Add(listener);
    }

    /// <summary>Hủy đăng ký lắng nghe — PHẢI gọi trong OnDisable() để tránh memory leak.</summary>
    public static void Unsubscribe<T>(Action<T> listener)
    {
        Type eventType = typeof(T);

        if (_subscribers.ContainsKey(eventType))
        {
            _subscribers[eventType].Remove(listener);
        }
    }

    /// <summary>Phát event — tất cả listener đã Subscribe loại event này sẽ được gọi.</summary>
    public static void Publish<T>(T eventData)
    {
        Type eventType = typeof(T);

        if (_subscribers.ContainsKey(eventType))
        {
            // ToArray() để tránh lỗi khi listener tự Unsubscribe ngay trong lúc đang được gọi
            foreach (Delegate listener in _subscribers[eventType].ToArray())
            {
                (listener as Action<T>)?.Invoke(eventData);
            }
        }
    }

    /// <summary>Xóa toàn bộ subscriber — chỉ dùng khi load lại game từ đầu hoặc trong Unit Test.</summary>
    public static void Clear()
    {
        _subscribers.Clear();
    }
}

// ═══════════════════════════════════════════════════════
// EVENT DEFINITIONS
// Mỗi event tương ứng với 1 hành động trong Sequence Diagram.
// Thêm event mới vào đây khi cần — KHÔNG tạo file riêng cho từng event.
// ═══════════════════════════════════════════════════════

public struct GameStateChangedEvent
{
    public GameState NewState;
    public GameStateChangedEvent(GameState newState) { NewState = newState; }
}

public struct SceneReadyEvent
{
    public string CaseId;
    public SceneReadyEvent(string caseId) { CaseId = caseId; }
}

public struct SceneRestoredEvent
{
    public string CaseId;
    public SceneRestoredEvent(string caseId) { CaseId = caseId; }
}

public struct EvidenceSelectedEvent
{
    public string EvidenceId;
    public EvidenceSelectedEvent(string evidenceId) { EvidenceId = evidenceId; }
}

public struct EvidenceCollectedEvent
{
    public string EvidenceId;
    public EvidenceCollectedEvent(string evidenceId) { EvidenceId = evidenceId; }
}

public struct ReputationChangedEvent
{
    public int NewReputation;
    public ReputationChangedEvent(int newReputation) { NewReputation = newReputation; }
}

public struct CourtTrialStartedEvent { }

public struct DayTransitionEvent
{
    public int NewDay;
    public DayTransitionEvent(int newDay) { NewDay = newDay; }
}
