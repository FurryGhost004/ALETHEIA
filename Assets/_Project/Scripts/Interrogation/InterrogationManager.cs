using UnityEngine;

public class InterrogationManager : SingletonBase<InterrogationManager>
{
    [SerializeField] private InterrogationDatabase _database;

    private void Awake() { }

    private void Start() { }

    public InterrogationResponseEntry LookupResponse(string npcId, WHType whType, KeywordData keyword)
    {
        if (_database == null) return null;

        foreach (var entry in _database.Responses)
        {
            if (entry.NpcId == npcId && entry.WhType == whType && entry.Keyword == keyword)
            {
                return entry;
            }
        }

        return null;
    }
}