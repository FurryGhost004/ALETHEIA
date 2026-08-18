using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [SerializeField] private string _npcId;
    [SerializeField] private string _whType;
    [SerializeField] private string _keyword;
    [SerializeField] private bool _hasNewInfo;
    [SerializeField] private string _characterName;
    [SerializeField] private Sprite _characterPortrait;
    [TextArea(3, 5)]
    [SerializeField] private string _content;

    public string NpcId => _npcId;
    public string WHType => _whType;
    public string Keyword => _keyword;
    public bool HasNewInfo => _hasNewInfo;
    public string CharacterName => _characterName;
    public Sprite CharacterPortrait => _characterPortrait;
    public string Content => _content;
}