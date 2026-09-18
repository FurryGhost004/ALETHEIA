using System.Collections;
using UnityEngine;

public class DoorInteractable : Interactable
{
    public enum DoorOpenDirection
    {
        DynamicAwayFromPlayer, // Tự động đẩy cửa ra xa vị trí người chơi
        FixedLeft,             // Cố định xoay mở sang trái
        FixedRight             // Cố định xoay mở sang phải
    }

    [Header("Door Settings")]
    [SerializeField] private float _openAngle = GameConstants.DOOR_OPEN_ANGLE;
    [SerializeField] private float _openSpeed = GameConstants.DOOR_OPEN_SPEED;
    [SerializeField] private DoorOpenDirection _openDirection = DoorOpenDirection.DynamicAwayFromPlayer;

    private bool _isOpen;
    private bool _isRotating;
    private Quaternion _closedRotation;
    private Quaternion _targetOpenRotation;
    private Camera _mainCamera;

    private void Awake()
    {
        _closedRotation = transform.localRotation;
        _mainCamera = Camera.main;
    }

    public override void Interact()
    {
        if (_isRotating) return;

        _isOpen = !_isOpen;

        if (_isOpen)
        {
            _targetOpenRotation = CalculateOpenRotation();
        }

        Quaternion targetRotation = _isOpen ? _targetOpenRotation : _closedRotation;
        StartCoroutine(RotateDoorRoutine(targetRotation));
    }

    private Quaternion CalculateOpenRotation()
    {
        float finalAngle = Mathf.Abs(_openAngle);

        switch (_openDirection)
        {
            case DoorOpenDirection.FixedLeft:
                finalAngle = -Mathf.Abs(_openAngle);
                break;

            case DoorOpenDirection.FixedRight:
                finalAngle = Mathf.Abs(_openAngle);
                break;

            case DoorOpenDirection.DynamicAwayFromPlayer:
                if (_mainCamera == null)
                {
                    _mainCamera = Camera.main;
                }

                if (_mainCamera != null)
                {
                    Vector3 doorToPlayer = _mainCamera.transform.position - transform.position;
                    float dot = Vector3.Dot(transform.forward, doorToPlayer);

                    // Nếu người chơi ở mặt trước cửa (dot >= 0), đẩy cửa lùi ra sau (-angle)
                    // Nếu người chơi ở mặt sau cửa (dot < 0), đẩy cửa tiến ra trước (+angle)
                    finalAngle = dot >= 0f ? -Mathf.Abs(_openAngle) : Mathf.Abs(_openAngle);
                }
                break;
        }

        return _closedRotation * Quaternion.Euler(0f, finalAngle, 0f);
    }

    private IEnumerator RotateDoorRoutine(Quaternion targetRotation)
    {
        _isRotating = true;

        while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.01f)
        {
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                targetRotation,
                Time.deltaTime * _openSpeed
            );
            yield return null;
        }

        transform.localRotation = targetRotation;
        _isRotating = false;
    }
}