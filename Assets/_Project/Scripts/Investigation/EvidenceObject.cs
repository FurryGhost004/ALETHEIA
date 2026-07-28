using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class EvidenceObject : Interactable
{
    [Header("Inspect Settings")]
    [SerializeField] private float _moveDuration = GameConstants.DEFAULT_MOVE_DURATION;
    [SerializeField] private float _distanceFromCamera = GameConstants.DEFAULT_CAMERA_DISTANCE;
    [SerializeField] private float _rotateSensitivity = GameConstants.DEFAULT_ROTATE_SENSITIVITY;

    [Header("References")]
    [SerializeField] private PlayerLook _playerLook;

    private bool _isInteracted;
    private Collider _collider;
    private Camera _mainCamera;

    private void Awake()
    {
        _collider = GetComponent<Collider>();

        if (_playerLook == null)
        {
            _playerLook = Object.FindFirstObjectByType<PlayerLook>();
        }

        _mainCamera = Camera.main;
    }

    private void Start()
    {
    }

    public override void Interact()
    {
        if (_isInteracted) return;
        _isInteracted = true;

        if (_collider != null)
        {
            _collider.enabled = false;
        }

        StartCoroutine(InspectRoutine());
    }

    private IEnumerator InspectRoutine()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        Transform mainCamTransform = _mainCamera.transform;

        // 1. Tắt xoay Camera & Mở con trỏ chuột
        if (_playerLook != null)
        {
            _playerLook.SetCursorLocked(false);
            _playerLook.ResetPitchSmooth(_moveDuration);
        }

        Vector3 centerOffset = GetLocalCenterOffset();

        // GIAI ĐOẠN 1: Bay mượt mà lên trước mặt
        float elapsedTime = 0f;
        while (elapsedTime < _moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / _moveDuration);

            Vector3 targetCenterPos = mainCamTransform.position + mainCamTransform.forward * _distanceFromCamera;
            Vector3 targetPivotPos = targetCenterPos - transform.TransformVector(centerOffset);

            transform.position = Vector3.Lerp(transform.position, targetPivotPos, t);
            yield return null;
        }

        // GIAI ĐOẠN 2: Xoay đồ vật & Click kiểm tra Hotspot
        bool isInspecting = true;
        while (isInspecting)
        {
            if (Mouse.current != null)
            {
                // Bắn Raycast kiểm tra Hotspot khi nhấp chuột trái
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    CheckClickHotspot();
                }

                // Kéo giữ chuột trái để xoay vật thể
                if (Mouse.current.leftButton.isPressed)
                {
                    Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                    float rotX = mouseDelta.x * _rotateSensitivity;
                    float rotY = mouseDelta.y * _rotateSensitivity;

                    transform.Rotate(mainCamTransform.up, -rotX, Space.World);
                    transform.Rotate(mainCamTransform.right, rotY, Space.World);
                }
            }

            Vector3 targetCenterPos = mainCamTransform.position + mainCamTransform.forward * _distanceFromCamera;
            transform.position = targetCenterPos - transform.TransformVector(centerOffset);

            if (Keyboard.current != null)
            {
                bool isSpacePressed = Keyboard.current.spaceKey.wasPressedThisFrame;
                bool isEPressed = Keyboard.current.eKey.wasPressedThisFrame;

                if (isSpacePressed || isEPressed)
                {
                    isInspecting = false;
                }
            }

            yield return null;
        }

        // GIAI ĐOẠN 3: Khóa lại con trỏ chuột & Hoàn tất kiểm tra
        if (_playerLook != null)
        {
            _playerLook.SetCursorLocked(true);
        }

        Debug.Log($"{GameConstants.LOG_EVIDENCE_COLLECTED}{gameObject.name}");
        Destroy(gameObject);
    }

    private void CheckClickHotspot()
    {
        if (_mainCamera == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = _mainCamera.ScreenPointToRay(mousePos);

        // Vẽ tia ray trong cửa sổ Scene (màu đỏ trong 2 giây) để debug
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 2f);

        // Bắn Raycast nhận diện cả Trigger Collider
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
        {
            InspectionHotspot hotspot = hit.collider.GetComponent<InspectionHotspot>();
            if (hotspot != null)
            {
                hotspot.OnDiscovered();
            }
            else
            {
                Debug.Log($"[Inspect] Click trúng: '{hit.collider.name}' nhưng GameObject này KHÔNG CÓ script InspectionHotspot.");
            }
        }
        else
        {
            Debug.Log("[Inspect] Raycast không bắn trúng bất kỳ Collider nào trên màn hình.");
        }
    }

    private Vector3 GetLocalCenterOffset()
    {
        Renderer objectRenderer = GetComponentInChildren<Renderer>();
        if (objectRenderer != null)
        {
            return transform.InverseTransformPoint(objectRenderer.bounds.center);
        }

        if (_collider != null)
        {
            return transform.InverseTransformPoint(_collider.bounds.center);
        }

        return Vector3.zero;
    }
}