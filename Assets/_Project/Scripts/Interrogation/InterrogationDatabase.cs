using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC_Keyword_Interrogation", menuName = "Interrogation/NPC Keyword Interrogation")]
public class InterrogationDatabase : ScriptableObject
{
    [Header("NPC & Keyword Config")]
    [SerializeField] private string _npcId;
    [SerializeField] private KeywordData _keyword;

    [Header("Responses List (Cho Keyword trên)")]
    [SerializeField] private List<InterrogationResponseEntry> _responses = new List<InterrogationResponseEntry>();

    public string NpcId => _npcId;
    public KeywordData Keyword => _keyword;
    public List<InterrogationResponseEntry> Responses => _responses;

    // Tìm câu trả lời theo WHType trong file này
    public InterrogationResponseEntry LookupResponse(WHType whType)
    {
        foreach (var entry in _responses)
        {
            if (entry.WhType == whType)
            {
                return entry;
            }
        }
        return null;
    }
}