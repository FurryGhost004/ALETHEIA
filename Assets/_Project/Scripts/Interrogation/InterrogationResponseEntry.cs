using System;
using UnityEngine;

[Serializable]
public class InterrogationResponseEntry
{
    [SerializeField] private WHType _whType;
    [SerializeField] private DialogueDatabase _dialogueResponse;

    public WHType WhType => _whType;
    public DialogueDatabase DialogueResponse => _dialogueResponse;
}