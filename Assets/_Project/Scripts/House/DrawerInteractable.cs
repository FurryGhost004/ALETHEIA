using System.Collections;
using UnityEngine;

public class DrawerInteractable : Interactable
{
    public enum DrawerSlideDirection
    {
        LocalForward,   // Trục +Z (Hướng phía trước của ngăn kéo)
        LocalBackward,  // Trục -Z (Hướng phía sau)
        LocalRight,     // Trục +X (Hướng sang phải)
        LocalLeft,      // Trục -X (Hướng sang trái)
        LocalUp,        // Trục +Y (Hướng lên trên)
        LocalDown       // Trục -Y (Hướng xuống dưới)
    }

    [Header("Drawer Settings")]
    [SerializeField] private float _openDistance = 0.5f;
    [SerializeField] private float _openSpeed = 3f;
    [SerializeField] private DrawerSlideDirection _slideDirection = DrawerSlideDirection.LocalForward;

    private bool _isOpen;
    private bool _isMoving;
    private Vector3 _closedPosition;
    private Vector3 _openPosition;

    private void Awake()
    {
        _closedPosition = transform.localPosition;
        Vector3 slideAxis = GetDirectionVector(_slideDirection);
        _openPosition = _closedPosition + slideAxis * _openDistance;
    }

    public override void Interact()
    {
        if (_isMoving) return;

        _isOpen = !_isOpen;
        Vector3 targetPosition = _isOpen ? _openPosition : _closedPosition;
        StartCoroutine(SlideDrawerRoutine(targetPosition));
    }

    private Vector3 GetDirectionVector(DrawerSlideDirection direction)
    {
        return direction switch
        {
            DrawerSlideDirection.LocalForward => Vector3.forward,
            DrawerSlideDirection.LocalBackward => Vector3.back,
            DrawerSlideDirection.LocalRight => Vector3.right,
            DrawerSlideDirection.LocalLeft => Vector3.left,
            DrawerSlideDirection.LocalUp => Vector3.up,
            DrawerSlideDirection.LocalDown => Vector3.down,
            _ => Vector3.forward
        };
    }

    private IEnumerator SlideDrawerRoutine(Vector3 targetPosition)
    {
        _isMoving = true;

        while (Vector3.Distance(transform.localPosition, targetPosition) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetPosition,
                Time.deltaTime * _openSpeed
            );
            yield return null;
        }

        transform.localPosition = targetPosition;
        _isMoving = false;
    }
}