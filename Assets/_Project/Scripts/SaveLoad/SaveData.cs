// Path: _Project/Scripts/SaveLoad/SaveData.cs

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class chứa toàn bộ data cần lưu khi người chơi Save Case hoặc Quit Game.
/// Đây KHÔNG phải MonoBehaviour — chỉ là data container, được SaveSystem
/// serialize ra JSON và lưu vào file (theo UC03 Continue Case).
///
/// [Serializable] để JsonUtility.ToJson()/FromJson() đọc/ghi được.
/// </summary>
[Serializable]
public class SaveData
{
    // ── Case hiện tại ─────────────────────────────
    public string CurrentCaseId;

    // ── Investigation: vị trí nhân vật (UC06) ────
    public Vector3 PlayerPosition;

    // ── Time System: ngày + thời gian còn lại (UC14) ──
    public int CurrentDay;
    public float TimeRemaining;

    // ── Notebook: bằng chứng + từ khóa đã thu thập (UC10) ──
    public List<string> CollectedEvidenceIds = new List<string>();
    public List<string> CollectedKeywordIds = new List<string>();

    // ── Convict Suspect: lịch phiên tòa (UC15) ───
    public bool HasScheduledCourtTrial;
    public string ScheduledSuspectId;
    public int ScheduledCourtDay;

    // ── Court Trial: số lần đã hoãn phiên tòa (UC18) ──
    public int PostponeCount;
}
