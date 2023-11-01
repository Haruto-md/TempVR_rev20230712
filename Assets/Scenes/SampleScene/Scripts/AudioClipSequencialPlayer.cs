using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipSequencialPlayer : MonoBehaviour, IDataReceiver<AudioClip>
{
    private List<AudioClip> _audioClips = new List<AudioClip>();

    private bool isPlaying = false;
    public bool _debug = true;

    private void Update()
    {
        // オーディオが再生中でない場合、次のオーディオを再生する
        if (!isPlaying && _audioClips.Count > 0)
        {
            PlayNextAudioClip();
            if (_debug)
            {
                Debug.Log("Play Nect Audio. remaining: "+(_audioClips.Count-1));
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (_audioClips.Count != 0)
            {
                _audioClips.RemoveAt(0);
                Debug.Log("Deleted audioClip. remaining: " + (_audioClips.Count-1));
            }
        }
    }

    private void PlayNextAudioClip()
    {
        isPlaying = true;
        StartCoroutine(PlayAudioClip(_audioClips[0]));
    }

    private IEnumerator PlayAudioClip(AudioClip clip)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();

        while (audioSource.isPlaying)
        {
            yield return null;
        }

        Destroy(audioSource);

        // 再生が終了したらリストから削除
        _audioClips.RemoveAt(0);
        isPlaying = false;
    }

    public void ReceiveData(AudioClip audioClip)
    {
        _audioClips.Add(audioClip);
    }
}