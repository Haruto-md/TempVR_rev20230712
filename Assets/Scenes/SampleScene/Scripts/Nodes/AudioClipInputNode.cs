using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
public class AudioClipInputNode : BaseNode<AudioClip,AudioClip>,IAudioRecorder
{
    private bool _isRecording = false;
    private void Update()
    {
        // キー入力などを検知して録音を制御
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartRecording();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            StopRecording();
        }else if(Input.GetKeyDown(KeyCode.C))
        {
            CancelRecording();
        }
    }
    [DllImport("__Internal")]
    private static extern void startRecording();
    [DllImport("__Internal")]
    private static extern void stopRecording();

    // 録音を開始する関数
    public void StartRecording()
    {
        if (!_isRecording)
        {
            startRecording();
            _isRecording = true;
        }
    }

    // 録音を停止する関数
    public void StopRecording()
    {
        if (_isRecording)
        {
            stopRecording();
            _isRecording = false;
        }
    }
    public void CancelRecording()
    {
        stopRecording();
        //Not turn _isRecording into false
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
        if(!_isRecording){
            StartCoroutine(DownloadAudioData(json));
        }else{
            _isRecording = false;
        }
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

                AudioClip audioClip;
#if UNITY_WEBGL && !UNITY_EDITOR
                audioClip = AudioClip.Create("recordedClip", (int)(pcmArray.Length * (arguments.samplingRate/44100f)), 1, arguments.samplingRate, false);
#else
                audioClip = AudioClip.Create("recordedClip", pcmArray.Length, 1,arguments.samplingRate, false);
#endif
                audioClip.SetData(pcmArray, 0);

                SendData(audioClip);
            }
            else
            {
                Debug.LogError("[Unity]Download error: " + www.error);
            }
        }
    }
}