using UnityEngine;
using System.Collections;
using System;
using System.Text;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using My.Communication;

public class S2T2T2S_requester : MonoBehaviour
{
    [SerializeField] public TextAsset textAsset;
    
    public string url;
    public List<AudioClip> receivedAudioClips = null;
    public int audioClipIndex = 0;
    public bool isRequesting;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public IEnumerator ComunicateAPI(float[] audioDataFloat, int samplingRate)
    {
        StartCoroutine(playAudioSequentially());

        byte[] audioBinaryData = new byte[audioDataFloat.Length * 4];
        Buffer.BlockCopy(audioDataFloat, 0, audioBinaryData, 0, audioBinaryData.Length);

        var form = new WWWForm();
        form.AddField("role1","user");
        form.AddField("content1", textAsset.text);
        form.AddField("sampling_rate", samplingRate);

        form.AddBinaryData("audio_data", audioBinaryData, "audio.wav", "audio/wav");

        // UnityWebRequestを作成し、FormDataを設定
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        request.downloadHandler = new StreamingDownloadHandler(this.gameObject);

        // リクエストを送信し、レスポンスを待機
        isRequesting = true;
        yield return request.SendWebRequest();
        Debug.Log("[Unity]request has been finished!");
        isRequesting = false;

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("[Unity]request has been completed Successfully!");
        }
    }

    public IEnumerator playAudioSequentially()
    {
        audioClipIndex = 0;
        for (;;)
        {
            if(receivedAudioClips.Count > audioClipIndex && !audioSource.isPlaying)
            {
                var currentAudioClip = receivedAudioClips[audioClipIndex];
                audioClipIndex++;
                audioSource.clip = currentAudioClip;
                audioSource.Play();
                Debug.Log("Playing AudioClip: " + audioSource.clip.name);
            }
            yield return new WaitForFixedUpdate();
            if (!isRequesting&&!audioSource.isPlaying&&audioClipIndex==receivedAudioClips.Count)
            {
                break;
            }
        }
        Debug.Log("Finish Playing");
        audioClipIndex = 0;
        receivedAudioClips = new List<AudioClip>();
    }
}
namespace My.Communication
{
    public class StreamingDownloadHandler : DownloadHandlerScript
    {
        S2T2T2S_requester requester;
        public StreamingDownloadHandler(GameObject gameObject)
        {
            this.requester = gameObject.GetComponent<S2T2T2S_requester>();
        }
        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            float[] samples = new float[data.Length / 4];
            Buffer.BlockCopy(data,0,samples,0,data.Length);
            int samplingRate = 20500;
            AudioClip singleAudioClip = CreateAudioClip(samples, samplingRate);
            requester.receivedAudioClips.Add(singleAudioClip);
            Debug.Log("Extend audioClips. index = " + requester.audioClipIndex);
            return base.ReceiveData(data, dataLength);
        }
        AudioClip CreateAudioClip(float[] audioData, int samplingRate)
        {
            // float配列からAudioClipを生成する処理
            AudioClip audioClip = AudioClip.Create("receivedAudioClip_" + Time.time, audioData.Length, 1, samplingRate, false);
            audioClip.SetData(audioData, 0);
            return audioClip;
        }
    }
}