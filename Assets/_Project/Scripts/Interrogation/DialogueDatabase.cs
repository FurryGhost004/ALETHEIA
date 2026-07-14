using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Database")]
public class DialogueDatabase : ScriptableObject
{
    [SerializeField] private DialogueLine[] _lines;

    public DialogueLine LookupResponse(string npcId, string whType, string keyword)
    {
        foreach (DialogueLine line in _lines)
        {
            if (line.NpcId == npcId &&
                line.WHType == whType &&
                line.Keyword == keyword)
            {
                return line;
            }
        }
        return null; // → NPCController gọi displayVagueReply()
    }
}