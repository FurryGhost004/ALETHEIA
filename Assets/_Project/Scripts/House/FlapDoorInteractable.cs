using System.Collections;
using UnityEngine;

public class FlapDoorInteractable : Interactable
{
    public enum FlapDirection
    {
        OpenDownwards, // Gập xuống dưới (Cửa lò nướng, cửa lò vi sóng)
        OpenUpwards,   // Gập ngược lên trên (Nắp bồn cầu, nắp rương, nắp thùng rác)
        CustomAxis     // Trục xoay tự chỉnh
    }

    [Header("Flap Settings")]
    [SerializeField] private float _openAngle = 90f;
    [SerializeField] private float _openSpeed = 3f;
    [SerializeField] private FlapDirection _flapDirection = FlapDirection.OpenDownwards;

    [Header("Custom Settings (If FlapDirection = CustomAxis)")]
    [SerializeField] private Vector3 _customRotationAxis = Vector3.right;

    private bool _isOpen;
    private bool _isRotating;
    private Quaternion _closedRotation;
    private Quaternion _openRotation;

    private void Awake()
    {
        _closedRotation = transform.localRotation;
        Vector3 axis = GetRotationAxis();
        _openRotation = _closedRotation * Quaternion.Euler(axis * _openAngle);
    }

    private void Start()
    {
    }

    public override void Interact()
    {
        if (_isRotating) return;

        _isOpen = !_isOpen;
        Quaternion targetRotation = _isOpen ? _openRotation : _closedRotation;
        StartCoroutine(RotateFlapRoutine(targetRotation));
    }

    private Vector3 GetRotationAxis()
    {
        return _flapDirection switch
        {
            FlapDirection.OpenDownwards => Vector3.right,  // Trục +X (Gập xuống)
            FlapDirection.OpenUpwards => Vector3.left,     // Trục -X (Gập lên)
            FlapDirection.CustomAxis => _customRotationAxis.normalized,
            _ => Vector3.right
        };
    }

    private IEnumerator RotateFlapRoutine(Quaternion targetRotation)
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