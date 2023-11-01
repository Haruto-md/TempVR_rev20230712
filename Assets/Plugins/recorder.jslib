let audioChunks;
let audioContext;
let mediaRecorder;
mergeInto(LibraryManager.library,{
    startRecording: function () {
        console.log("[JS1]start recording");
        navigator.mediaDevices.getUserMedia({audio: true,video:false})
        .then(stream => {
            audioChunks = [];
            audioContext = new AudioContext();
            mediaRecorder = new MediaRecorder(stream,{mimeType: 'audio/webm'})

            // var streamSource = audioContext.createMediaStreamSource(stream);
            // streamSource.connect(audioContext.destination);

            mediaRecorder.addEventListener('dataavailable', e => {
                audioChunks.push(e.data);
            });
            mediaRecorder.addEventListener('stop', e => {
                const chunkBlob = new Blob(audioChunks, {"type":'audio/webm'});
                const reader = new FileReader();
                reader.readAsArrayBuffer(chunkBlob);

                reader.onload = (e) => {

                    audioContext.decodeAudioData(e.target.result)
                    .then(audioBuffer => {
                        const source = audioContext.createBufferSource();
                        source.buffer = audioBuffer;
                        const pcmArray = audioBuffer.getChannelData(0);

                        const uint8Array = new Uint8Array(pcmArray.buffer);
                        // let binary = '';
                        // const length = uint8Array.length;
                        // for (let i = 0; i < length; i++) {
                        //     binary += String.fromCharCode(uint8Array[i]);
                        // }
                        // var audioDataString = btoa(binary);

                        const blob = new Blob([uint8Array], {"type":'application/octet-stream'});
                        const blobUrl = URL.createObjectURL(blob);
                        console.log("[JS] blobUrl ",blobUrl);

                        console.log("[JS7]samplingRate:",audioContext.sampleRate);
                        var json_arguments = {
                            blobUrl: blobUrl,
                            samplingRate: audioContext.sampleRate
                        }
                        var json_string = JSON.stringify(json_arguments);
                        try {
                            unityInstance.SendMessage('AudioClipInputNode', 'ReceiveAudioData', json_string)
                        }catch (error) {
                            console.error("[JS] Error",error)
                        }
                    });
                }
            });
            mediaRecorder.start();
        })
        .catch(error => {
            console.error("getUserMedia error:", error);
        });
    },
    stopRecording: function () {
        if(mediaRecorder.state = "recording"){
            console.log("[JS2]stop recording")
            mediaRecorder.stop();
        }
    }
});