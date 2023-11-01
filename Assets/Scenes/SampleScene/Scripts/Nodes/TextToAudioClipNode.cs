using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class TextToAudioClipNode : BaseNode<string,AudioClip>
{
    AudioClip _audioClip;
    int _count;
    protected override IEnumerator ProcessData(string text)
    {

        yield return TextToAudioAPI(text);
        SendData(_audioClip);
    }
    public IEnumerator TextToAudioAPI(string text)
    {

        var form = new WWWForm();
        form.AddField("text", text);
        form.AddField("character", _config.API_Configs.AudioCharacter);

        string url = "https://" + _config.AI_Server_IP_Port + _config.API_Configs.TextToAudio;
        if (_debug)
        {
            Debug.Log("url: " + url);
            Debug.Log("AudioCharacter: " + _config.API_Configs.AudioCharacter);
        }
        UnityWebRequest request = UnityWebRequest.Post(url, form);

        // リクエストを送信し、レスポンスを待機
        yield return request.SendWebRequest();
        if(request.result == UnityWebRequest.Result.Success)
        {
            //Send Data to the next nodes
            byte[] audioByteData = request.downloadHandler.data;
            string base64Data = Convert.ToBase64String(audioByteData);
            byte[] decodedBytes = Convert.FromBase64String(base64Data);

            float[] pcmArray = new float[decodedBytes.Length / 4];
            for (int i = 0; i < pcmArray.Length; i++)
            {
                pcmArray[i] = BitConverter.ToSingle(decodedBytes, i * 4);
            }
#if UNITY_WEBGL && !UNITY_EDITOR
            _audioClip = AudioClip.Create(_count.ToString(), (int)(pcmArray.Length * (22050/44100f)), 1, 22050, false);
#else
            _audioClip = AudioClip.Create(_count.ToString(), pcmArray.Length, 1, 22050, false);
#endif
            _count++;
            _audioClip.SetData(pcmArray, 0);
        }
        else
        {
                Debug.LogError("Audio Inference has been Failed.");
        }
    }
}
