using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _gravity = -9.81f;

    private CharacterController _controller;
    private Vector2 _moveInput;
    private float _verticalVelocity;

    // Biến kiểm tra cho phép di chuyển
    private bool _canMove = true;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public void SetMoveInput(Vector2 input)
    {
        _moveInput = input;
    }

    // Hàm cho phép kích hoạt / vô hiệu hóa di chuyển từ bên ngoài
    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
        if (!_canMove)
        {
            _moveInput = Vector2.zero; // Xóa input cũ khi bị khóa
        }
    }

    private void Update()
    {
        ApplyGravity();

        // Nếu không cho phép di chuyển, chỉ giữ lại vận tốc trọng lực
        Vector3 move = Vector3.zero;
        if (_canMove)
        {
            move = (transform.right * _moveInput.x + transform.forward * _moveInput.y) * _moveSpeed;
        }

        move.y = _verticalVelocity;

        _controller.Move(move * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (_controller.isGrounded)
        {
            _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity += _gravity * Time.deltaTime;
        }
    }
}