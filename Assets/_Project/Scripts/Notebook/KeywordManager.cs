using System;
using System.Collections.Generic;
using UnityEngine;

public class KeywordManager : SingletonBase<KeywordManager>
{
    [SerializeField] private List<KeywordData> _unlockedKeywords = new List<KeywordData>();

    public IReadOnlyList<KeywordData> UnlockedKeywords => _unlockedKeywords;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start() { }

    public KeywordData GetKeyword(string keywordNameOrId)
    {
        if (string.IsNullOrEmpty(keywordNameOrId)) return null;

        foreach (var keyword in _unlockedKeywords)
        {
            if (keyword == null) continue;

            if (string.Equals(keyword.KeywordName, keywordNameOrId, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(keyword.Id, keywordNameOrId, StringComparison.OrdinalIgnoreCase))
            {
                return keyword;
            }
        }

        return null;
    }

    public void UnlockKeyword(KeywordData keyword)
    {
        if (keyword != null && !_unlockedKeywords.Contains(keyword))
        {
            _unlockedKeywords.Add(keyword);

            // TỰ ĐỘNG ĐỒNG BỘ SANG PLAYER INVENTORY
            if (PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.AddEvidence(keyword);
            }
        }
    }
}