// Path: _Project/Scripts/Utils/SingletonBase.cs

using UnityEngine;

/// <summary>
/// Base class Singleton dùng chung cho toàn bộ Manager trong game
/// (GameManager, TimeManager, SaveSystem, CrossExamManager...).
/// Kế thừa class này để tự động có Instance, không cần viết lại logic Singleton mỗi lần.
/// </summary>
/// <typeparam name="T">Type của Manager kế thừa, ví dụ: GameManager : SingletonBase&lt;GameManager&gt;</typeparam>
public abstract class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
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
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
