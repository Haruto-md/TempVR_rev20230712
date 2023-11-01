using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public class AudioClipToTextNode : BaseNode<AudioClip,string>
{
    private string _transcription;

    protected override IEnumerator ProcessData(AudioClip audioClip){

        yield return AudioToTextAPI(audioClip);
        SendData(_transcription);
    }
    public IEnumerator AudioToTextAPI(AudioClip audioClip)
    {
        float[] pcmData = new float[audioClip.samples];
        audioClip.GetData(pcmData, 0);
        byte[] audioBinaryData = new byte[pcmData.Length * 4];
        Buffer.BlockCopy(pcmData, 0, audioBinaryData, 0, audioBinaryData.Length);

        var form = new WWWForm();
        form.AddField("sampling_rate", audioClip.frequency);
        form.AddBinaryData("audio_data", audioBinaryData, "audio.wav", "audio/wav");

        string url = "https://"+_config.AI_Server_IP_Port + _config.API_Configs.AudioToText;

        UnityWebRequest request = UnityWebRequest.Post(url, form);

        // リクエストを送信し、レスポンスを待機
        yield return request.SendWebRequest();
        //Send Data to the next nodes
        _transcription = request.downloadHandler.text;

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
    }
}