// Path: _Project/Scripts/Audio/SFXEntry.cs

using UnityEngine;

/// <summary>
/// Một entry SFX trong danh sách của AudioManager.
/// Team chỉ cần thêm entry mới trong Inspector, không cần sửa code.
/// </summary>
[System.Serializable]
public class SFXEntry
{
    [Tooltip("ID dùng để gọi âm thanh này trong code. Ví dụ: ButtonClick, DoorOpen.")]
    public string Id;

    [Tooltip("File âm thanh tương ứng với ID này.")]
    public AudioClip Clip;

    [Range(0f, 1f)]
    [Tooltip("Âm lượng riêng của SFX này (nhân thêm với SFX Volume trong Settings).")]
    public float Volume = 1f;

    [Range(0.5f, 2f)]
    [Tooltip("Cao độ (pitch) của SFX. Để 1 là bình thường.")]
    public float Pitch = 1f;
}