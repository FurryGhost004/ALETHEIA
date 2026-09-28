using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TopicUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CourtTrialController _trialController;
    [SerializeField] private Transform _topicButtonContainer; // Object chứa nút (thường cài Vertical/Grid Layout Group)
    [SerializeField] private GameObject _topicButtonPrefab;    // Prefab PF_TopicButton

    private void OnEnable()
    {
        GenerateTopicButtons();
    }

    public void GenerateTopicButtons()
    {
        // 1. Kiểm tra an toàn Reference trong Inspector
        if (_topicButtonContainer == null || _topicButtonPrefab == null)
        {
            Debug.LogWarning("[TopicUIController] Chưa gán _topicButtonContainer hoặc _topicButtonPrefab trong Inspector!");
            return;
        }

        // 2. Xóa các nút cũ trước khi sinh nút mới
        foreach (Transform child in _topicButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // 3. Kiểm tra dữ liệu Nghi phạm
        SuspectData suspect = CourtTrialController.ConvictedSuspect;
        if (suspect == null || suspect.TrialTopics == null || suspect.TrialTopics.Count == 0)
        {
            Debug.LogWarning("[TopicUIController] ConvictedSuspect bằng null hoặc không có Topic nào trong danh sách!");
            return;
        }

        // 4. Tạo button cho từng Topic
        foreach (var topic in suspect.TrialTopics)
        {
            if (topic == null) continue;

            GameObject btnObj = Instantiate(_topicButtonPrefab, _topicButtonContainer);

            // Gán chữ hiển thị trên nút
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = topic.topicName;
            }

            // Gán sự kiện Click
            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                {
                    if (_trialController != null)
                    {
                        _trialController.OnTopicSelected(topic);
                    }
                });
            }
        }
    }
}