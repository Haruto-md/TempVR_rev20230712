using System.Collections.Generic;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
public class AudioClipInputNode : BaseNode<AudioClip,string>
{
    string _transcription;
    protected override IEnumerator ProcessData(AudioClip audioClip){
        yield return AudioToTextAPI(audioClip.GetData(audioClip.samples,0),audioClip.frequency);
        SendData(_transcription);
    }
    public IEnumerator AudioToTextAPI(float[] audioDataFloat, int samplingRate)
    {
        byte[] audioBinaryData = new byte[audioDataFloat.Length * 4];
        Buffer.BlockCopy(audioDataFloat, 0, audioBinaryData, 0, audioBinaryData.Length);

        var form = new WWWForm();
        form.AddBinaryData("audio_data", audioBinaryData, "audio.wav", "audio/wav");

        // UnityWebRequestを作成し、FormDataを設定
        UnityWebRequest request = UnityWebRequest.Post(_config.API_ENDPOINTs.AudioToText, form);
        request.downloadHandler = new DownloadHandler();

        // リクエストを送信し、レスポンスを待機
        yield return request.SendWebRequest();
        //Send Data to the next nodes
        _transcription = request.downloadHandler.text;

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("[Unity]request has been completed Successfully!");
        }
    }
}