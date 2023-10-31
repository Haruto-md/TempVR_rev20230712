using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

public class InitializationManager : MonoBehaviour
{
    public string fileName = "configs.json"; // �ǂݍ���JSON�t�@�C���̖��O
    private string filePath; // �t�@�C���̃t���p�X

    [System.Serializable]
    public class ConfigData
    {
        public string AI_Server_IP;
        public string AI_Server_Port;
        public API_Configs API_Configs;
        public string protcol;
    }
    public class API_Configs
    {
        public string AudioToTextEndPoint;
        public string Text2AudioEndPoint;
        public Chat Chat;

    }
    public class Chat{
        public string ChatWhole
        public string GetNewChat
        public string InitialPrompt;
    }

    public ConfigData config_data;
    void Awake()
    {
        // StreamingAssets�t�H���_�̃p�X���擾
        string streamingAssetsPath = Application.streamingAssetsPath;

        // WebGL�̏ꍇ�AStreamingAssets���̃t�@�C���ɂ͒��ڃA�N�Z�X�ł��Ȃ����߁AUnityWebRequest���g�p���ēǂݍ���
        filePath = Path.Combine(streamingAssetsPath, fileName);
        StartCoroutine(LoadConfig());
    }

    public IEnumerator LoadConfig()
    {
        // UnityWebRequest���g�p���ăt�@�C����񓯊��œǂݍ���
        using (UnityWebRequest www = UnityWebRequest.Get(filePath))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // JSON�f�[�^�𕶎���Ƃ��Ď擾
                string jsonContent = www.downloadHandler.text;

                // JSON�f�[�^���p�[�X����C#�̃f�[�^�\���ɕϊ�
                config_data = JsonUtility.FromJson<ConfigData>(jsonContent);
            }
            else
            {
                Debug.LogError("Failed to load Config.: " + www.error);
            }
        }
    }
}