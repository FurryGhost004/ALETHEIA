// Path: _Project/Scripts/SaveLoad/SaveData.cs

using System;
using System.Collections.Generic;
using UnityEngine;

// ── Wrapper cho Player's Note của từng nghi phạm (Req 6) ─────────────────────
// JsonUtility không hỗ trợ Dictionary → phải dùng class riêng.
//
// Người chơi gõ vào ô Player's Note rồi bấm Enter → note đó hiển thị bên dưới ô.
// Có thể thêm nhiều lần → mỗi lần Enter = 1 phần tử mới trong Notes.
// Mỗi entry = 1 nghi phạm + danh sách các note đã gõ theo thứ tự.
[Serializable]
public class SuspectNoteEntry
{
    public string SuspectId;        // ID của nghi phạm trong Suspect List
    public List<string> Notes;      // Danh sách note theo thứ tự gõ — mỗi Enter = 1 phần tử

    public SuspectNoteEntry()
    {
        Notes = new List<string>();
    }
}

/// <summary>
/// Class chứa toàn bộ data cần lưu khi người chơi Save Case hoặc Quit Game.
/// Đây KHÔNG phải MonoBehaviour — chỉ là data container, được SaveSystem
/// serialize ra JSON và lưu vào file (theo UC03 Continue Case).
///
/// [Serializable] để JsonUtility.ToJson()/FromJson() đọc/ghi được.
///
/// Đáp ứng 6 yêu cầu từ Duy (level designer):
///   Req 1 — Thời gian + sự kiện: CurrentDay, TimeRemaining, [TODO: CompletedEventIds — chờ xác nhận hệ thống event]
///   Req 2 — Vật phẩm đã bị destroy: DestroyedEvidenceIds
///   Req 3 — Danh sách đã tương tác (nhân chứng/vật chứng/nghi phạm): Interacted*Ids
///   Req 4 — Đã kết tội những ai: ConvictedSuspectIds
///   Req 5 — Keyword (liên kết với Req 3): CollectedKeywordIds
///   Req 6 — Player's Note của từng nghi phạm: SuspectNotes
/// </summary>
[Serializable]
public class SaveData
{
    // ── Case hiện tại ─────────────────────────────────────────────────────
    public string CurrentCaseId;

    // ── Vị trí nhân vật (UC06) ────────────────────────────────────────────
    public Vector3 PlayerPosition;

    // ── Req 1 — Thời gian ─────────────────────────────────────────────────
    // TimeManager không có TimeRemaining — chỉ có CurrentDay, CurrentHour, CurrentMinute.
    // Save: TimeManager.Instance.CurrentDay / CurrentHour / CurrentMinute
    // Load: TimeManager.Instance.SetTimeAndDay(CurrentDay, CurrentHour, CurrentMinute)
    public int CurrentDay;
    public int CurrentHour;
    public int CurrentMinute;

    // ── Req 1 (phần sự kiện) — ĐÃ XÁC NHẬN, KHÔNG CẦN THÊM FIELD ──────────
    // NPCEventData là ScriptableObject baked sẵn — Active Days và Day Locations
    // là dữ liệu tĩnh trong asset, không phải state runtime.
    // Khi load lại game, hệ thống NPC Event tự đọc CurrentDay → đối chiếu với
    // Active Days → tính ra NPC nào xuất hiện ở đâu. Không có "event đã fire"
    // nào cần lưu riêng. CurrentDay + TimeRemaining ở trên là đủ cho Req 1.

    // ── Req 2 — Vật phẩm đã bị destroy ──────────────────────────────────
    // EvidenceObject (_destroyOnCollected = true): sau khi inspect xong → Destroy(gameObject).
    // Khi load lại scene, các object có ID trong list này sẽ KHÔNG được spawn lại.
    //
    // *** YÊU CẦU VỚI THỊNH ***
    // Thêm vào EvidenceObject.cs:
    //   [SerializeField] private string _evidenceId;
    //   public string EvidenceId => _evidenceId;
    // Và trước Destroy(gameObject) thêm:
    //   if (SaveLoadManager.Instance != null)
    //       SaveLoadManager.Instance.NotifyEvidenceDestroyed(_evidenceId);
    // Duy điền _evidenceId duy nhất cho từng EvidenceObject trong Inspector.
    public List<string> DestroyedEvidenceIds = new List<string>();

    // ── Req 3 — Danh sách đã tương tác ───────────────────────────────────
    // Khi người chơi tương tác với nhân chứng / vật chứng / nghi phạm lần đầu,
    // ID của họ được thêm vào list tương ứng. Khi load lại, list này được dùng
    // để khôi phục đúng danh sách đã unlock trong Investigation Board.
    //
    // "Vật chứng" (PhysicalEvidence) ở đây là loại vật chứng CỐ ĐỊNH trong scene
    // (EvidenceObject._destroyOnCollected = false), KHÔNG phải vật bị destroy.
    //
    // Save: InvestigationBoardManager.Instance.GetInteractedSuspectIds() (hoặc tương đương)
    // Load: InvestigationBoardManager.Instance.RestoreInteractions(...)
    public List<string> InteractedSuspectIds = new List<string>();
    public List<string> InteractedWitnessIds = new List<string>();
    public List<string> InteractedEvidenceIds = new List<string>(); // vật chứng cố định

    // ── Req 4 — Đã kết tội những ai ──────────────────────────────────────
    // Khi người chơi bấm Convict → SuspectId đó được thêm vào list.
    // Khi load lại, những nghi phạm này vẫn hiển thị trạng thái "đã bị kết tội".
    // Khác với HasScheduledCourtTrial (phiên tòa đang PENDING, chưa diễn ra).
    //
    // Save: gọi SaveConviction(suspectId) trong SaveLoadManager
    // Load: foreach id → đánh dấu suspect đó là "Convicted" trong SuspectManager
    public List<string> ConvictedSuspectIds = new List<string>();

    // ── Req 5 — Keyword (liên kết Req 3) ─────────────────────────────────
    // Keyword được mở khóa khi tương tác với nhân chứng / vật chứng / nghi phạm.
    // Về mặt logic thì liên kết với Req 3, nhưng KeywordManager quản lý riêng
    // → vẫn cần lưu danh sách ID rõ ràng để load lại đúng.
    //
    // Lưu keyword.Id (KeywordData.Id) — xác nhận từ KeywordManager.GetKeyword() dòng 27.
    //
    // *** YÊU CẦU VỚI THỊNH ***
    // KeywordManager cần thêm:
    //   [SerializeField] private List<KeywordData> _allKeywords; // catalog đầy đủ — Duy điền
    //   public KeywordData GetKeywordById(string id) { /* tìm trong _allKeywords */ }
    // Không có catalog này, Load không thể resolve ID → KeywordData để gọi UnlockKeyword lại.
    //
    // Save: foreach kw in KeywordManager.Instance.UnlockedKeywords → lưu kw.Id
    // Load: foreach id → KeywordManager.Instance.GetKeywordById(id) → UnlockKeyword(result)
    public List<string> CollectedKeywordIds = new List<string>();

    // ── Req 6 — Player's Note của từng nghi phạm ─────────────────────────
    // Trong Suspect List, mỗi nghi phạm có ô "Player's Note" để người chơi tự gõ ghi chú.
    // Đây là text tự do, không phải ID → cần lưu cả SuspectId lẫn NoteText.
    //
    // JsonUtility không hỗ trợ Dictionary<string, string> → dùng List<SuspectNoteEntry>.
    // Mỗi entry = 1 nghi phạm + nội dung ghi chú.
    //
    // Save: foreach suspect → lấy List<string> notes từ UI → tạo SuspectNoteEntry {SuspectId, Notes}
    // Load: foreach entry → tìm suspect theo SuspectId → render từng string trong Notes xuống dưới ô input
    public List<SuspectNoteEntry> SuspectNotes = new List<SuspectNoteEntry>();

    // ── Court Trial: lịch phiên tòa đang PENDING (UC15) ──────────────────
    // HasScheduledCourtTrial = true: có phiên tòa đã lên lịch nhưng CHƯA diễn ra.
    // Tách biệt với ConvictedSuspectIds (phiên tòa đã xong, có kết quả).
    public bool HasScheduledCourtTrial;
    public string ScheduledSuspectId;     // SuspectData.NpcId của nghi phạm đang chờ xét xử
    public int ScheduledCourtDay;

    // ── Court Trial: số lần đã hoãn phiên tòa (UC18) ─────────────────────
    public int PostponeCount;

    // ── Scene điều tra cuối cùng (dùng cho ReturnToExplorationScene) ──────
    // TimeManager.LastExplorationScene là static string — mất khi tắt game.
    // Lưu lại để sau khi tòa xử xong (Win hoặc Lose) biết quay về scene nào.
    //
    // Save: gọi SaveLastExplorationScene(TimeManager.LastExplorationScene)
    //       → nên gọi ngay trước khi LoadScene vào tòa án
    // Load: TimeManager.LastExplorationScene = data.LastExplorationScene
    public string LastExplorationScene;
}