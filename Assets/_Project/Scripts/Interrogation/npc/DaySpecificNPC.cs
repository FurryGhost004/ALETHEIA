using UnityEngine;
using UnityEngine.AI;

public class DaySpecificNPC : MonoBehaviour
{
    [Header("SO Data Config")]
    [SerializeField] private NPCEventData _eventData;

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
            _renderers = GetComponentsInChildren<Renderer>();

        if (_colliders == null || _colliders.Length == 0)
            _colliders = GetComponentsInChildren<Collider>();
    }

    private void Start()
    {
        TrySubscribe();

        if (TimeManager.Instance != null)
        {
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
        }
    }

    private void EvaluateVisibility(int currentDay)
    {
        if (_eventData == null) return;

        bool shouldBeActive = _eventData.ShouldBeActive(currentDay);

        if (shouldBeActive)
        {
            UpdatePositionForDay(currentDay);
        }

        SetNPCVisibility(shouldBeActive);
    }

    private void UpdatePositionForDay(int currentDay)
    {
        if (_eventData.GetLocationForDay(currentDay, out Vector3 targetPos, out Quaternion targetRot))
        {
            // Tắt tạm thời Agent/CharacterController để dịch chuyển vị trí không bị lỗi physics
            if (_navAgent != null) _navAgent.enabled = false;
            if (_characterController != null) _characterController.enabled = false;

            transform.position = targetPos;
            transform.rotation = targetRot;

            if (_navAgent != null) _navAgent.enabled = true;
            if (_characterController != null) _characterController.enabled = true;
        }
    }

    private void SetNPCVisibility(bool isVisible)
    {
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