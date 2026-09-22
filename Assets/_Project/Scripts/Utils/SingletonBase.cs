using UnityEngine;

/// <summary>
/// Base class Singleton an toàn cho Unity 6.
/// Hỗ trợ tìm kiếm GameObject bị ẩn (Inactive) và ngăn tự sinh GameObject rác làm mất UI Inspector.
/// </summary>
/// <typeparam name="T">Type của Manager kế thừa</typeparam>
public abstract class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    /// <summary>
    /// Cho biết Singleton này có giữ lại khi chuyển Scene hay không.
    /// Mặc định = true. Nếu là UI Manager thuộc Scene cụ thể, hãy override = false.
    /// </summary>
    protected virtual bool IsPersistent => true;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // Tìm kể cả các GameObject Manager đang bị Disable/Hide trên Hierarchy
                _instance = Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);

                if (_instance == null)
                {
                    Debug.LogError($"[SingletonBase] KHÔNG tìm thấy Manager '{typeof(T).Name}' trên Scene! " +
                                   $"Nếu đây là UI Manager, hãy chắc chắn bạn đã kéo Prefab/GameObject của nó vào Hierarchy.");
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;

            // Chỉ DontDestroyOnLoad nếu là Root Object và IsPersistent = true
            if (IsPersistent && transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}