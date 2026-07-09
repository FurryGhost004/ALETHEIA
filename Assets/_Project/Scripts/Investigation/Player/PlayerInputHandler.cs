using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerInteraction _interaction;
    [SerializeField] private PlayerLook _playerLook;

    private PlayerInputActions _input;

    private void Awake()
    {
        _input = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _input.Enable();

        _input.Player.Move.performed += OnMove;
        _input.Player.Move.canceled += OnMove;

        _input.Player.Interact.performed += OnInteract;

        _input.Player.Look.performed += OnLook;
        _input.Player.Look.canceled += OnLook;
    }

    private void OnDisable()
    {
        _input.Player.Move.performed -= OnMove;
        _input.Player.Move.canceled -= OnMove;

        _input.Player.Interact.performed -= OnInteract;

        _input.Disable();

        _input.Player.Look.performed -= OnLook;
        _input.Player.Look.canceled -= OnLook;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _movement.SetMoveInput(context.ReadValue<Vector2>());
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        _interaction.TryInteract();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        _playerLook.SetLookInput(context.ReadValue<Vector2>());
    }
}