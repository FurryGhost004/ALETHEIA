// Path: _Project/Scripts/SaveLoad/SaveLoadManager.cs

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
/// - (Load dùng nội bộ bởi GameManager khi Continue Case)
/// </summary>
public class SaveLoadManager : SingletonBase<SaveLoadManager>
{
    // ── Private Fields ───────────────────────────
    // Giữ lại SaveData hiện tại trong RAM để SaveCourtSchedule() có thể
    // cập nhật 1 phần mà không cần load lại toàn bộ file từ disk.
    private SaveData _currentSaveData;

    // ── Public Methods ───────────────────────────

    /// <summary>
    /// UC13 Pause Menu (Save Case): Gom toàn bộ trạng thái game hiện tại
    /// thành SaveData, rồi ghi xuống disk qua SaveSystem.
    ///
    /// TODO: Phần gom dữ liệu (vị trí player, evidence, ngày, thời gian...)
    /// sẽ được điền đầy đủ khi Investigation System (2.2), Keyword System (2.3)
    /// và Time System (2.5) của Võ Thái Thịnh đã có — vì SaveLoadManager cần
    /// đọc dữ liệu từ PlayerController, EvidenceEngine, TimeManager.
    /// Hiện tại chỉ lưu được những gì đã có sẵn trong _currentSaveData.
    /// </summary>
    public void SaveGame()
    {
        SaveData data = _currentSaveData ?? new SaveData();

        // TODO: data.PlayerPosition = PlayerController.Instance.transform.position;
        // TODO: data.CurrentDay = TimeManager.Instance.CurrentDay;
        // TODO: data.TimeRemaining = TimeManager.Instance.TimeRemaining;
        // TODO: data.CollectedEvidenceIds = NotebookData.Instance.GetEvidenceIds();
        // TODO: data.CollectedKeywordIds = NotebookData.Instance.GetKeywordIds();

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
}
