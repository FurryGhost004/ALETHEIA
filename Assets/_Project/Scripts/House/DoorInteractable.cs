using System.Collections;
using UnityEngine;

public class DoorInteractable : Interactable
{
    [SerializeField] private float _openAngle = GameConstants.DOOR_OPEN_ANGLE;
    [SerializeField] private float _openSpeed = GameConstants.DOOR_OPEN_SPEED;

    private bool _isOpen;
    private bool _isRotating;
    private Quaternion _closedRotation;
    private Quaternion _openRotation;

    private void Awake()
    {
        _closedRotation = transform.localRotation;
        _openRotation = _closedRotation * Quaternion.Euler(0f, _openAngle, 0f);
    }

    public override void Interact()
    {
        if (_isRotating) return;

        _isOpen = !_isOpen;
        Quaternion targetRotation = _isOpen ? _openRotation : _closedRotation;
        StartCoroutine(RotateDoorRoutine(targetRotation));
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