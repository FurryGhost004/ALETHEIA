using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerInteraction _interaction;
    [SerializeField] private PlayerLook _playerLook;
    [SerializeField] private NotebookToggleUI _notebookToggle;

    private PlayerInputActions _input;
    private bool _isInterrogating;

    private void Awake()
    {
        _input = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _input.Enable();

        _input.Player.Move.performed += OnMove;
        _input.Player.Move.canceled += OnMove;

        _input.Player.Look.performed += OnLook;
        _input.Player.Look.canceled += OnLook;

        _input.Player.Interact.performed += OnInteract;

        _input.Player.ToggleCursor.performed += OnCursorPressed;
        _input.Player.ToggleCursor.canceled += OnCursorReleased;

        _input.Player.ToggleNotebook.performed += OnToggleNotebook;
    }

    private void OnDisable()
    {
        _input.Player.Move.performed -= OnMove;
        _input.Player.Move.canceled -= OnMove;

        _input.Player.Look.performed -= OnLook;
        _input.Player.Look.canceled -= OnLook;

        _input.Player.Interact.performed -= OnInteract;

        _input.Player.ToggleCursor.performed -= OnCursorPressed;
        _input.Player.ToggleCursor.canceled -= OnCursorReleased;

        _input.Player.ToggleNotebook.performed -= OnToggleNotebook;

        _input.Disable();
    }

    // --- HÀM BẬT / TẮT TRẠNG THÁI THẨM VẤN ---
    public void SetInterrogating(bool state)
    {
        _isInterrogating = state;

        if (_movement != null)
        {
            _movement.SetCanMove(!state); // Bật/Tắt di chuyển
        }

        if (_playerLook != null)
        {
            // Mở con trỏ chuột khi đang thẩm vấn (state = true), khóa chuột lại khi xong (state = false)
            _playerLook.SetCursorLocked(!state);
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (_isInterrogating) return; // Không nhận di chuyển khi đang thẩm vấn
        if (_movement != null) _movement.SetMoveInput(context.ReadValue<Vector2>());
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        if (_isInterrogating)
        {
            if (_playerLook != null) _playerLook.SetLookInput(Vector2.zero); // Ngừng xoay góc nhìn
            return;
        }
        if (_playerLook != null) _playerLook.SetLookInput(context.ReadValue<Vector2>());
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (_isInterrogating) return;
        if (_interaction != null) _interaction.TryInteract();
    }

    private void OnCursorPressed(InputAction.CallbackContext context)
    {
        if (_isInterrogating) return;
        if (_playerLook != null) _playerLook.SetCursorLocked(false);
    }

    private void OnCursorReleased(InputAction.CallbackContext context)
    {
        if (_isInterrogating) return;
        if (_playerLook != null) _playerLook.SetCursorLocked(true);
    }

    private void OnToggleNotebook(InputAction.CallbackContext context)
    {
        if (_isInterrogating) return;
        if (_notebookToggle != null) _notebookToggle.ToggleNotebook();
    }
}