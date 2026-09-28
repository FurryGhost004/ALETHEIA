using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSuspectData", menuName = "DetectiveGame/Suspect Data")]
public class SuspectData : ScriptableObject
{
    [SerializeField] private string _npcId;
    [SerializeField] private string _npcName;
    [SerializeField] private Sprite _portrait;
    [SerializeField] private string _address;
    [SerializeField] private string _height;

    [TextArea(3, 5)]
    [SerializeField] private string _description;

    [Header("Conviction Settings")]
    [SerializeField] private bool _isCulprit; // Tích true nếu đây là hung thủ thật sự
    [SerializeField] private GameObject _suspect3DPrefab; // Prefab mô hình 3D của nghi phạm
    public GameObject Suspect3DPrefab => _suspect3DPrefab;

    [Header("Trial Data")]
    [SerializeField] private List<TrialTopic> _trialTopics = new List<TrialTopic>();

    // Getter cho TopicUIController truy cập
    public List<TrialTopic> TrialTopics => _trialTopics;

    // Public Properties cho phép SuspectListUI truy cập
    public string NpcId => _npcId;
    public string NpcName => _npcName;
    public Sprite Portrait => _portrait;
    public string Address => _address;
    public string Height => _height;
    public string Description => _description; // Khai báo dạng Property để tránh lỗi method group
    public bool IsCulprit => _isCulprit;

}