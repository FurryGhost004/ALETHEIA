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
    public void AddSuspect(string id, string name, Sprite portrait)
    {
        // Kiểm tra xem nghi phạm đã có trong danh sách chưa
        if (_discoveredSuspects.Exists(s => s.NpcId == id))
        {
            return; // Đã thêm trước đó rồi
        }

        SuspectData newSuspect = new SuspectData(id, name, portrait);
        _discoveredSuspects.Add(newSuspect);
        Debug.Log($"[SuspectManager] Đã thêm nghi phạm mới: {name} (ID: {id})");
    }
}