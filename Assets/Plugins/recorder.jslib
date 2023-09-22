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

                console.log("[JS]audioChunks",audioChunks);
                const chunkBlob = new Blob(audioChunks, {"type":'audio/webm'});
                const reader = new FileReader();
                reader.readAsArrayBuffer(chunkBlob);

                reader.onload = (e) => {
                    console.log("[JS3]reader.result",e.target.result);

                    audioContext.decodeAudioData(e.target.result)
                    .then(audioBuffer => {
                        console.log("[JS4]buf",audioBuffer);
                        const source = audioContext.createBufferSource();
                        source.buffer = audioBuffer;
                        source.connect(audioContext.destination);
                        source.start();

                        source.onended = (e) => {

                            const pcmArray = audioBuffer.getChannelData(0);
                            console.log("[JS5]pcmArray",pcmArray);

                            let maxAbsoluteValue = 0;

                            for (let i = 0; i < pcmArray.length; i++) {
                                const absoluteValue = Math.abs(pcmArray[i]);
                                if (absoluteValue > maxAbsoluteValue) {
                                    maxAbsoluteValue = absoluteValue;
                                }
                            }
                            console.log("[JS]maxAbsoluteValue: ",maxAbsoluteValue);

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
                            console.log("[JS] blob ",blob);

                            console.log("[JS7]samplingRate:",audioContext.sampleRate);
                            var json_arguments = {
                                blobUrl: blobUrl,
                                samplingRate: audioContext.sampleRate
                            }
                            console.log("[JS]signifying")
                            var json_string = JSON.stringify(json_arguments);
                            console.log("[JS]Sending Message")
                            try {
                                unityInstance.SendMessage('AudioManager', 'ReceiveAudioData', json_string)
                            }catch (error) {
                                console.error("[JS] Error",error)
                            }
                        }
                    });
                };
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