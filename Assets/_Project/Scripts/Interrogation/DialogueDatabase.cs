using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueDatabase", menuName = "Dialogue/Dialogue Database")]
public class DialogueDatabase : ScriptableObject
{
    [SerializeField] private List<DialogueLine> _lines = new List<DialogueLine>();

    public IReadOnlyList<DialogueLine> Lines => _lines;
}