using UnityEngine;

public class SuspectNPC : Interactable
{
    [Header("Suspect Data")]
    [SerializeField] private string _npcId = "Suspect_01";
    [SerializeField] private string _npcName = "Nghi phạm 01";
    [SerializeField] private Sprite _npcPortrait;
    [SerializeField] private DialogueDatabase _npcDatabase;

    [Header("UI Reference")]
    [SerializeField] private InterrogationUIController _interrogationUI;

    public override void Interact()
    {
        // 1. Tự động thêm NPC này vào Danh sách nghi phạm khi tương tác lần đầu
        if (SuspectManager.Instance != null)
        {
            SuspectManager.Instance.AddSuspect(_npcId, _npcName, _npcPortrait);
        }

        // 2. Mở giao diện Thẩm vấn
        if (_interrogationUI == null)
        {
            _interrogationUI = Object.FindFirstObjectByType<InterrogationUIController>(FindObjectsInactive.Include);
        }

        if (_interrogationUI != null)
        {
            _interrogationUI.SetTargetNPC(_npcId, _npcName, _npcPortrait, _npcDatabase);
        }
        else
        {
            Debug.LogError($"[SuspectNPC] Không tìm thấy InterrogationUIController trên Scene!");
        }

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.AdvanceTime();
        }
    }
}