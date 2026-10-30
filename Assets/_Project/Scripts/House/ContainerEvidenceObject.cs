using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ContainerEvidenceObject : Interactable
{
    [Header("Lock Settings")]
    [SerializeField] private bool _isLocked = false;
    [SerializeField] private string _requiredKeyId = "ChestKey_01";
    [SerializeField] private DialogueDatabase _lockedDialogue;

    [Header("Container Configuration")]
    [SerializeField] private KeywordData _boxKeywordData;
    [SerializeField] private DialogueDatabase _boxDialogueDatabase;

    [Header("Inspect Settings")]
    [SerializeField] private float _moveDuration = GameConstants.DEFAULT_MOVE_DURATION;
    [SerializeField] private float _distanceFromCamera = GameConstants.DEFAULT_CAMERA_DISTANCE;
    [SerializeField] private float _rotateSensitivity = 5f;

    [Header("References")]
    [SerializeField] private PlayerLook _playerLook;

    private bool _isInteracted;
    private bool _isUnlocked;
    private Collider _collider;
    private Camera _mainCamera;
    private EvidenceObject[] _childItems;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _mainCamera = Camera.main;

        if (_playerLook == null)
        {
            _playerLook = Object.FindFirstObjectByType<PlayerLook>();
        }

        _childItems = GetComponentsInChildren<EvidenceObject>(true);
    }

    public override void Interact()
    {
        if (_isInteracted) return;

        // 1. Kiểm tra khóa (nếu rương cần chìa khóa)
        if (_isLocked && !_isUnlocked)
        {
            if (KeyManager.Instance != null && KeyManager.Instance.HasKey(_requiredKeyId))
            {
                _isUnlocked = true;
                Debug.Log($"<color=green>[Container]</color> Đã mở khóa hộp bằng chìa: {_requiredKeyId}");
            }
            else
            {
                if (_lockedDialogue != null && DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.StartDialogue(_lockedDialogue);
                }
                else
                {
                    Debug.Log($"<color=yellow>[Container]</color> Hộp đang bị khóa! Cần chìa khóa: {_requiredKeyId}");
                }
                return;
            }
        }

        _isInteracted = true;

        if (_collider != null)
        {
            _collider.enabled = false;
        }

        StartCoroutine(InspectContainerRoutine());
    }

    private IEnumerator InspectContainerRoutine()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        Transform mainCamTransform = _mainCamera.transform;

        // 2. Khóa xoay camera, mở con trỏ chuột tự do
        if (_playerLook != null)
        {
            _playerLook.SetCursorLocked(false);
            _playerLook.ResetPitchSmooth(_moveDuration);
        }

        // Phát hội thoại/suy nghĩ mở hộp
        if (_boxDialogueDatabase != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(_boxDialogueDatabase);
        }

        Vector3 centerOffset = GetLocalCenterOffset();

        // GIAI ĐOẠN 1: Hộp bay mượt mà ra giữa màn hình trước mặt camera
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

        // Mở khóa keyword của chiếc hộp
        if (_boxKeywordData != null && KeywordManager.Instance != null)
        {
            KeywordManager.Instance.UnlockKeyword(_boxKeywordData);
        }

        // GIAI ĐOẠN 2: Vòng lặp khi hộp đang ở giữa màn hình
        bool isInspecting = true;
        while (isInspecting)
        {
            // Cập nhật danh sách vật phẩm con bên trong
            _childItems = GetComponentsInChildren<EvidenceObject>();

            if (Mouse.current != null)
            {
                // Click chuột trái: Bắn Raycast để tương tác với Nắp hộp hoặc Vật phẩm bên trong
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    TryClickInteraction();
                }

                // Giữ chuột phải: Xoay hộp quan sát các góc độ
                if (Mouse.current.rightButton.isPressed)
                {
                    Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                    float rotX = mouseDelta.x * _rotateSensitivity;
                    float rotY = mouseDelta.y * _rotateSensitivity;

                    transform.Rotate(mainCamTransform.up, -rotX, Space.World);
                    transform.Rotate(mainCamTransform.right, rotY, Space.World);
                }
            }

            // Bấm phím E hoặc Space để cất hộp đi (chỉ cho phép thoát NẾU ĐÃ NHẬT HẾT ĐỒ BÊN TRONG)
            if (Keyboard.current != null)
            {
                bool isSpacePressed = Keyboard.current.spaceKey.wasPressedThisFrame;
                bool isEPressed = Keyboard.current.eKey.wasPressedThisFrame;

                if (isSpacePressed || isEPressed)
                {
                    if (_childItems.Length == 0)
                    {
                        isInspecting = false; // Đã nhặt hết đồ -> Thoát vòng lặp
                    }
                    else
                    {
                        // Nhắc nhở người chơi vẫn còn đồ trong hộp chưa nhặt hết
                        if (ThinkingFeatureUI.Instance != null)
                        {
                            ThinkingFeatureUI.Instance.ShowThinking("Vẫn còn vật phẩm bên trong hộp chưa thu thập...");
                        }
                        else
                        {
                            Debug.Log("[Container] Vẫn còn vật phẩm bên trong hộp chưa thu thập hết.");
                        }
                    }
                }
            }

            // Giữ vị trí hộp cố định trước mặt camera trong lúc soi
            Vector3 targetCenterPos = mainCamTransform.position + mainCamTransform.forward * _distanceFromCamera;
            transform.position = targetCenterPos - transform.TransformVector(centerOffset);

            yield return null;
        }

        // GIAI ĐOẠN 3: Người chơi đã bấm E/Space sau khi nhặt hết đồ -> Khóa lại chuột và xóa chiếc hộp
        if (_playerLook != null)
        {
            _playerLook.SetCursorLocked(true);
        }

        Debug.Log($"[ContainerEvidenceObject] Đã cất hộp thành công: {gameObject.name}");
        Destroy(gameObject);
    }

    private void TryClickInteraction()
    {
        if (_mainCamera == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = _mainCamera.ScreenPointToRay(mousePos);

        Debug.DrawRay(ray.origin, ray.direction * 50f, Color.green, 2f);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, ~0, QueryTriggerInteraction.Collide))
        {
            Interactable interactable = hit.collider.GetComponentInParent<Interactable>();
            if (interactable != null)
            {
                if (interactable == this) return;

                // Nếu click trúng nắp hộp (shelfbox_2) hoặc script mở nắp
                if (hit.collider.gameObject.name.Contains("shelfbox_2") || hit.collider.GetComponent<FlapDoorInteractable>() != null)
                {
                    interactable.Interact();
                    SetChildItemsActive(true);
                    return;
                }

                // Nếu click trúng vật phẩm bên trong
                EvidenceObject item = hit.collider.GetComponentInParent<EvidenceObject>();
                if (item != null)
                {
                    item.CollectItem();
                }
            }
        }
    }

    private void SetChildItemsActive(bool isActive)
    {
        EvidenceObject[] allChildren = GetComponentsInChildren<EvidenceObject>(true);
        foreach (var item in allChildren)
        {
            if (item != null && item.gameObject != gameObject)
            {
                item.gameObject.SetActive(isActive);
            }
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