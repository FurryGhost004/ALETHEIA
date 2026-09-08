using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _gravity = -9.81f;

    private CharacterController _controller;
    private Vector2 _moveInput;
    private float _verticalVelocity;

    // Biến kiểm tra quyền di chuyển
    private bool _canMove = true;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public void SetMoveInput(Vector2 input)
    {
        _moveInput = input;
    }

    // Hàm Bật/Tắt di chuyển từ UI
    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
        if (!_canMove)
        {
            _moveInput = Vector2.zero; // Xóa input cũ để tránh nhân vật bị trôi
        }
    }

    private void Update()
    {
        ApplyGravity();

        Vector3 move = Vector3.zero;

        // Chỉ tính toán hướng đi nếu được phép di chuyển
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