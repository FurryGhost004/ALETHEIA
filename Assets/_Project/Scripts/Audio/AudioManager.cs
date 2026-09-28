// Path: _Project/Scripts/Audio/AudioManager.cs

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// AudioManager trung tâm cho toàn bộ game — persistent xuyên Scene (kế thừa SingletonBase).
/// Mọi script khác chỉ cần gọi:
///   AudioManager.Instance.PlaySFX("ButtonClick");                     → âm thanh UI/2D
///   AudioManager.Instance.PlaySFXAtPosition("DoorOpen", pos);         → âm thanh World/3D
/// Không cần biết AudioClip nằm ở đâu, không cần biết AudioMixer hoạt động thế nào.
/// </summary>
public class AudioManager : SingletonBase<AudioManager>
{
    [Header("Audio Mixer")]
    [Tooltip("Mixer Group 'SFX' bên trong MasterMixer. Mọi SFX (2D lẫn 3D) đều output ra group này.")]
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;

    [Header("SFX Database")]
    [Tooltip("Danh sách toàn bộ SFX trong game. Kéo AudioClip vào đây, không sửa code khi thêm âm thanh mới.")]
    [SerializeField] private List<SFXEntry> _sfxList = new List<SFXEntry>();

    [Header("2D SFX Sources (UI / Global)")]
    [Tooltip("Số lượng AudioSource dùng chung cho SFX 2D. 3-4 là đủ để các SFX gần nhau không cắt tiếng nhau.")]
    [SerializeField] private int _sfx2DSourceCount = 4;

    private Dictionary<string, SFXEntry> _sfxDict;
    private AudioSource[] _sfx2DSources;
    private int _next2DSourceIndex;

    protected override void Awake()
    {
        base.Awake(); // SingletonBase xử lý Instance + DontDestroyOnLoad + chống duplicate

        // Nếu Awake này chạy trên object bị Destroy do duplicate thì dừng luôn, không setup gì thêm
        if (Instance != this) return;

        BuildSfxDictionary();
        CreateTwoDAudioSources();
    }

    // ── Setup nội bộ ──────────────────────────────
    private void BuildSfxDictionary()
    {
        _sfxDict = new Dictionary<string, SFXEntry>();

        foreach (SFXEntry entry in _sfxList)
        {
            if (string.IsNullOrEmpty(entry.Id))
            {
                Debug.LogWarning("[AudioManager] Có 1 SFX Entry bị thiếu ID, đã bỏ qua.");
                continue;
            }

            if (_sfxDict.ContainsKey(entry.Id))
            {
                Debug.LogWarning($"[AudioManager] SFX ID trùng lặp: \"{entry.Id}\". Entry sau sẽ bị bỏ qua.");
                continue;
            }

            _sfxDict.Add(entry.Id, entry);
        }
    }

    private void CreateTwoDAudioSources()
    {
        _sfx2DSources = new AudioSource[Mathf.Max(1, _sfx2DSourceCount)];

        for (int i = 0; i < _sfx2DSources.Length; i++)
        {
            GameObject sourceGO = new GameObject($"SFX2D_Source_{i}");
            sourceGO.transform.SetParent(transform);

            AudioSource source = sourceGO.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f; // 2D
            source.outputAudioMixerGroup = _sfxMixerGroup;

            _sfx2DSources[i] = source;
        }
    }

    // ── API công khai ─────────────────────────────

    /// <summary>Phát SFX 2D (UI/Global) — dùng cho Button, Panel, Notebook, Save/Load...</summary>
    public void PlaySFX(string id)
    {
        if (!TryGetEntry(id, out SFXEntry entry)) return;

        AudioSource source = GetNextTwoDSource();
        source.pitch = entry.Pitch;
        source.PlayOneShot(entry.Clip, entry.Volume);
    }

    /// <summary>Phát SFX 3D tại một vị trí trong world — dùng cho Door, Object, Evidence, Footstep...</summary>
    public void PlaySFXAtPosition(string id, Vector3 position)
    {
        if (!TryGetEntry(id, out SFXEntry entry)) return;

        GameObject tempGO = new GameObject($"SFX3D_{id}");
        tempGO.transform.position = position;

        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = entry.Clip;
        source.volume = entry.Volume;
        source.pitch = entry.Pitch;
        source.spatialBlend = 1f; // 3D
        source.outputAudioMixerGroup = _sfxMixerGroup;
        source.Play();

        float safePitch = Mathf.Max(entry.Pitch, 0.01f);
        Destroy(tempGO, entry.Clip.length / safePitch);
    }

    // ── Helper nội bộ ─────────────────────────────
    private bool TryGetEntry(string id, out SFXEntry entry)
    {
        entry = null;

        if (_sfxDict == null || !_sfxDict.TryGetValue(id, out entry))
        {
            Debug.LogWarning($"[AudioManager] Không tìm thấy SFX ID: \"{id}\".");
            return false;
        }

        if (entry.Clip == null)
        {
            Debug.LogWarning($"[AudioManager] SFX ID \"{id}\" chưa được gán AudioClip.");
            return false;
        }

        return true;
    }

    private AudioSource GetNextTwoDSource()
    {
        AudioSource source = _sfx2DSources[_next2DSourceIndex];
        _next2DSourceIndex = (_next2DSourceIndex + 1) % _sfx2DSources.Length;
        return source;
    }
}