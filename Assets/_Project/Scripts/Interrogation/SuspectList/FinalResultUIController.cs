using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalResultUIController : MonoBehaviour
{
    public static FinalResultUIController Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject _panelFinalResult;
    [SerializeField] private TextMeshProUGUI _txtTitle;
    [SerializeField] private TextMeshProUGUI _txtMessage;
    [SerializeField] private Button _btnRestart;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (_btnRestart != null)
        {
            _btnRestart.onClick.AddListener(RestartGame);
        }
    }

    public void ShowResult(bool isWin, SuspectData suspect)
    {
        if (_panelFinalResult != null)
        {
            _panelFinalResult.SetActive(true);
        }

        if (isWin)
        {
            if (_txtTitle != null) _txtTitle.text = "TÒA TUYÊN ÁN: PHÁ ÁN THÀNH CÔNG!";
            if (_txtMessage != null)
            {
                _txtMessage.text = $"Chính xác! {suspect.NpcName} chính là hung thủ thực sự của vụ án. Bạn đã hoàn thành xuất sắc nhiệm vụ!";
            }
        }
        else
        {
            if (_txtTitle != null) _txtTitle.text = "TÒA TUYÊN ÁN: BẮT LẦM NGUỜI VÔ TỘI!";
            if (_txtMessage != null)
            {
                _txtMessage.text = $"{suspect.NpcName} hoàn toàn vô tội! Hung thủ thật sự đã tẩu thoát. Vụ án thất bại!";
            }
        }

        // Mở lại con trỏ chuột để bấm nút Chơi lại
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void RestartGame()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
}