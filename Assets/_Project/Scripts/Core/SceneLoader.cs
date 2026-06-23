// Path: _Project/Scripts/Core/SceneLoader.cs

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Load scene bất đồng bộ (async) để tránh freeze màn hình khi chuyển cảnh —
/// đặc biệt quan trọng với Unity 6 + URP vì scene có nhiều asset nặng.
/// Theo Sequence Diagram (Main Menu): loadScene(caseId) → sceneReady() → sceneRestored()
/// </summary>
public class SceneLoader : SingletonBase<SceneLoader>
{
    // ── Private Fields ───────────────────────────
    private string _currentCaseId;

    // ── Public Methods ───────────────────────────
    public void LoadScene(string caseId)
    {
        _currentCaseId = caseId;
        StartCoroutine(LoadSceneRoutine(caseId));
    }

    // ── Private Methods ──────────────────────────
    private IEnumerator LoadSceneRoutine(string caseId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(caseId);

        while (!operation.isDone)
        {
            yield return null;
        }

        NotifySceneReady();
    }

    private void NotifySceneReady()
    {
        EventBus.Publish(new SceneReadyEvent(_currentCaseId));
    }

    public void NotifySceneRestored()
    {
        EventBus.Publish(new SceneRestoredEvent(_currentCaseId));
    }
}
