using System.Collections.Generic;
using UnityEngine;

public class InterrogationManager : SingletonBase<InterrogationManager>
{
<<<<<<< HEAD
    [Header("Danh sách tất cả các Interrogation Database")]
    [SerializeField] private List<InterrogationDatabase> _databases = new List<InterrogationDatabase>();
=======
    [SerializeField] private InterrogationDatabase _database;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start() { }
>>>>>>> fa3c7a815dbf0b904972f1ce7b048cb872c571de

    public InterrogationResponseEntry LookupResponse(string npcId, WHType whType, KeywordData keyword)
    {
        if (string.IsNullOrEmpty(npcId) || keyword == null) return null;

        // Quét tìm file Database khớp cả NPC Id và Keyword
        foreach (var db in _databases)
        {
            if (db != null && db.NpcId == npcId && db.Keyword == keyword)
            {
                return db.LookupResponse(whType);
            }
        }

        return null;
    }
}

