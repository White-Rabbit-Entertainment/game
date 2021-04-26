var WebRTCPlugin = {

  $Data: {
    peerConnections: {},
    localStream: null,
    remoteStream: null
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
            // Show local stream
            // var videoElement = document.createElement("video");
            // document.body.appendChild(videoElement);
            // videoElement.autoplay = true;
            // videoElement.controls = "false";
            // videoElement.playsinline = true;
            // videoElement.srcObject = stream;
            // videoElement.muted = true;
        })
        .catch(function(error) {
            console.error('Error accessing media devices.', error);
        });
  },

  MakeOffer: function(id) {
      var peerConnection = Module.WebRTCPre.CreatePeerConnection(id, Data.localStream)
      Data.peerConnections[id] = peerConnection
      peerConnection.peerId = id

      peerConnection.createOffer({offerToReceiveAudio: true, offerToReceiveVideo: true})
        .then(function(offer) {
            peerConnection.setLocalDescription(offer)
              .then(function() {
                console.log("Made offer");
                var offerData = {"offer": JSON.stringify(offer), "peerId": id}
                console.log(JSON.stringify(offerData));
                unityInstance.SendMessage("WebRTC", "SendOffer", JSON.stringify(offerData));
              });
        });
  },

  MakeAnswer: function(jsonData) {
      const data = JSON.parse(Pointer_stringify(jsonData));
      data.offer = JSON.parse(data.offer);

      var peerConnection = Module.WebRTCPre.CreatePeerConnection(data.peerId, Data.localStream)
      // Sender Id (Who we are sending the answer to
      peerConnection.setRemoteDescription(new RTCSessionDescription(data.offer))
        .then(function() {
          peerConnection.createAnswer()
            .then(function(answer) {
              peerConnection.setLocalDescription(answer)
                .then(function() {
                  console.log("Made answer");
                  var answerData = {"peerId": data.peerId, "answer": JSON.stringify(answer)}
                  unityInstance.SendMessage("WebRTC", "SendAnswer", JSON.stringify(answerData));
                });
              });
          });
  },

  ApplyAnswer: function(jsonData) {
      const data = JSON.parse(Pointer_stringify(jsonData));
      data.answer = JSON.parse(data.answer);
      var peerConnection = Data.peerConnections[data.peerId]

      const answer = new RTCSessionDescription(data.answer);
      peerConnection.setRemoteDescription(data.answer).then(function() {
        console.log("Handle answer complete");
      });
  },

  ApplyIceCandidate: function(candidateData) {
      const data = JSON.parse(Pointer_stringify(candidateData));
      data.candidate = JSON.parse(data.candidate)
      Data.peerConnections[data.peerId].addIceCandidate(data.candidate).then(function() {
        console.log("Applied ice candidate");
        console.log(data.candidate);
      });
  },

  RemovePeerConnection: function(playerId) {
      const peerConnection = Data.peerConnections[playerId]
      peerConnection.videoElement.remove()
      peerConnection.close()
      delete Data.peerConnections[playerId]
  },
  
  SetPeerVolume: function(playerId, volume) {
      Data.peerConnections[playerId].videoElement.volume = volume
  },
  
  GetPeerVolume: function(playerId) {
      return Data.peerConnections[playerId].videoElement.volume
  },

  HelloString: function (str) {
      window.alert(Pointer_stringify(str));
  }

};

autoAddDeps(WebRTCPlugin, '$Data');
mergeInto(LibraryManager.library, WebRTCPlugin);
