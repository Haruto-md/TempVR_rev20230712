public interface IAudioRecorder
{
    void StartRecording();//start recording
    void StopRecording();//override current audioClip
    void CancelRecording();
}