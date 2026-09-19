using System;
using System.Collections.Generic;
using UnityEngine;

public class KeywordManager : SingletonBase<KeywordManager>
{
    [SerializeField] private List<KeywordData> _unlockedKeywords = new List<KeywordData>();

    public IReadOnlyList<KeywordData> UnlockedKeywords => _unlockedKeywords;

    private void Awake() { }

    private void Start() { }

    /// <summary>
    /// Tìm kiếm KeywordData theo Tên từ khóa hoặc ID.
    /// </summary>
    public KeywordData GetKeyword(string keywordNameOrId)
    {
        if (string.IsNullOrEmpty(keywordNameOrId)) return null;

        foreach (var keyword in _unlockedKeywords)
        {
            if (keyword == null) continue;

            // So sánh với KeywordName hoặc Id của KeywordData
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
        }
    }
}