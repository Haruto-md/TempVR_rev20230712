using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

public class InitializationManager : MonoBehaviour
{
    public string fileName = "configs.json"; // 読み込むJSONファイルの名前
    private string filePath; // ファイルのフルパス

    [System.Serializable]
    public class ConfigData
    {
        public string AI_Server_IP;
        public string AI_Server_Port;
        public string API_ENDPOINT;
    }

    public ConfigData config_data;
    void Awake()
    {
        // StreamingAssetsフォルダのパスを取得
        string streamingAssetsPath = Application.streamingAssetsPath;

        // WebGLの場合、StreamingAssets内のファイルには直接アクセスできないため、UnityWebRequestを使用して読み込む
        filePath = Path.Combine(streamingAssetsPath, fileName);
    }

    public IEnumerator LoadJsonFile()
    {
        // UnityWebRequestを使用してファイルを非同期で読み込む
        using (UnityWebRequest www = UnityWebRequest.Get(filePath))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // JSONデータを文字列として取得
                string jsonContent = www.downloadHandler.text;

                // JSONデータをパースしてC#のデータ構造に変換
                config_data = JsonUtility.FromJson<ConfigData>(jsonContent);
            }
            else
            {
                Debug.LogError("JSONファイルの読み込みに失敗しました: " + www.error);
            }
        }
    }
}