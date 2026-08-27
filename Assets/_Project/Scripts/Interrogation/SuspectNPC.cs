using UnityEngine;

public class SuspectNPC : Interactable
{
    [Header("Suspect Data")]
    [SerializeField] private string _npcId = "Suspect_01";
    [SerializeField] private string _npcName = "Nghi phạm"; // Thêm Tên NPC
    [SerializeField] private Sprite _npcPortrait;          // Thêm Ảnh đại diện NPC
    [SerializeField] private DialogueDatabase _npcDatabase;

    [Header("UI Reference")]
    [SerializeField] private InterrogationUIController _interrogationUI;

    public override void Interact()
    {
        if (_interrogationUI == null)
        {
            _interrogationUI = Object.FindFirstObjectByType<InterrogationUIController>(FindObjectsInactive.Include);
        }

        if (_interrogationUI != null)
        {
            // Truyền đủ 4 tham số: ID, Tên, Ảnh, Database
            _interrogationUI.SetTargetNPC(_npcId, _npcName, _npcPortrait, _npcDatabase);
        }
        else
        {
            Debug.LogError($"[SuspectNPC] Không tìm thấy InterrogationUIController trên Scene!");
        }
    }
}