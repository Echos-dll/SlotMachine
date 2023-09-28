using System;
using Newtonsoft.Json;
using UnityEngine;

public static class SaveSystem
{
    public static void Save(SaveData saveData)
    {
        string data = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("SaveData", data);
    }

    public static bool TryLoad(out SaveData dataCount)
    {
        dataCount = new SaveData();

        if (!PlayerPrefs.HasKey("SaveData")) return false;
        
        SaveData load = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString("SaveData"));
        dataCount._pocketData = load._pocketData;
        dataCount._pickedCountData = load._pickedCountData;
        dataCount._pickedAmountsData = load._pickedAmountsData;
        return true;
    }
}

[Serializable]
public struct SaveData
{
    [JsonProperty("pcd")] public int _pickedCountData;
    [JsonProperty("pd")] public float[] _pocketData;
    [JsonProperty("pa")] public int[] _pickedAmountsData;
}
