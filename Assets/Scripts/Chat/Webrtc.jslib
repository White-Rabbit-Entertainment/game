var WebRTCPlugin = {

  $Data: {
    peerConnection: null,
    localStream: null,
    remoteStream: null,
    constraints = {'video': true, 'audio': true}
    configuration = {'iceServers': [{'urls': 'stun:stun.l.google.com:19302'}]}
  },

  Init: function() {
    // Setup remote stream
    Data.remoteStream = new MediaStream();
    // const remoteVideo = document.querySelector('#remoteVideo');
    // Data.remoteVideo.srcObject = remoteStream;

    // Setup peer connection
    Data.peerConnection = new RTCPeerConnection(Data.configuration);
    Data.peerConnection.ontrack = () => Data.remoteStream.addTrack(event.track, remoteStream)
    Data.peerConnection.onnegotiationneeded = (event) => console.log("Negotiation needed!");
    Data.peerConnection.onicecandidate = (event) => {
        console.log("icecandidate happened")
        if (event.candidate) {
            console.log("icecandidate really happened")
            // socket.emit("message", {"iceCandidate": event.candidate});
        }
    };

    Data.peerConnection.onconnectionstatechanged = (event) => {
        // If peerConnection becomes connected
        if (peerConnection.connectionState === 'connected') {
            // Peers connected!
            console.log("CONNECTED!!!!!!")
        }
    };

    // SetupLocalStream();
  }

  SetupLocalStream: function () {
      // try {
      //     navigator.mediaDevices.getUserMedia(Data.constraints)
      //         .then(stream => {
      //             console.log('Got MediaStream:', stream);
      //         })
      //         .catch(error => {
      //             console.error('Error accessing media devices.', error);
      //         });
  
      //     Data.localStream = await navigator.mediaDevices.getUserMedia(Data.constraints);
      //     // const videoElement = document.querySelector('video#localVideo');
      //     // videoElement.srcObject = Data.localStream;
      //     Data.localStream.getTracks().forEach(track => {
      //         console.log("Sending track")
      //         console.log(track)
      //         Data.peerConnection.addTrack(track, Data.localStream);
      //     });
      // } catch(error) {
      //     console.error('Error opening video camera.', error);
      // }
  }

};

autoAddDeps(WebRTCPlugin, '$Data');
mergeInto(LibraryManager.library, WebRTCPlugin);
