using UnityEngine;
using Newtonsoft.Json;

public class PlayerPrefsHandler
{
    public void SaveData<T>(string key, T data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.Save();
    }

    public T LoadData<T>(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string jsonData = PlayerPrefs.GetString(key);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }

        return default;
    }

    public void DeleteData(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }
}
