using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [Header("Danh sách bằng chứng đã thu thập")]
    public List<KeywordData> CollectedEvidences = new List<KeywordData>();

    [Header("Bằng chứng mẫu để Test (Dùng khi Play trực tiếp Scene Court)")]
    [SerializeField] private List<KeywordData> _debugTestEvidences = new List<KeywordData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Tự động nạp bằng chứng test nếu chạy riêng Scene Court
#if UNITY_EDITOR
            if (CollectedEvidences.Count == 0 && _debugTestEvidences.Count > 0)
            {
                CollectedEvidences.AddRange(_debugTestEvidences);
            }
#endif
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddEvidence(KeywordData evidence)
    {
        if (evidence != null && !CollectedEvidences.Contains(evidence))
        {
            CollectedEvidences.Add(evidence);
        }
    }

    public void ClearEvidences()
    {
        CollectedEvidences.Clear();
    }
}