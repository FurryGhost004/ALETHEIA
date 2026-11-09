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

            // Tự động đồng bộ toàn bộ keyword từ KeywordManager
            SyncAllKeywordsFromManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Lấy toàn bộ keyword từ KeywordManager đưa vào kho của Player
    /// </summary>
    public void SyncAllKeywordsFromManager()
    {
        CollectedEvidences.Clear();

        // 1. Thử lấy thông qua Instance của KeywordManager (nếu có)
        if (KeywordManager.Instance != null && KeywordManager.Instance.UnlockedKeywords != null)
        {
            CollectedEvidences.AddRange(KeywordManager.Instance.UnlockedKeywords);
        }
        else
        {
            // 2. Nếu không tìm thấy Instance, tìm trực tiếp trong Scene
            KeywordManager kwManager = Object.FindAnyObjectByType<KeywordManager>();
            if (kwManager != null && kwManager.UnlockedKeywords != null)
            {
                CollectedEvidences.AddRange(kwManager.UnlockedKeywords);
            }
        }

        // 3. Dự phòng trường hợp chạy riêng Scene Court mà KeywordManager chưa có sẵn dữ liệu
#if UNITY_EDITOR
        if (CollectedEvidences.Count == 0 && _debugTestEvidences.Count > 0)
        {
            CollectedEvidences.AddRange(_debugTestEvidences);
        }
#endif
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