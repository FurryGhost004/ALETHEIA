using System.Collections;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private float _mouseSensitivity = 150f;

    private float _pitch;
    private Vector2 _lookInput;
    private bool _isCursorLocked = true;
    private bool _canLook = true; // Biến kiểm tra quyền xoay camera
    private Coroutine _resetPitchCoroutine;

    private void Start()
    {
        SetCursorState(true);
    }

    private void Update()
    {
        // Nếu chuột không bị khóa HOẶC đang bị cấm xoay (đang hội thoại) thì dừng tính toán xoay camera
        if (!_isCursorLocked || !_canLook)
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

    // --- HÀM BẬT/TẮT QUYỀN XOAY CAMERA (Dùng cho DialogueManager) ---
    public void SetCanLook(bool canLook)
    {
        _canLook = canLook;
        if (!_canLook)
        {
            _lookInput = Vector2.zero; // Xóa input cũ để tránh camera bị trôi hướng khi bật lại
        }
    }

    public void SetCursorLocked(bool isLocked)
    {
        _isCursorLocked = isLocked;

        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }

    private void SetCursorState(bool isLocked)
    {
        _isCursorLocked = isLocked;

        Cursor.lockState = isLocked
            ? CursorLockMode.Locked
            : CursorLockMode.None;

        Cursor.visible = !isLocked;

        Debug.Log(
            $"[PlayerLook] Cursor State: Locked={isLocked}, Visible={Cursor.visible}\n" +
            $"STACK TRACE:\n{System.Environment.StackTrace}"
        );
    }

    // Xoay góc nhìn Camera về nằm ngang (Pitch = 0) mượt mà
    public void ResetPitchSmooth(float duration)
    {
        if (_resetPitchCoroutine != null)
        {
            StopCoroutine(_resetPitchCoroutine);
        }
        _resetPitchCoroutine = StartCoroutine(ResetPitchRoutine(duration));
    }

    private IEnumerator ResetPitchRoutine(float duration)
    {
        float startPitch = _pitch;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);

            _pitch = Mathf.Lerp(startPitch, 0f, t);
            _cameraHolder.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

            yield return null;
        }

        _pitch = 0f;
        _cameraHolder.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }
}