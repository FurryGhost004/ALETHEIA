using System.Collections.Generic;
using UnityEngine;

public class KeyManager : SingletonBase<KeyManager>
{
    private HashSet<string> _collectedKeys = new HashSet<string>();

    public void AddKey(string keyId)
    {
        if (string.IsNullOrEmpty(keyId)) return;

        if (!_collectedKeys.Contains(keyId))
        {
            _collectedKeys.Add(keyId);
            Debug.Log($"[KeyManager] Đã nhặt chìa khóa: {keyId}");
        }
    }

    public bool HasKey(string keyId)
    {
        if (string.IsNullOrEmpty(keyId)) return false;
        return _collectedKeys.Contains(keyId);
    }
}