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

    private void Start()
    {
    }

    private void OnEnable()
    {
        EventBus.Subscribe<KeywordUnlockedEvent>(OnKeywordUnlocked);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<KeywordUnlockedEvent>(OnKeywordUnlocked);
    }

    private void OnKeywordUnlocked(KeywordUnlockedEvent eventData)
    {
        UnlockKeyword(eventData.KeywordData);
    }

    public bool UnlockKeyword(KeywordData keyword)
    {
        if (keyword == null) return false;

        if (HasKeyword(keyword.Id))
        {
            Debug.Log($"{GameConstants.LOG_KEYWORD_DUPLICATE}{keyword.KeywordName}");
            return false;
        }

        _unlockedKeywords.Add(keyword);
        Debug.Log($"{GameConstants.LOG_KEYWORD_UNLOCKED}{keyword.KeywordName}");

        return true;
    }

    public bool HasKeyword(string keywordId)
    {
        if (string.IsNullOrEmpty(keywordId)) return false;

        foreach (KeywordData data in _unlockedKeywords)
        {
            if (data != null && data.Id == keywordId)
            {
                return true;
            }
        }
        return false;
    }
}