using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private float _mouseSensitivity = 150f;

    private float _pitch;
    private Vector2 _lookInput;
    private bool _isCursorLocked = true;

    private void Start()
    {
        SetCursorState(true);
    }

    private void Update()
    {
        if (!_isCursorLocked)
        {
            return;
        }

        float mouseX = _lookInput.x * _mouseSensitivity * Time.deltaTime;
        float mouseY = _lookInput.y * _mouseSensitivity * Time.deltaTime;

        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, -80f, 80f);

        _cameraHolder.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    public void SetLookInput(Vector2 input)
    {
        _lookInput = input;
    }

    public void SetCursorLocked(bool isLocked)
    {
        SetCursorState(isLocked);
    }

    private void SetCursorState(bool isLocked)
    {
        _isCursorLocked = isLocked;

        Cursor.lockState = isLocked
            ? CursorLockMode.Locked
            : CursorLockMode.None;

        Cursor.visible = !isLocked;
    }
}