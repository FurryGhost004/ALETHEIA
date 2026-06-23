// Path: _Project/Scripts/Utils/ExtensionMethods.cs

using UnityEngine;

/// <summary>
/// Các extension method tiện ích dùng chung toàn project.
/// Thêm dần vào đây khi cả nhóm cần 1 hàm tiện ích lặp lại ở nhiều script
/// (ví dụ: format thời gian, kiểm tra string rỗng, clamp giá trị...).
/// KHÔNG copy-paste cùng 1 đoạn logic tiện ích vào nhiều script khác nhau.
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Format giây thành dạng "mm:ss" — dùng cho HUDManager hiển thị Time bar.
    /// Ví dụ: 125f → "02:05"
    /// </summary>
    public static string ToTimeString(this float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int remainingSeconds = Mathf.FloorToInt(seconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, remainingSeconds);
    }

    /// <summary>
    /// Kiểm tra string null hoặc rỗng — dùng khi xử lý keyword/evidence ID.
    /// Ví dụ: keywordInput.IsNullOrEmpty()
    /// </summary>
    public static bool IsNullOrEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Giới hạn giá trị Reputation trong khoảng MIN_REPUTATION đến MAX_REPUTATION.
    /// Dùng trong ReputationManager khi penaltyRep() hoặc restoreReputation().
    /// </summary>
    public static int ClampReputation(this int value)
    {
        return Mathf.Clamp(value, GameConstants.MIN_REPUTATION, GameConstants.MAX_REPUTATION);
    }

    /// <summary>
    /// Set alpha cho CanvasGroup — dùng khi fade UI (Notebook, Pause Menu, Dialogue).
    /// Ví dụ: canvasGroup.SetAlpha(0f) để ẩn UI mà không cần SetActive(false).
    /// </summary>
    public static void SetAlpha(this CanvasGroup canvasGroup, float alpha)
    {
        canvasGroup.alpha = alpha;
        canvasGroup.interactable = alpha > 0f;
        canvasGroup.blocksRaycasts = alpha > 0f;
    }
}
