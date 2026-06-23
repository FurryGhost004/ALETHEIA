// Path: _Project/Scripts/Core/GameStateEnum.cs

/// <summary>
/// Enum định nghĩa các trạng thái chính của game.
/// Dùng trong GameManager để quản lý luồng chuyển đổi giữa các giai đoạn.
/// </summary>
public enum GameState
{
    MainMenu,
    Investigation,
    CourtTrial,
    GameOver
}
