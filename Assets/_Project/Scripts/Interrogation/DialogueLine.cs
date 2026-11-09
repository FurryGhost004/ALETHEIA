using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    [SerializeField] private string _characterName;
    [SerializeField] private Sprite _characterPortrait;
    [TextArea(3, 5)]
    [SerializeField] private string _content;

    [Header("Animation Settings")]
    [Tooltip("Danh sách các Trigger trong Animator sẽ được kích hoạt khi chạy câu thoại này")]
    [SerializeField] private List<string> _animationTriggers = new List<string>();

    public string CharacterName => _characterName;
    public Sprite CharacterPortrait => _characterPortrait;
    public string Content => _content;
    public IReadOnlyList<string> AnimationTriggers => _animationTriggers; // Property trả về danh sách trigger

    public DialogueLine(string characterName, Sprite characterPortrait, string content, List<string> animationTriggers = null)
    {
        _characterName = characterName;
        _characterPortrait = characterPortrait;
        _content = content;
        _animationTriggers = animationTriggers ?? new List<string>();
    }
}