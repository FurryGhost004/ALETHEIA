using UnityEngine;

public class InspectionHotspot : MonoBehaviour
{
    [TextArea(2, 4)]
    [SerializeField] private string _innerMonologue = "Vết máu này trông còn rất mới...";
    [SerializeField] private KeywordData _associatedKeyword;

    public string InnerMonologue => _innerMonologue;
    public KeywordData AssociatedKeyword => _associatedKeyword;

    private void Awake()
    {
    }

    private void Start()
    {
    }

    public void OnDiscovered()
    {
        Debug.Log($"{GameConstants.LOG_DETECTIVE_THOUGHT}{_innerMonologue}");

        if (_associatedKeyword != null)
        {
            EventBus.Publish(new KeywordUnlockedEvent(_associatedKeyword));
        }
    }
}