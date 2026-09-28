// Path: _Project/Scripts/Settings/SettingsPanelUI.cs

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Đồng bộ hiển thị 3 Slider trong Settings Panel với giá trị hiện tại của SettingsManager.
/// Chạy mỗi khi Panel này được bật (OnEnable), đảm bảo Slider luôn hiện đúng giá trị đã lưu.
/// </summary>
public class SettingsPanelUI : MonoBehaviour
{
    [Header("Sliders (kéo từ SettingsPanel vào)")]
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;
    [SerializeField] private Slider _mouseSensitivitySlider;

    private void OnEnable()
    {
        if (SettingsManager.Instance == null) return;

        // Dùng SetValueWithoutNotify để KHÔNG kích hoạt lại OnValueChanged
        // (tránh gọi lại SetMasterVolume/SetSfxVolume/SetMouseSensitivity một cách thừa thãi)
        _masterVolumeSlider.SetValueWithoutNotify(SettingsManager.Instance.MasterVolume);
        _sfxVolumeSlider.SetValueWithoutNotify(SettingsManager.Instance.SfxVolume);
        _mouseSensitivitySlider.SetValueWithoutNotify(SettingsManager.Instance.MouseSensitivity01);
    }
}