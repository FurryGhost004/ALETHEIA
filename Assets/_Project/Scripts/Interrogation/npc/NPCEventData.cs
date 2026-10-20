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

    [Tooltip("Dùng cho SingleDay: Ngày duy nhất NPC xuất hiện")]
    public int specificDay = 1;

    [Tooltip("Dùng cho MultipleDays: Danh sách các ngày NPC sẽ xuất hiện")]
    public List<int> activeDays = new List<int>();

    [Tooltip("Dùng cho IntervalDays: Cứ bao nhiêu ngày xuất hiện 1 lần")]
    public int intervalDays = 2;

    [Header("Location Settings")]
    [Tooltip("Vị trí mặc định khi NPC xuất hiện nếu không có thiết lập vị trí riêng cho ngày đó")]
    public Vector3 defaultPosition;
    public Vector3 defaultRotation;

    [Tooltip("Danh sách vị trí cụ thể theo từng ngày (Không bắt buộc điền hết)")]
    public List<DayLocation> dayLocations = new List<DayLocation>();

    public bool ShouldBeActive(int currentDay)
    {
        switch (eventType)
        {
            case EventType.SingleDay:
                // Sửa từ >= thành == để sự kiện Single chỉ đúng vào ngày cụ thể đó
                return currentDay == specificDay;

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
    /// Lấy vị trí và góc xoay tương ứng với ngày hiện tại. 
    /// Nếu không có cấu hình riêng trong dayLocations, tự động trả về defaultPosition.
    /// </summary>
    public bool GetLocationForDay(int currentDay, out Vector3 targetPos, out Quaternion targetRot)
    {
        foreach (var loc in dayLocations)
        {
            if (loc.day == currentDay)
            {
                targetPos = loc.position;
                targetRot = Quaternion.Euler(loc.rotation);
                return true; // Tìm thấy vị trí riêng theo ngày
            }
        }

        // Nếu không cài đặt ngày riêng thì dùng vị trí mặc định của EventData
        targetPos = defaultPosition;
        targetRot = Quaternion.Euler(defaultRotation);
        return false; // Dùng vị trí mặc định
    }
}