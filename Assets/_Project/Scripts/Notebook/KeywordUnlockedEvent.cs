public struct KeywordUnlockedEvent
{
    public KeywordData KeywordData { get; }

    public KeywordUnlockedEvent(KeywordData keywordData)
    {
        KeywordData = keywordData;
    }
}
