using System;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    SingleDay,
    MultipleDays,
    IntervalDays
}

[Serializable]
public struct DayLocation
{
    public int day;
    public Vector3 position;
    public Vector3 rotation; // Góc xoay (Euler Angles)
}

[CreateAssetMenu(fileName = "NewNPCEvent", menuName = "DetectiveGame/NPC Event Data")]
public class NPCEventData : ScriptableObject
{
    [Header("NPC Identifier")]
    public string npcID;

    [Header("Event Settings")]
    public EventType eventType = EventType.SingleDay;

    [Tooltip("Dùng cho SingleDay: Ngày NPC xuất hiện")]
    public int specificDay = 1;

    [Tooltip("Dùng cho MultipleDays: Danh sách các ngày NPC sẽ xuất hiện")]
    public List<int> activeDays = new List<int>();

    [Tooltip("Dùng cho IntervalDays: Cứ bao nhiêu ngày xuất hiện 1 lần")]
    public int intervalDays = 2;

    [Header("Location Settings")]
    [Tooltip("Vị trí mặc định khi NPC xuất hiện nếu không có thiết lập vị trí riêng")]
    public Vector3 defaultPosition;
    public Vector3 defaultRotation;

    [Tooltip("Danh sách vị trí cụ thể theo từng ngày")]
    public List<DayLocation> dayLocations = new List<DayLocation>();

    public bool ShouldBeActive(int currentDay)
    {
        switch (eventType)
        {
            case EventType.SingleDay:
                return currentDay >= specificDay;

            case EventType.MultipleDays:
                return activeDays != null && activeDays.Contains(currentDay);

            case EventType.IntervalDays:
                if (intervalDays <= 0) return false;
                return (currentDay % intervalDays == 0);

            default:
                return false;
        }
    }

    /// <summary>
    /// Lấy vị trí và góc xoay tương ứng với ngày hiện tại
    /// </summary>
    public bool GetLocationForDay(int currentDay, out Vector3 targetPos, out Quaternion targetRot)
    {
        foreach (var loc in dayLocations)
        {
            if (loc.day == currentDay)
            {
                targetPos = loc.position;
                targetRot = Quaternion.Euler(loc.rotation);
                return true;
            }
        }

        // Nếu không cài đặt ngày riêng thì lấy vị trí mặc định
        targetPos = defaultPosition;
        targetRot = Quaternion.Euler(defaultRotation);
        return false;
    }
}