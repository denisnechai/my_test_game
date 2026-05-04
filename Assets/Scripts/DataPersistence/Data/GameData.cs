using UnityEngine;
[System.Serializable]
public class GameData
{
    public int move;
    public SerializableDictionary<string, bool> LevelsCompleted;
    public GameData()
    {
        this.LevelsCompleted = new SerializableDictionary<string, bool>();
    }
}

