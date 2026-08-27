using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueDatabase", menuName = "Interrogation/Dialogue Database")]
public class DialogueDatabase : ScriptableObject
{
    [SerializeField] private List<DialogueLine> _lines;

    public DialogueLine LookupResponse(string npcId, string whType, string keyword)
    {
        if (_lines == null) return null;

        foreach (DialogueLine line in _lines)
        {
            if (line == null) continue;

            bool isNpcMatch = string.Equals(line.NpcId?.Trim(), npcId?.Trim(), System.StringComparison.OrdinalIgnoreCase);
            bool isWHMatch = string.Equals(line.WHType?.Trim(), whType?.Trim(), System.StringComparison.OrdinalIgnoreCase);
            bool isKeywordMatch = string.Equals(line.Keyword?.Trim(), keyword?.Trim(), System.StringComparison.OrdinalIgnoreCase);

            if (isNpcMatch && isWHMatch && isKeywordMatch)
            {
                return line;
            }
        }
        return null;
    }
}