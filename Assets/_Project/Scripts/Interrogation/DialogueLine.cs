using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    // ── Để DialogueDatabase.LookupResponse() tra cứu ──
    [SerializeField] private string _npcId;
    [SerializeField] private string _whType;
    [SerializeField] private string _keyword;
    [SerializeField] private bool _hasNewInfo;

    // ── Để DialogueUI hiển thị lên màn hình ──
    [SerializeField] private string _characterName;
    [SerializeField] private Sprite _characterPortrait;
    [TextArea(2, 4)]
    [SerializeField] private string _content;

    // ── Properties ──
    public string NpcId => _npcId;
    public string WHType => _whType;
    public string Keyword => _keyword;
    public bool HasNewInfo => _hasNewInfo;
    public string CharacterName => _characterName;
    public Sprite CharacterPortrait => _characterPortrait;
    public string Content => _content;
}