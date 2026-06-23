// Path: _Project/Scripts/Utils/GameConstants.cs

/// <summary>
/// Hằng số dùng chung toàn project — KHÔNG hardcode số/string trực tiếp trong các script khác.
/// Mọi giá trị cố định liên quan đến gameplay phải khai báo ở đây.
/// Khi cần điều chỉnh cân bằng gameplay (ví dụ tăng/giảm số lần hoãn phiên tòa)
/// chỉ cần sửa 1 chỗ duy nhất ở file này.
/// </summary>
public static class GameConstants
{
    // ── Time System ──────────────────────────────
    public const float DAY_DURATION = 300f;          // 1 ngày trong game = 300 giây thực

    // ── Court Trial System ───────────────────────
    public const int MAX_POSTPONE_COUNT = 2;          // Tối đa 2 lần hoãn phiên tòa
    public const int MAX_REPUTATION = 100;            // Danh tiếng tối đa
    public const int MIN_REPUTATION = 0;              // Danh tiếng tối thiểu (0 = trigger Postpone)

    // ── Scene Names ───────────────────────────────
    public const string SCENE_MAIN_MENU = "MainMenu";
    public const string SCENE_INVESTIGATION = "Investigation";
    public const string SCENE_COURT_TRIAL = "CourtTrial";

    // ── Save System ───────────────────────────────
    public const string SAVE_FILE_NAME = "savegame.json";
}
