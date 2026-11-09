using TMPro;
using UnityEngine;

public class ResultUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _txtTotalDays;
    [SerializeField] private TextMeshProUGUI _txtTotalEvidences;
    [SerializeField] private TextMeshProUGUI _txtTotalMistakes;
    [SerializeField] private TextMeshProUGUI _txtFinalScore;

    [Header("Score Configuration")]
    [SerializeField] private int _baseScore = 100;
    [SerializeField] private int _penaltyPerDay = 5;
    [SerializeField] private int _penaltyPerMistake = 10;

    private void Start()
    {
        DisplayResultData();
    }

    public void DisplayResultData()
    {
        // 1. Lấy dữ liệu thực tế
        int totalDays = (TimeManager.Instance != null) ? TimeManager.Instance.CurrentDay : 5;
        int totalEvidences = (PlayerInventory.Instance != null) ? PlayerInventory.Instance.CollectedEvidences.Count : 7;

        // Giả sử bạn lấy số lần lỗi từ CourtTrialController hoặc lưu chung ở một Manager nào đó
        int totalMistakes = 8;

        // 2. Tính toán điểm thô
        int rawScore = _baseScore - (totalDays * _penaltyPerDay) - (totalMistakes * _penaltyPerMistake);

        // 3. CHẶN ÂM: Đảm bảo điểm thấp nhất là 0, không bao giờ bị số âm
        int finalScore = Mathf.Max(0, rawScore);

        // 4. Đưa lên giao diện UI
        if (_txtTotalDays != null) _txtTotalDays.text = $"Total days spend: {totalDays}";
        if (_txtTotalEvidences != null) _txtTotalEvidences.text = $"Total evident found: {totalEvidences}";
        if (_txtTotalMistakes != null) _txtTotalMistakes.text = $"Total Mistake: {totalMistakes}";
        if (_txtFinalScore != null)
        {
            _txtFinalScore.text = $"Final Score: {finalScore}";

            // Tùy chọn: Nếu điểm về 0 thì đổi màu chữ thành đỏ cho kịch tính
            if (finalScore == 0)
            {
                _txtFinalScore.color = Color.red;
            }
        }
    }
}