using SojaExiles;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerInteraction _interaction;
    [SerializeField] private PlayerLook _playerLook;
    [SerializeField] private NotebookHubUI _notebookHub; // Đổi sang NotebookHubUI

    private PlayerInputActions _input;
    private bool _isInterrogating;
    private bool _isTalkingToNPC = false;

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

    public void SetInterrogating(bool isInterrogating)
    {
        _isInterrogating = isInterrogating;
        _isTalkingToNPC = isInterrogating;

        // 1. Quản lý di chuyển của Player (Dùng _movement đúng với khai báo)
        if (_movement != null)
        {
            _movement.SetCanMove(!isInterrogating);
            if (isInterrogating)
            {
                // Triệt tiêu ngay lập tức hướng di chuyển cũ nếu đang bấm WASD lúc kích hoạt thoại
                _movement.SetMoveInput(Vector2.zero);
            }
        }

        // 2. Quản lý xoay camera & con trỏ chuột
        if (_playerLook != null)
        {
            // Mở khóa chuột để click UI khi isInterrogating = true
            _playerLook.SetCursorLocked(!isInterrogating);

            if (isInterrogating)
            {
                // Cố định góc nhìn camera, tránh xoay khi di chuột vào UI
                _playerLook.SetLookInput(Vector2.zero);
            }
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (_isInterrogating) return;
        if (_movement != null) _movement.SetMoveInput(context.ReadValue<Vector2>());
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        if (_isInterrogating)
        {
            if (_playerLook != null) _playerLook.SetLookInput(Vector2.zero);
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
        bool isNotebookOpen = _notebookHub != null && _notebookHub.gameObject.activeSelf;

        // Chặn mở Notebook khi một UI khác đang khóa Player,
        // nhưng vẫn cho phép Q đóng chính Notebook đang mở.
        if (_isInterrogating && !isNotebookOpen)
            return;

        if (_notebookHub != null)
        {
            if (PauseManager.Instance != null)
            {
                PauseManager.Instance.ToggleNotebookFromGameplay();
            }
            else
            {
                _notebookHub.ToggleHub();
            }
        }
    }
}
