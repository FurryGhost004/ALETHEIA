using UnityEngine;

[System.Serializable]
public class SuspectData
{
    public string NpcId;
    public string NpcName;
    public Sprite Portrait;

    public SuspectData(string id, string name, Sprite portrait)
    {
        NpcId = id;
        NpcName = name;
        Portrait = portrait;
    }
}