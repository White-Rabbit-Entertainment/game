var WebRTCPlugin = {

  $Data: {
    peerConnections: {},
    localStream: null,
    remoteStream: null,
    configuration = {
        'iceServers': [
            {
                urls: 'turn:157.90.119.115:3478',
                username: 'test',
                credential: 'test123'
            }
        ]
    },
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
  
      peerConnection.ontrack = () => stream.addTrack(event.track, stream)
      peerConnection.onnegotiationneeded = (event) => console.log("Negotiation needed!");
      peerConnection.onicecandidate = (event) => {
          if (event.candidate) {
              var data = JSON.stringify({"candidate": event.candidate, "peerId": peerConnection.peerId});
              unityInstance.SendMessage("WebRTC", "SendIceCandidate", data);
          }
      };
      // Add out local video and audio
      localStream.getTracks().forEach(track => {
          peerConnection.addTrack(track, localStream);
      });
      
      return peerConnection
  }, 

  MakeOffer: function(id) {
      var peerConnection = CreatePeerConnection(id)
      Data.peerConnections[id] = peerConnection
      peerConnection.peerId = id

      peerConnection.createOffer({offerToReceiveAudio: true, offerToReceiveVideo: true})
        .then(function(offer) {
            peerConnection.setLocalDescription(offer)
              .then(function() {
                var offerData = {"offer": offer, "peerId": id}
                unityInstance.SendMessage("WebRTC", "SendOffer", JSON.stringify(offerData));
              });
        });
  },

  MakeAnswer: function(jsonData) {
      const data = JSON.parse(Pointer_stringify(jsonData));

      var peerConnection = CreatePeerConnection(data.peerId)
      // Sender Id (Who we are sending the answer to
      peerConnection.setRemoteDescription(new RTCSessionDescription(data.offer))
        .then(function() {
          peerConnection.createAnswer()
            .then(function(answer) {
              peerConnection.setLocalDescription(answer)
                .then(function() {
                  var answerData = {"peerId": data.peerId, "answer": answer}
                  unityInstance.SendMessage("WebRTC", "SendAnswer", JSON.stringify(answerData));
                });
              });
          });
  },

  ApplyAnswer: function(jsonData) {
      const data = JSON.parse(Pointer_stringify(jsonData));
      var peerConnection = Data.peerConnections[data.peerId]

      const answer = new RTCSessionDescription({type: "answer", sdp: Pointer_stringify(sdp)});
      peerConnection.setRemoteDescription(data.answer).then(function() {
        console.log("Handle answer complete");
      });
  },

  ApplyIceCandidate: function(candidateData) {
      const data = JSON.parse(Pointer_stringify(candidateData));
      Data.peerConnections[data.peerId].addIceCandidate(data.candidate).then(function() {
        console.log(data.candidate);
      });
  },

  HelloString: function (str) {
    window.alert(Pointer_stringify(str));
  },

};

autoAddDeps(WebRTCPlugin, '$Data');
autoAddDeps(WebRTCPlugin, 'CreatePeerConnection');
mergeInto(LibraryManager.library, WebRTCPlugin);
