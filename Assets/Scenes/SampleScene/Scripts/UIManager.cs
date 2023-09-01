using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public AudioManager audioManager; // AudioManagerÇ÷ÇÃéQè∆ÇInspectorÇ©ÇÁê›íË

    public Button startRecordingButton;
    public Button stopRecordingButton;

    private void Start()
    {
        startRecordingButton.onClick.AddListener(StartRecording);
        stopRecordingButton.onClick.AddListener(StopRecording);
    }

    private void StartRecording()
    {
        Debug.Log("[INFO]Start Recording Butten Clicked");
        audioManager.StartRecording();
    }

    private void StopRecording()
    {
        Debug.Log("[INFO]Stop Recording Butten Clicked");
        audioManager.StopRecording();
    }
}