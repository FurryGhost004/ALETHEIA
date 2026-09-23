using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CourtTrialController : MonoBehaviour
{
    public static SuspectData ConvictedSuspect { get; set; }

    [Header("Spawn Points")]
    [SerializeField] private Transform _suspectSpawnPoint;
    [SerializeField] private Transform _playerSpawnPoint;

    [Header("Player Reference")]
    [SerializeField] private GameObject _playerPrefab; // Kéo Prefab Player từ Project vào đây

    [Header("UI References (Optional)")]
    [SerializeField] private TextMeshProUGUI _txtSuspectName;
    [SerializeField] private Image _imgSuspectPortrait;

    private void Start()
    {
        // 1. Spawn hoặc di chuyển Player vào đúng PlayerSpawnPoint
        SetupPlayer();

        // 2. Spawn mô hình 3D của Nghi phạm tại SuspectSpawnPoint
        SpawnSuspectModel();
    }

    private void SetupPlayer()
    {
        if (_playerSpawnPoint == null)
        {
            Debug.LogWarning("[CourtTrial] Thiếu PlayerSpawnPoint!");
            return;
        }

        // Tìm xem Player đã có sẵn trên Scene chưa (ví dụ: chuyển từ Scene trước sang)
        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");

        if (existingPlayer != null)
        {
            // Nếu đã có Player trên Scene, dịch chuyển về PlayerSpawnPoint
            CharacterController cc = existingPlayer.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            existingPlayer.transform.position = _playerSpawnPoint.position;
            existingPlayer.transform.rotation = _playerSpawnPoint.rotation;

            if (cc != null) cc.enabled = true;
        }
        else if (_playerPrefab != null)
        {
            // Nếu chưa có trên Scene, Spawn một Player mới từ Prefab
            Instantiate(_playerPrefab, _playerSpawnPoint.position, _playerSpawnPoint.rotation);
        }
        else
        {
            Debug.LogError("[CourtTrial] Chưa gán Player Prefab trong Inspector!");
        }
    }

    private void SpawnSuspectModel()
    {
        if (ConvictedSuspect == null)
        {
            Debug.LogWarning("[CourtTrial] Không tìm thấy nghi phạm nào được truyền sang!");
            return;
        }

        if (ConvictedSuspect.Suspect3DPrefab != null && _suspectSpawnPoint != null)
        {
            Instantiate(ConvictedSuspect.Suspect3DPrefab, _suspectSpawnPoint.position, _suspectSpawnPoint.rotation);
        }

        if (_txtSuspectName != null) _txtSuspectName.text = ConvictedSuspect.NpcName;
        if (_imgSuspectPortrait != null && ConvictedSuspect.Portrait != null)
        {
            _imgSuspectPortrait.sprite = ConvictedSuspect.Portrait;
        }
    }
}