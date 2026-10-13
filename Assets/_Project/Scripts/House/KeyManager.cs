using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance { get; private set; }

    // Dùng HashSet để tìm kiếm chìa khóa với độ phức tạp O(1) và tránh lưu trùng keyId
    private readonly HashSet<string> _collectedKeys = new HashSet<string>();

    private void Awake()
    {
        // Khởi tạo Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ KeyManager tồn tại khi chuyển Scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Kiểm tra xem người chơi đã sở hữu chìa khóa này chưa (Khớp với LockedChest.cs)
    /// </summary>
    public bool HasKey(string keyId)
    {
        if (string.IsNullOrEmpty(keyId)) return false;

        bool hasKey = _collectedKeys.Contains(keyId);
        Debug.Log($"<color=cyan>[KEY MANAGER]</color> Kiểm tra chìa <b>{keyId}</b>: {(hasKey ? "<color=green>CÓ CHÌA</color>" : "<color=red>CHƯA CÓ</color>")}");

        return hasKey;
    }

    /// <summary>
    /// Thêm chìa khóa vào danh sách khi người chơi nhặt được (Khớp với KeyItem.cs)
    /// </summary>
    public void AddKey(string keyId)
    {
        if (string.IsNullOrEmpty(keyId)) return;

        if (_collectedKeys.Add(keyId))
        {
            Debug.Log($"<color=green>[KEY MANAGER]</color> Đã thêm chìa khóa mới vào túi: <b>{keyId}</b>");
        }
        else
        {
            Debug.LogWarning($"<color=yellow>[KEY MANAGER]</color> Chìa khóa <b>{keyId}</b> đã có trong túi từ trước.");
        }
    }

    /// <summary>
    /// (Tùy chọn) Xóa chìa khóa nếu loại chìa khóa đó chỉ xài 1 lần rồi mất
    /// </summary>
    public bool UseKey(string keyId)
    {
        if (_collectedKeys.Remove(keyId))
        {
            Debug.Log($"<color=orange>[KEY MANAGER]</color> Đã sử dụng và xóa chìa khóa: <b>{keyId}</b>");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Xóa toàn bộ chìa khóa khi Reset game hoặc chơi lại
    /// </summary>
    public void ClearAllKeys()
    {
        _collectedKeys.Clear();
        Debug.Log("<color=red>[KEY MANAGER]</color> Đã xóa toàn bộ chìa khóa trong túi.");
    }
}