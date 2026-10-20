using UnityEngine;
using UnityEngine.AI;

public class DaySpecificNPC : MonoBehaviour
{
    [Header("SO Data Config")]
    [SerializeField] private NPCEventData _eventData;

    [Header("Default Fallback Position (Dùng khi không có sự kiện / hết ngày)")]
    [SerializeField] private bool _useDefaultPositionWhenNoEvent = true;
    private Vector3 _sceneDefaultPosition;
    private Quaternion _sceneDefaultRotation;

    [Header("Components to Toggle")]
    [SerializeField] private Renderer[] _renderers;
    [SerializeField] private Collider[] _colliders;

    private NavMeshAgent _navAgent;
    private CharacterController _characterController;
    private bool _isSubscribed = false;

    private void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _characterController = GetComponent<CharacterController>();

        if (_renderers == null || _renderers.Length == 0)
            _renderers = GetComponentsInChildren<Renderer>(true);

        if (_colliders == null || _colliders.Length == 0)
            _colliders = GetComponentsInChildren<Collider>();

        // Lưu lại vị trí và rotation mặc định ban đầu của NPC đặt sẵn trong Scene
        _sceneDefaultPosition = transform.position;
        _sceneDefaultRotation = transform.rotation;
    }

    private void Start()
    {
        TrySubscribe();

        if (TimeManager.Instance != null)
        {
            Debug.Log($"[{gameObject.name}] Start kiểm tra ngày hiện tại: Day {TimeManager.Instance.CurrentDay}");
            EvaluateVisibility(TimeManager.Instance.CurrentDay);
        }
    }

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        if (TimeManager.Instance != null && _isSubscribed)
        {
            TimeManager.Instance.OnDayAdvanced -= EvaluateVisibility;
            _isSubscribed = false;
        }
    }

    private void TrySubscribe()
    {
        if (!_isSubscribed && TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDayAdvanced += EvaluateVisibility;
            _isSubscribed = true;
            Debug.Log($"[{gameObject.name}] Đã đăng ký lắng nghe sự kiện chuyển ngày từ TimeManager.");
        }
    }

    private void EvaluateVisibility(int currentDay)
    {
        if (_eventData == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Chưa gán NPCEventData!");
            SetNPCVisibility(true);
            return;
        }

        bool shouldBeActive = _eventData.ShouldBeActive(currentDay);
        Debug.Log($"[{gameObject.name}] EvaluateVisibility - Ngày hiện tại: {currentDay} | ShouldBeActive: {shouldBeActive}");

        if (shouldBeActive)
        {
            // Lấy vị trí theo ngày (nếu có vị trí riêng thì dùng, nếu không sẽ lấy defaultPosition của SO)
            bool hasSpecificLoc = _eventData.GetLocationForDay(currentDay, out Vector3 targetPos, out Quaternion targetRot);
            Debug.Log($"[{gameObject.name}] Kích hoạt event! Vị trí đích: {targetPos} (Có vị trí riêng: {hasSpecificLoc})");

            TeleportToPosition(targetPos, targetRot);
            SetNPCVisibility(true);
        }
        else
        {
            if (_useDefaultPositionWhenNoEvent)
            {
                Debug.Log($"[{gameObject.name}] Không có sự kiện hôm nay, quay về vị trí mặc định trong Scene.");
                TeleportToPosition(_sceneDefaultPosition, _sceneDefaultRotation);
                SetNPCVisibility(true);
            }
            else
            {
                Debug.Log($"[{gameObject.name}] Không có sự kiện hôm nay, ẩn NPC đi.");
                SetNPCVisibility(false);
            }
        }
    }

    private void TeleportToPosition(Vector3 targetPos, Quaternion targetRot)
    {
        if (_characterController != null) _characterController.enabled = false;

        if (_navAgent != null)
        {
            _navAgent.enabled = false;
            transform.position = targetPos;
            transform.rotation = targetRot;
            _navAgent.enabled = true;
            _navAgent.Warp(targetPos); // Bắt buộc dùng Warp cho NavMeshAgent để tránh lỗi định vị
        }
        else
        {
            transform.position = targetPos;
            transform.rotation = targetRot;
        }

        if (_characterController != null) _characterController.enabled = true;
    }

    private void SetNPCVisibility(bool isVisible)
    {
        Debug.Log($"[{gameObject.name}] SetNPCVisibility -> {isVisible}");
        foreach (var r in _renderers)
        {
            if (r != null) r.enabled = isVisible;
        }

        foreach (var c in _colliders)
        {
            if (c != null) c.enabled = isVisible;
        }
    }
}