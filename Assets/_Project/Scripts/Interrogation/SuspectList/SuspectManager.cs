using System.Collections.Generic;
using UnityEngine;

public class SuspectManager : MonoBehaviour
{
    public static SuspectManager Instance { get; private set; }

    private readonly List<SuspectData> _discoveredSuspects = new List<SuspectData>();

    [SerializeField] private List<SuspectData> _allSuspects = new List<SuspectData>();
    private readonly HashSet<string> _convictedIds = new HashSet<string>();

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

    public SuspectData GetSuspectById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        return _allSuspects?.Find(s => s != null && s.NpcId == id);
    }

    public void MarkAsConvicted(string npcId)
    {
        if (!string.IsNullOrEmpty(npcId))
            _convictedIds.Add(npcId);
    }

    public bool IsConvicted(string npcId)
    {
        return !string.IsNullOrEmpty(npcId) && _convictedIds.Contains(npcId);
    }
}