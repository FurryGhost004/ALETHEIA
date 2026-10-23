// Path: _Project/Scripts/SaveLoad/SaveLoadManager.cs

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tầng CAO của Save/Load — gom dữ liệu game thật (vị trí, evidence, ngày...)
/// thành 1 SaveData, rồi giao cho SaveSystem ghi xuống disk. Đây là nơi
/// PauseMenuUI và GameManager gọi vào để Save/Load — không gọi trực tiếp
/// SaveSystem từ UI.
///
/// Theo Sequence Diagram:
/// - Pause Menu (Save Case):    PauseMenuUI → SaveGame()
/// - Convict Suspect (UC15):    GameManager → SaveCourtSchedule()
/// - Convicted (UC15 result):   GameManager → SaveConviction()
/// - (Load dùng nội bộ bởi GameManager khi Continue Case)
/// </summary>
public class SaveLoadManager : SingletonBase<SaveLoadManager>
{
    // ── Private Fields ───────────────────────────
    // Giữ lại SaveData hiện tại trong RAM để các method partial update
    // có thể cập nhật 1 phần mà không cần load lại toàn bộ file từ disk.
    private SaveData _currentSaveData;

    // ── Public Methods ───────────────────────────

    /// <summary>
    /// UC13 Pause Menu (Save Case): Gom toàn bộ trạng thái game hiện tại
    /// thành SaveData, rồi ghi xuống disk qua SaveSystem.
    /// </summary>
    public void SaveGame()
    {
        SaveData data = _currentSaveData ?? new SaveData();

        // ── Req 1: Thời gian ──────────────────────────────────────────────
        // TODO: data.CurrentDay       = TimeManager.Instance.CurrentDay;
        // TODO: data.TimeRemaining    = TimeManager.Instance.TimeRemaining;

        // ── Vị trí player ────────────────────────────────────────────────
        // TODO: data.PlayerPosition   = PlayerController.Instance.transform.position;

        // ── Req 2: Vật phẩm đã bị destroy ───────────────────────────────
        // EvidenceObject.EvidenceId cần được thêm vào list này mỗi khi Destroy() gọi.
        // SaveGame() KHÔNG tự quét scene — EvidenceObject tự gọi NotifyDestroyed()
        // khi bị phá hủy để cập nhật _currentSaveData.DestroyedEvidenceIds.
        // (Xem method NotifyEvidenceDestroyed bên dưới)
        // data.DestroyedEvidenceIds đã được cập nhật realtime — không cần gom lại ở đây.

        // ── Req 3: Keywords đã mở khóa ──────────────────────────────────
        // TODO (chờ Thịnh thêm _allKeywords catalog vào KeywordManager):
        // data.CollectedKeywordIds = new List<string>();
        // foreach (var kw in KeywordManager.Instance.UnlockedKeywords)
        //     if (!string.IsNullOrEmpty(kw.Id)) data.CollectedKeywordIds.Add(kw.Id);

        // ── Notebook: bằng chứng đã thu thập ────────────────────────────
        // TODO: data.CollectedEvidenceIds = NotebookData.Instance.GetEvidenceIds();

        // ── Req 4: Danh sách nhân chứng / vật chứng / nghi phạm ──────────
        // TODO (cần CaseBoard/SuspectManager file):
        // data.UnlockedSuspectIds          = CaseBoardManager.Instance.GetUnlockedSuspectIds();
        // data.UnlockedWitnessIds          = CaseBoardManager.Instance.GetUnlockedWitnessIds();
        // data.UnlockedPhysicalEvidenceIds = CaseBoardManager.Instance.GetUnlockedPhysicalEvidenceIds();

        // ── Req 6: Notes trong Notebook ──────────────────────────────────
        // TODO (cần NotebookData.cs file):
        // data.NotebookNoteIds = NotebookData.Instance.GetNoteIds();

        _currentSaveData = data;
        SaveSystem.Instance.SaveData(data);
    }

    /// <summary>
    /// UC03 Continue Case: Đọc file save từ disk qua SaveSystem, trả về SaveData
    /// để GameManager dùng cho SceneLoader.RestoreState().
    /// </summary>
    public SaveData LoadGame()
    {
        _currentSaveData = SaveSystem.Instance.ReadSaveFile();
        return _currentSaveData;
    }

    /// <summary>
    /// Req 2 — Được gọi bởi EvidenceObject ngay trước Destroy(gameObject).
    /// Cập nhật realtime thay vì đợi đến lúc SaveGame() quét lại scene.
    ///
    /// Cách dùng trong EvidenceObject (thêm vào cuối InspectRoutine, trước Destroy):
    ///   if (SaveLoadManager.Instance != null)
    ///       SaveLoadManager.Instance.NotifyEvidenceDestroyed(_evidenceId);
    /// </summary>
    public void NotifyEvidenceDestroyed(string evidenceId)
    {
        if (string.IsNullOrEmpty(evidenceId)) return;

        SaveData data = _currentSaveData ?? new SaveData();
        if (!data.DestroyedEvidenceIds.Contains(evidenceId))
        {
            data.DestroyedEvidenceIds.Add(evidenceId);
        }

        _currentSaveData = data;
    }

    /// <summary>
    /// UC15 Convict Suspect: Lưu lịch phiên tòa sau khi người chơi kết tội nghi phạm.
    /// Cập nhật phần lịch phiên tòa trong SaveData hiện tại rồi ghi xuống disk.
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
    /// Req 5 — Gọi khi phiên tòa kết thúc và nghi phạm bị kết tội chính thức.
    /// Khác với SaveCourtSchedule (đang lên lịch) — đây là kết quả cuối cùng.
    /// Ghi xuống disk ngay lập tức vì đây là sự kiện không thể đảo ngược.
    /// </summary>
    public void SaveConviction(string suspectId)
    {
        if (string.IsNullOrEmpty(suspectId)) return;

        SaveData data = _currentSaveData ?? new SaveData();
        if (!data.ConvictedSuspectIds.Contains(suspectId))
        {
            data.ConvictedSuspectIds.Add(suspectId);
        }

        // Xóa khỏi pending schedule nếu đây là suspect vừa bị xét xử xong
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