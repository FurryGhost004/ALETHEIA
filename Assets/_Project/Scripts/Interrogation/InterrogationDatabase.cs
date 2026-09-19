using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InterrogationResponseEntry
{
    [SerializeField] private string _npcId;
    [SerializeField] private WHType _whType;
    [SerializeField] private KeywordData _keyword;
    [SerializeField] private bool _hasNewInfo;
    [SerializeField] private DialogueDatabase _responseDialogue;

    public string NpcId => _npcId;
    public WHType WhType => _whType;
    public KeywordData Keyword => _keyword;
    public bool HasNewInfo => _hasNewInfo;
    public DialogueDatabase ResponseDialogue => _responseDialogue;
}

[CreateAssetMenu(fileName = "NewInterrogationDatabase", menuName = "Interrogation/Interrogation Database")]
public class InterrogationDatabase : ScriptableObject
{
    [SerializeField] private List<InterrogationResponseEntry> _responses = new List<InterrogationResponseEntry>();

    public IReadOnlyList<InterrogationResponseEntry> Responses => _responses;
}