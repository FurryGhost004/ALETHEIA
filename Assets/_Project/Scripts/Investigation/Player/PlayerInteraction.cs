using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _interactDistance = 3f;
    [SerializeField] private LayerMask _interactionLayer;
    private void Awake()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }
    }

    // Hàm gọi khi nhấn nút Interact (gán phím E trong Input Action)
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryInteract();
        }
    }
    public void TryInteract()
    {
        Ray ray = new Ray(
            _camera.transform.position,
            _camera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _interactDistance, _interactionLayer))
        {
            if (hit.collider.TryGetComponent(out Interactable interactable))
            {
                interactable.Interact();
            }
        }
    }
}