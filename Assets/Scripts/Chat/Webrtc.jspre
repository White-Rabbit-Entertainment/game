Module['WebRTCPre'] = Module['WebRTCPre'] || {};

Module['WebRTCPre'].CreatePeerConnection = function (id, localStream) {

      const configuration = {
          'iceServers': [
              {
                  urls: 'turn:157.90.119.115:3478',
                  username: 'test',
                  credential: 'test123'
              }
          ]
      }
      const peerConnection = new RTCPeerConnection(configuration)
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
              var data = JSON.stringify({"candidate": JSON.stringify(event.candidate), "peerId": peerConnection.peerId});
              unityInstance.SendMessage("WebRTC", "SendIceCandidate", data);
          }
      };
      // Add out local video and audio
      localStream.getTracks().forEach(function(track) {
          peerConnection.addTrack(track, localStream);
      });
      
      return peerConnection
}
