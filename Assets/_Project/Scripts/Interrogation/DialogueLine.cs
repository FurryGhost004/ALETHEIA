using System;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    [SerializeField] private string _characterName;
    [SerializeField] private Sprite _characterPortrait;
    [TextArea(3, 5)]
    [SerializeField] private string _content;

    public string CharacterName => _characterName;
    public Sprite CharacterPortrait => _characterPortrait;
    public string Content => _content;

    public DialogueLine(string characterName, Sprite characterPortrait, string content)
    {
        _characterName = characterName;
        _characterPortrait = characterPortrait;
        _content = content;
    }
}