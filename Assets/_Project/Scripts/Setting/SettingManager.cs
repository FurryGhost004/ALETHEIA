// Path: _Project/Scripts/Settings/SettingsManager.cs

using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Quản lý toàn bộ Settings (Master Volume, SFX Volume, Mouse Sensitivity).
/// Persistent xuyên Scene (kế thừa SingletonBase), lưu/đọc bằng PlayerPrefs.
/// UI Slider gọi thẳng vào các method Set... của class này qua OnValueChanged.
/// </summary>
public class SettingsManager : SingletonBase<SettingsManager>
{
    [Header("Audio Mixer")]
    [Tooltip("Kéo asset MasterMixer.mixer vào đây. Dùng để set MasterVolume / SFXVolume bằng dB.")]
    [SerializeField] private AudioMixer _audioMixer;

    [Header("Mouse Sensitivity Range")]
    [Tooltip("Sensitivity thực tế khi Slider = 0.")]
    [SerializeField] private float _minSensitivity = 50f;
    [Tooltip("Sensitivity thực tế khi Slider = 1.")]
    [SerializeField] private float _maxSensitivity = 300f;

    [Header("Default Values (dùng khi chưa có PlayerPrefs)")]
    [Range(0f, 1f)][SerializeField] private float _defaultMasterVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float _defaultSfxVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float _defaultMouseSensitivity = 0.5f;

    private const string MIXER_PARAM_MASTER = "MasterVolume";
    private const string MIXER_PARAM_SFX = "SFXVolume";
    private const float MIN_DB = -80f; // dB khi Slider gần 0, coi như mute

    // ── Public Properties (đọc giá trị hiện tại, dùng để hiển thị lại lên Slider) ──
    public float MasterVolume { get; private set; }
    public float SfxVolume { get; private set; }
    public float MouseSensitivity01 { get; private set; } // giá trị 0-1 để hiển thị lên Slider

    protected override void Awake()
    {
        base.Awake();

        if (Instance != this) return;

        LoadAllSettings();
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        ReapplyMouseSensitivityToCurrentScene();
    }

    // ── Load lúc game khởi động ───────────────────
    private void LoadAllSettings()
    {
        float savedMaster = PlayerPrefs.GetFloat(GameConstants.PREF_MASTER_VOLUME, _defaultMasterVolume);
        float savedSfx = PlayerPrefs.GetFloat(GameConstants.PREF_SFX_VOLUME, _defaultSfxVolume);
        float savedSens = PlayerPrefs.GetFloat(GameConstants.PREF_MOUSE_SENSITIVITY, _defaultMouseSensitivity);

        SetMasterVolume(savedMaster);
        SetSfxVolume(savedSfx);
        SetMouseSensitivity(savedSens);
    }

    // ── API gọi trực tiếp từ Slider (OnValueChanged) ──

    /// <summary>Gọi từ Slider "VOLUME". value trong khoảng 0-1.</summary>
    public void SetMasterVolume(float value)
    {
        MasterVolume = value;
        ApplyMixerVolume(MIXER_PARAM_MASTER, value);
        PlayerPrefs.SetFloat(GameConstants.PREF_MASTER_VOLUME, value);
    }

    /// <summary>Gọi từ Slider "SFX". value trong khoảng 0-1.</summary>
    public void SetSfxVolume(float value)
    {
        SfxVolume = value;
        ApplyMixerVolume(MIXER_PARAM_SFX, value);
        PlayerPrefs.SetFloat(GameConstants.PREF_SFX_VOLUME, value);
    }

    /// <summary>Gọi từ Slider "MOUSE SEN". value trong khoảng 0-1.</summary>
    public void SetMouseSensitivity(float value01)
    {
        MouseSensitivity01 = value01;

        float actualSensitivity = Mathf.Lerp(_minSensitivity, _maxSensitivity, value01);

        PlayerLook playerLook = FindFirstObjectByType<PlayerLook>();
        if (playerLook != null)
        {
            playerLook.SetSensitivity(actualSensitivity);
        }
        // Không log warning nếu null — vì ở MainMenu/CaseSelect sẽ không có PlayerLook trong Scene,
        // đây là chuyện bình thường, không phải lỗi.

        PlayerPrefs.SetFloat(GameConstants.PREF_MOUSE_SENSITIVITY, value01);
    }

    // ── Helper: convert Slider(0-1) → dB, xử lý mute an toàn ──
    private void ApplyMixerVolume(string mixerParam, float sliderValue01)
    {
        float dB = sliderValue01 <= 0.0001f
            ? MIN_DB
            : Mathf.Log10(sliderValue01) * 20f;

        _audioMixer.SetFloat(mixerParam, dB);
    }

    /// <summary>
    /// Gọi hàm này khi Scene mới load xong (ví dụ trong Start của UI Settings)
    /// để áp lại Mouse Sensitivity cho PlayerLook của Scene mới,
    /// vì FindFirstObjectByType chỉ tìm được PlayerLook đang tồn tại trong Scene hiện tại.
    /// </summary>
    public void ReapplyMouseSensitivityToCurrentScene()
    {
        SetMouseSensitivity(MouseSensitivity01);
    }
}