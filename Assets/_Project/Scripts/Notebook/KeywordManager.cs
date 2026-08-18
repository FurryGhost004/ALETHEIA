using System;
using System.Collections.Generic;
using UnityEngine;

public class KeywordManager : MonoBehaviour
{
    public static KeywordManager Instance { get; private set; }

    [SerializeField] private List<KeywordData> _unlockedKeywords = new List<KeywordData>();

    public event Action<KeywordData> OnKeywordUnlocked;

    public IReadOnlyList<KeywordData> UnlockedKeywords => _unlockedKeywords;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void UnlockKeyword(KeywordData keyword)
    {
        if (keyword == null) return;

        if (!_unlockedKeywords.Contains(keyword))
        {
            _unlockedKeywords.Add(keyword);
            Debug.Log($"[Keyword Manager] Đã mở khóa từ khóa mới: {keyword.KeywordName}");
            OnKeywordUnlocked?.Invoke(keyword);
        }
        else
        {
            Debug.Log($"[Keyword Manager] Từ khóa đã tồn tại: {keyword.KeywordName}");
        }
    }
}