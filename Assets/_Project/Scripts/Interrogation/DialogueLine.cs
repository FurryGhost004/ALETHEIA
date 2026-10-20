using System;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    [SerializeField] private string _characterName;
    [SerializeField] private Sprite _characterPortrait;
    [TextArea(3, 5)]
    [SerializeField] private string _content;

    [Header("Animation Settings")]
    [Tooltip("Tên Trigger trong Animator của nhân vật khi nói câu này")]
    [SerializeField] private string _animationTrigger; // Thêm trường animation

    public string CharacterName => _characterName;
    public Sprite CharacterPortrait => _characterPortrait;
    public string Content => _content;
    public string AnimationTrigger => _animationTrigger; // Property để gọi từ bên ngoài

    public DialogueLine(string characterName, Sprite characterPortrait, string content, string animationTrigger = "")
    {
        _characterName = characterName;
        _characterPortrait = characterPortrait;
        _content = content;
        _animationTrigger = animationTrigger;
    }
}