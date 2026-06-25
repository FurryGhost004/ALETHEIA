// Path: _Project/Scripts/SaveLoad/SaveSystem.cs

using System.IO;
using UnityEngine;

/// <summary>
/// Tầng THẤP NHẤT của Save/Load — chỉ lo việc đọc/ghi file thật trên disk.
/// KHÔNG biết gì về logic game (vị trí player, evidence...) — chỉ nhận
/// vào/trả ra đúng 1 đối tượng SaveData.
///
/// Theo Sequence Diagram:
/// - Main Menu (UC03 Continue Case):   GameManager → ReadSaveFile()
/// - Main Menu (UC05 Quit Game):       GameManager → AutoSave()
/// - Pause Menu (Return to Main Menu): GameManager → AutoSave()
///
/// AutoSave() KHÔNG tự gom data — nó gọi lên SaveLoadManager.SaveGame()
/// để gom dữ liệu game thật rồi ghi xuống. Việc gọi qua lại giữa 2 class
/// này không gây lỗi circular reference trong C# vì cùng 1 assembly.
/// </summary>
public class SaveSystem : SingletonBase<SaveSystem>
{
    // ── Private Fields ───────────────────────────
    private string SavePath => Path.Combine(Application.persistentDataPath, GameConstants.SAVE_FILE_NAME);

    // ── Public Methods ───────────────────────────

    /// <summary>UC03: Đọc file save từ disk, deserialize ra SaveData. Trả về null nếu chưa có file.</summary>
    public SaveData ReadSaveFile()
    {
        if (!File.Exists(SavePath))
        {
            return null;
        }

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    /// <summary>Serialize SaveData ra JSON và ghi xuống disk. Được SaveLoadManager gọi sau khi gom data.</summary>
    public void SaveData(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    /// <summary>
    /// UC05/UC13: Tự động lưu khi Quit Game hoặc Return to Main Menu.
    /// Không tự gom data — giao lại cho SaveLoadManager xử lý phần gom + ghi.
    /// </summary>
    public void AutoSave()
    {
        SaveLoadManager.Instance.SaveGame();
    }
}
