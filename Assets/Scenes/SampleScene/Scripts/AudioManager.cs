using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class AudioManager : MonoBehaviour
{
    // �^�����J�n���ꂽ���ǂ����̃t���O
    private bool isRecording = false;

    public S2T2T2S_requester requestor;

    private void Update()
    {
        // �L�[���͂Ȃǂ����m���Ę^���𐧌�
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartRecording();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            StopRecording();
        }
    }
    [DllImport("__Internal")]
    private static extern void startRecording();
    [DllImport("__Internal")]
    private static extern void stopRecording();
    // �^�����J�n����֐�
    public void StartRecording()
    {
        if (!isRecording&&!requestor.isRequesting)
        {
            startRecording();
            isRecording = true;
        }
    }

    // �^�����~����֐�
    public void StopRecording()
    {
        if (isRecording)
        {
            stopRecording();
            isRecording = false;
        }
    }

    [System.Serializable]
    private class AudioArguments
    {
        public string arrayBuffer;
        public int samplingRate;
    }
    [System.Serializable]
    private class URLArguments
    {
        public string blobUrl;
        public int samplingRate;
    }

    public void ReceiveAudioData(string json)
    {
        StartCoroutine(DownloadAudioData(json));
    }

    private IEnumerator DownloadAudioData(string json)
    {
        yield return 0;
        URLArguments arguments = JsonUtility.FromJson<URLArguments>(json);
        using (UnityWebRequest www = UnityWebRequest.Get(arguments.blobUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                byte[] byteData = www.downloadHandler.data;
                string base64Data = Convert.ToBase64String(byteData);
                byte[] decodedBytes = Convert.FromBase64String(base64Data);

                float[] pcmArray = new float[decodedBytes.Length / 4];
                for (int i = 0; i < pcmArray.Length; i++)
                {
                    pcmArray[i] = BitConverter.ToSingle(decodedBytes, i * 4);
                }

                Debug.Log("[Unity]Start Communincating");
                StartCoroutine(requestor.ComunicateAPI(pcmArray, arguments.samplingRate));
            }
            else
            {
                Debug.LogError("[Unity]Download error: " + www.error);
            }
        }
    }
    private float[] ByteToFloatArray(byte[] byteArray)
    {
        float[] floatArray = new float[byteArray.Length / 4];

        for (int i = 0; i < floatArray.Length; i++)
        {
            byte[] tempBytes = new byte[4];
            Array.Copy(byteArray, i * 4, tempBytes, 0, 4);
            floatArray[i] = BitConverter.ToSingle(tempBytes, 0);
        }
        return floatArray;
    }
}