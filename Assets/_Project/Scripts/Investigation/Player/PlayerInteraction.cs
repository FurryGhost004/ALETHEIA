using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _interactDistance = 3f;
    [SerializeField] private LayerMask _interactionLayer;
    [SerializeField] private LayerMask _obstacleLayer; // Layer của tường hoặc vật cản cần chặn

    private void Awake()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }
    }

    private void Update()
    {
        // Bấm phím E để tương tác nếu sự kiện Input Action chưa gán ở Unity Events
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryInteract();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryInteract();
        }
    }

    public void TryInteract()
    {
        if (_camera == null) _camera = Camera.main;

        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

        // 1. Kiểm tra xem tia nhìn có chạm vào vật thể tương tác trong tầm với không
        if (Physics.Raycast(ray, out RaycastHit hit, _interactDistance, _interactionLayer))
        {
            // 2. KIỂM TRA VẬT CẢN (CHỐNG XUYÊN TƯỜNG):
            // Tính khoảng cách và hướng từ Camera đến vị trí điểm va chạm của NPC
            Vector3 directionToTarget = hit.point - _camera.transform.position;
            float distanceToTarget = directionToTarget.magnitude;

            // Bắn một tia ray kiểm tra xem có vật cản (như tường, kính) chắn ngang giữa đường không
            if (Physics.Raycast(_camera.transform.position, directionToTarget.normalized, out RaycastHit obstacleHit, distanceToTarget, _obstacleLayer))
            {
                // Nếu có vật cản nằm chắn giữa camera và NPC -> Dừng lại, không cho tương tác
                return;
            }

            // 3. Nếu không bị tường chắn, tiến hành gọi lệnh tương tác
            if (hit.collider.TryGetComponent(out Interactable interactable))
            {
                interactable.Interact();
            }
        }
    }
}