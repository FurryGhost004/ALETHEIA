// Path: _Project/Scripts/SaveLoad/SaveLoadManager.cs

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tầng CAO của Save/Load — gom dữ liệu game thật thành SaveData,
/// rồi giao cho SaveSystem ghi xuống disk.
///
/// Theo Sequence Diagram:
/// - Pause Menu (Save Case):   PauseMenuUI  → SaveGame()
/// - Convict (UC15):           GameManager  → SaveConviction()
/// - Schedule court (UC15):    GameManager  → SaveCourtSchedule()
/// - Continue Case (UC03):     GameManager  → LoadGame()
/// </summary>
public class SaveLoadManager : SingletonBase<SaveLoadManager>
{
    // Giữ SaveData trong RAM để các method partial update không phải đọc lại file từ disk.
    private SaveData _currentSaveData;
    public SaveData CurrentSaveData => _currentSaveData;

    // ─────────────────────────────────────────────────────────────────────
    // SAVE FULL
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// UC13 Pause Menu (Save Case): Gom toàn bộ trạng thái game thành SaveData
    /// rồi ghi xuống disk qua SaveSystem.
    /// </summary>
    public void SaveGame()
    {
        SaveData data = _currentSaveData ?? new SaveData();

        // ── Vị trí player ────────────────────────────────────────────────
        // TODO: data.PlayerPosition = PlayerController.Instance.transform.position;

        // ── Req 1: Thời gian ──────────────────────────────────────────────
        // NPCEventData là ScriptableObject tĩnh — chỉ cần lưu CurrentDay + giờ/phút là đủ.
        // TimeManager.Instance.SetTimeAndDay(day, hour, minute) dùng để restore khi Load.
        if (TimeManager.Instance != null)
        {
            data.CurrentDay = TimeManager.Instance.CurrentDay;
            data.CurrentHour = TimeManager.Instance.CurrentHour;
            data.CurrentMinute = TimeManager.Instance.CurrentMinute;
        }

        // ── Req 2: Vật phẩm đã bị destroy ───────────────────────────────
        // KHÔNG gom ở đây — EvidenceObject tự gọi NotifyEvidenceDestroyed() realtime.
        // _currentSaveData.DestroyedEvidenceIds đã được cập nhật liên tục.

        // ── Req 3: Danh sách đã tương tác ───────────────────────────────
        // Nhân chứng + nghi phạm đều là SuspectData trong SuspectManager — gộp chung một list.
        if (SuspectManager.Instance != null)
        {
            data.InteractedNpcIds = new System.Collections.Generic.List<string>();
            foreach (SuspectData s in SuspectManager.Instance.DiscoveredSuspects)
                if (!string.IsNullOrEmpty(s.NpcId))
                    data.InteractedNpcIds.Add(s.NpcId);
        }
        // TODO: data.InteractedEvidenceIds = ... (chờ file quản lý vật chứng cố định)

        // ── Req 4: Đã kết tội những ai ──────────────────────────────────
        // KHÔNG gom ở đây — SaveConviction() đã ghi realtime khi người chơi bấm Convict.

        // ── Req 5: Keyword đã mở khóa ────────────────────────────────────
        // KeywordManager.Instance.UnlockedKeywords đã có sẵn (IReadOnlyList<KeywordData>).
        // Lấy Id từ mỗi item — KeywordData.Id xác nhận từ dòng 26-27 KeywordManager.cs.
        // Lưu ý: UnlockKeyword() tự sync sang PlayerInventory (dòng 44-47 KeywordManager.cs).
        if (KeywordManager.Instance != null)
        {
            data.CollectedKeywordIds = new System.Collections.Generic.List<string>();
            foreach (KeywordData kw in KeywordManager.Instance.UnlockedKeywords)
                if (kw != null && !string.IsNullOrEmpty(kw.Id))
                    data.CollectedKeywordIds.Add(kw.Id);
        }
        // *** YÊU CẦU THỊNH: thêm _allKeywords catalog + GetKeywordById() vào KeywordManager.cs
        // để Load side có thể resolve ID → KeywordData và gọi UnlockKeyword() lại.

        // ── Req 6: Player's Note của từng nghi phạm ───────────────────────
        // TODO: data.SuspectNotes = SuspectListUI.Instance.GetAllSuspectNotes();
        //   GetAllSuspectNotes() trả về List<SuspectNoteEntry>, mỗi entry = {SuspectId, NoteText}

        _currentSaveData = data;
        SaveSystem.Instance.SaveData(data);
    }

    // ─────────────────────────────────────────────────────────────────────
    // LOAD
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// UC03 Continue Case: Đọc file save từ disk, trả về SaveData để
    /// GameManager dùng cho SceneLoader.RestoreState().
    /// </summary>
    public SaveData LoadGame()
    {
        _currentSaveData = SaveSystem.Instance.ReadSaveFile();
        return _currentSaveData;
    }

    public void RestoreState(SaveData data)
    {
        if (data == null) return;

        // Req 1: Thời gian
        if (TimeManager.Instance != null)
            TimeManager.Instance.SetTimeAndDay(data.CurrentDay, data.CurrentHour, data.CurrentMinute);

        if (!string.IsNullOrEmpty(data.LastExplorationScene))
            TimeManager.LastExplorationScene = data.LastExplorationScene;

        // Req 3: Nhân chứng + nghi phạm đã tương tác
        if (SuspectManager.Instance != null)
        {
            foreach (string id in data.InteractedNpcIds)
            {
                SuspectData s = SuspectManager.Instance.GetSuspectById(id);
                if (s != null) SuspectManager.Instance.AddSuspect(s);
            }
        }

        // Req 4: Đánh dấu đã kết tội
        if (SuspectManager.Instance != null)
        {
            foreach (string id in data.ConvictedSuspectIds)
                SuspectManager.Instance.MarkAsConvicted(id);
        }

        // Req 4: Khôi phục ConvictedSuspect nếu có phiên tòa đang chờ
        if (data.HasScheduledCourtTrial && !string.IsNullOrEmpty(data.ScheduledSuspectId)
            && SuspectManager.Instance != null)
        {
            SuspectData pending = SuspectManager.Instance.GetSuspectById(data.ScheduledSuspectId);
            if (pending != null) CourtTrialController.ConvictedSuspect = pending;
        }

        // Req 5: Keyword đã mở khóa
        if (KeywordManager.Instance != null)
        {
            foreach (string id in data.CollectedKeywordIds)
            {
                KeywordData kw = KeywordManager.Instance.GetKeywordById(id);
                if (kw != null) KeywordManager.Instance.UnlockKeyword(kw);
            }
        }

        // Req 6: TODO sau khi có SuspectItemUI.cs
        // Req 2: EvidenceObject tự xử lý trong Awake/Start — xem EvidenceObject.cs
        // Vị trí player: TODO sau khi có PlayerController
    }

    // ─────────────────────────────────────────────────────────────────────
    // PARTIAL UPDATE — Các sự kiện ghi ngay lập tức, không chờ SaveGame()
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Req 2 — Gọi bởi EvidenceObject ngay TRƯỚC Destroy(gameObject).
    /// Cập nhật realtime thay vì đợi SaveGame() quét scene.
    ///
    /// Cách thêm vào EvidenceObject.cs (cuối InspectRoutine, trước Destroy):
    ///   if (SaveLoadManager.Instance != null)
    ///       SaveLoadManager.Instance.NotifyEvidenceDestroyed(_evidenceId);
    ///   Destroy(gameObject);
    /// </summary>
    public void NotifyEvidenceDestroyed(string evidenceId)
    {
        if (string.IsNullOrEmpty(evidenceId)) return;

        SaveData data = _currentSaveData ?? new SaveData();
        if (!data.DestroyedEvidenceIds.Contains(evidenceId))
            data.DestroyedEvidenceIds.Add(evidenceId);

        _currentSaveData = data;
    }

    /// <summary>
    /// Lưu tên scene điều tra cuối cùng — gọi ngay trước khi LoadScene vào tòa án.
    /// Khi tòa xử xong (Win/Lose), CourtTrialController dùng TimeManager.LastExplorationScene
    /// để quay về đúng scene, nhưng static đó mất khi tắt game → cần persist vào file.
    ///
    /// Cách gọi (từ GameManager hoặc nơi trigger LoadScene vào CourtScene):
    ///   if (SaveLoadManager.Instance != null)
    ///       SaveLoadManager.Instance.SaveLastExplorationScene(TimeManager.LastExplorationScene);
    /// </summary>
    public void SaveLastExplorationScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;

        SaveData data = _currentSaveData ?? new SaveData();
        data.LastExplorationScene = sceneName;
        _currentSaveData = data;
        SaveSystem.Instance.SaveData(data);
    }

    /// <summary>
    /// UC15 — Lưu lịch phiên tòa sau khi người chơi chọn nghi phạm và bấm Convict.
    /// Ghi xuống disk ngay vì đây là sự kiện quan trọng, không thể để mất.
    /// </summary>
    public void SaveCourtSchedule(string suspectId, int courtDay)
    {
        SaveData data = _currentSaveData ?? new SaveData();

        data.HasScheduledCourtTrial = true;
        data.ScheduledSuspectId = suspectId;
        data.ScheduledCourtDay = courtDay;

        _currentSaveData = data;
        SaveSystem.Instance.SaveData(data);
    }

    /// <summary>
    /// Req 4 — Gọi khi phiên tòa kết thúc và nghi phạm bị kết tội chính thức.
    /// Khác với SaveCourtSchedule (đang lên lịch) — đây là kết quả cuối cùng.
    /// Tự động xóa khỏi pending schedule nếu đây là người vừa xét xử xong.
    /// Ghi xuống disk ngay.
    ///
    /// Cách gọi từ CourtTrialController.OnTrialWin():
    ///   if (SaveLoadManager.Instance != null && ConvictedSuspect != null)
    ///       SaveLoadManager.Instance.SaveConviction(ConvictedSuspect.NpcId);
    /// </summary>
    public void SaveConviction(string suspectId)
    {
        if (string.IsNullOrEmpty(suspectId)) return;

        SaveData data = _currentSaveData ?? new SaveData();

        if (!data.ConvictedSuspectIds.Contains(suspectId))
            data.ConvictedSuspectIds.Add(suspectId);

        // Xóa pending schedule nếu đây là người vừa bị xét xử
        if (data.ScheduledSuspectId == suspectId)
        {
            data.HasScheduledCourtTrial = false;
            data.ScheduledSuspectId = string.Empty;
            data.ScheduledCourtDay = 0;
        }

        _currentSaveData = data;
        SaveSystem.Instance.SaveData(data);
    }
}