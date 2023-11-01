using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class InitializationManager : MonoBehaviour
{
    public string _fileName = "test_config.json";
    private string _filePath;

    public ConfigData config_data;
    void Awake()
    {
        string streamingAssetsPath = Application.streamingAssetsPath;

        _filePath = Path.Combine(streamingAssetsPath, _fileName);
        StartCoroutine(LoadConfig());
    }

    public IEnumerator LoadConfig()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(_filePath))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonContent = www.downloadHandler.text;

                Debug.Log("Load jsonContent: " + jsonContent);
                config_data = JsonUtility.FromJson<ConfigData>(jsonContent);
            }
            else
            {
                Debug.LogError("Failed to load Config.: " + www.error);
            }
            Debug.Log("Load Config File: " + _fileName);

        }
    }
}