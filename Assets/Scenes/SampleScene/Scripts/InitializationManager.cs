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
        public string API_ENDPOINT;
    }

    public ConfigData config_data;
    void Awake()
    {
        // StreamingAssets�t�H���_�̃p�X���擾
        string streamingAssetsPath = Application.streamingAssetsPath;

        // WebGL�̏ꍇ�AStreamingAssets���̃t�@�C���ɂ͒��ڃA�N�Z�X�ł��Ȃ����߁AUnityWebRequest���g�p���ēǂݍ���
        filePath = Path.Combine(streamingAssetsPath, fileName);
    }

    public IEnumerator LoadJsonFile()
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
                Debug.LogError("JSON�t�@�C���̓ǂݍ��݂Ɏ��s���܂���: " + www.error);
            }
        }
    }
}