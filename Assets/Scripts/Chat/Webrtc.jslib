var WebRTCPlugin = {

  $Data: {
    peerConnections: {},
    localStream: null,
    remoteStream: null,
  },
  
  Hello: function () {
    window.alert("Hello, world!");
  },
  
  Init: function() {
    // Setup local stream
    const constraints = {'video': true, 'audio': true}
    navigator.mediaDevices.getUserMedia(constraints)
        .then(function(stream) {
            console.log('Got MediaStream:', stream);
            Data.localStream = stream;
        })
        .catch(function(error) {
            console.error('Error accessing media devices.', error);
        });
  },

  CreatePeerConnection: function (id) {
      const peerConnection = new RTCPeerConnection(Data.configuration)
      peerConnection.peerId = id
      Data.peerConnections[id] = peerConnection
      
      const stream = new MediaStream();
      var videoElement = document.createElement("video");
      document.body.appendChild(videoElement);
      videoElement.autoplay = true;
      videoElement.controls = "false";
      videoElement.playsinline = true;
      videoElement.srcObject = stream;
  
      peerConnection.ontrack = function() {stream.addTrack(event.track, stream)}
      peerConnection.onnegotiationneeded = function(event) {console.log("Negotiation needed!")};
      peerConnection.onicecandidate = function(event) {
          if (event.candidate) {
              var data = JSON.stringify({"candidate": event.candidate, "peerId": peerConnection.peerId});
              unityInstance.SendMessage("WebRTC", "SendIceCandidate", data);
          }
      };
      // Add out local video and audio
      localStream.getTracks().forEach(function(track) {
          peerConnection.addTrack(track, localStream);
      });
      
      return peerConnection
  }



};

autoAddDeps(WebRTCPlugin, '$Data');
autoAddDeps(WebRTCPlugin, 'CreatePeerConnection');
mergeInto(LibraryManager.library, WebRTCPlugin);
