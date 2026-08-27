using System.Collections;
using TMPro;
using UnityEngine;

public class ThinkingFeatureUI : MonoBehaviour
{
    public static ThinkingFeatureUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject _panelThinking;
    [SerializeField] private TextMeshProUGUI _txtDialogueContent;

    [Header("Settings")]
    [SerializeField] private float _autoHideDuration = 3f; // Tự động ẩn sau 3 giây (nếu không ấn Space)

    private Coroutine _hideCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (_panelThinking == null) _panelThinking = gameObject;
        _panelThinking.SetActive(false);
    }

    public void ShowThinking(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        if (_txtDialogueContent != null)
        {
            _txtDialogueContent.text = message;
        }

        _panelThinking.SetActive(true);

        if (_hideCoroutine != null)
        {
            StopCoroutine(_hideCoroutine);
        }

        _hideCoroutine = StartCoroutine(AutoHideRoutine());
    }

    public void HideThinking()
    {
        _panelThinking.SetActive(false);
    }

    private IEnumerator AutoHideRoutine()
    {
        yield return new WaitForSeconds(_autoHideDuration);
        HideThinking();
    }
}