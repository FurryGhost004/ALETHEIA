// Path: _Project/Scripts/Core/GameManager.cs

using UnityEngine;

/// <summary>
/// Quản lý game state tổng thể — đây là script cả nhóm phụ thuộc vào,
/// nên phải hoàn thành SỚM trước khi các thành viên khác attach script vào scene.
///
/// Theo Sequence Diagram (Main Menu): loadScene(), initCaseData(), goToMainMenu()
/// Theo Sequence Diagram (Time System): triggerGameOver() khi hết lượt hoãn phiên tòa
/// </summary>
public class GameManager : SingletonBase<GameManager>
{
    // ── Serialized Fields ────────────────────────
    [SerializeField] private GameState _currentState = GameState.MainMenu;

    // ── Public Properties ────────────────────────
    public GameState CurrentState => _currentState;

    // ── Public Methods ───────────────────────────

    /// <summary>UC01/UC02: Bắt đầu vụ án mới — load scene Investigation.</summary>
    public void LoadScene(string caseId)
    {
        SceneLoader.Instance.LoadScene(caseId);
    }

    /// <summary>
    /// UC01: Khởi tạo dữ liệu vụ án (nghi phạm, bằng chứng, từ khóa, môi trường).
    /// TODO: Sẽ gọi EvidenceEngine.LoadCaseData() và NPCDatabase.LoadCaseData()
    /// khi 2 hệ thống đó được Võ Thái Thịnh xây dựng xong ở task 2.3/2.4.
    /// </summary>
    public void InitCaseData(string caseId)
    {
        ChangeState(GameState.Investigation);
    }

    /// <summary>UC05/UC13: Quay về Main Menu — dùng trong Quit Game và Pause Menu.</summary>
    public void GoToMainMenu()
    {
        ChangeState(GameState.MainMenu);
        SceneLoader.Instance.LoadScene(GameConstants.SCENE_MAIN_MENU);
    }

    /// <summary>UC15: Kích hoạt phiên tòa sau khi Convict Suspect.</summary>
    public void StartCourtTrial()
    {
        ChangeState(GameState.CourtTrial);
        SceneLoader.Instance.LoadScene(GameConstants.SCENE_COURT_TRIAL);
    }

    /// <summary>UC18: Kích hoạt Game Over khi hết lượt hoãn phiên tòa (lần thứ 3).</summary>
    public void TriggerGameOver()
    {
        ChangeState(GameState.GameOver);
    }

    // ── Private Methods ──────────────────────────
    private void ChangeState(GameState newState)
    {
        _currentState = newState;
        EventBus.Publish(new GameStateChangedEvent(newState));
    }
}
