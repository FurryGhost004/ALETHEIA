using UnityEngine;

[CreateAssetMenu(fileName = "NewKeywordData", menuName = "DetectiveGame/Keyword Data")]
public class KeywordData : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private string _keywordName;

    [TextArea(2, 4)]
    [SerializeField] private string _description;
    [SerializeField] private Sprite _icon;

    public string Id => _id;
    public string KeywordName => _keywordName;
    public string Description => _description;
    public Sprite Icon => _icon;
}