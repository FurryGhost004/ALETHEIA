using System.Collections.Generic;
using UnityEngine;

public class SuspectManager : MonoBehaviour
{
    public static SuspectManager Instance { get; private set; }

    private readonly List<SuspectData> _discoveredSuspects = new List<SuspectData>();
    public IReadOnlyList<SuspectData> DiscoveredSuspects => _discoveredSuspects;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Thêm nghi phạm vào danh sách nếu chưa tồn tại
    /// </summary>
    public void AddSuspect(SuspectData suspect)
    {
        if (suspect == null) return;

        // Kiểm tra xem nghi phạm đã có trong danh sách chưa
        if (_discoveredSuspects.Exists(s => s.NpcId == suspect.NpcId))
        {
            return; // Đã thêm trước đó
        }

        _discoveredSuspects.Add(suspect);
        Debug.Log($"[SuspectManager] Đã thêm nghi phạm mới: {suspect.NpcName} (ID: {suspect.NpcId})");
    }
}