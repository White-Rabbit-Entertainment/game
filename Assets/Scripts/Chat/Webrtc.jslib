var WebRTCPlugin = {

  $Data: {
    peerConnection: null,
    localStream: null,
    remoteStream: null,
  },
  
  Hello: function () {
    window.alert("Hello, world!");
  },
  
  Init: function() {
  
    const constraints = {'video': true, 'audio': true}
    const configuration = {'iceServers': [{'urls': 'stun:stun.l.google.com:19302'}]}

    Data.remoteStream = new MediaStream();
    Data.peerConnection = new RTCPeerConnection(Data.configuration);
    Data.peerConnection.ontrack = function() {
      Data.remoteStream.addTrack(event.track, Data.remoteStream);
    };
    Data.peerConnection.onnegotiationneeded = function(event) {
      console.log("Negotiation needed!");
    };
    Data.peerConnection.onicecandidate = function(event) {
        console.log("icecandidate happened");
        if (event.candidate) {
          console.log("icecandidate really happened");
          var candidateData = JSON.stringify(event.candidate);
          unityInstance.SendMessage("WebRTC", "SendIceCandidate", candidateData);
        }
    };
    Data.peerConnection.onconnectionstatechanged = function(event) {
        // If peerConnection becomes connected
        if (Data.peerConnection.connectionState === 'connected') {
            // Peers connected!
            console.log("CONNECTED!!!!!!")
            unityInstance.SendMessage("WebRTC", "OnConnected");
        }
    };
      
    navigator.mediaDevices.getUserMedia(constraints)
        .then(function(stream) {
            console.log('Got MediaStream:', stream);
            Data.localStream = stream;
            Data.localStream.getTracks().forEach(function(track) {
                console.log("Sending track")
                console.log(track)
                Data.peerConnection.addTrack(track, Data.localStream);
            });
        })
        .catch(function(error) {
            console.error('Error accessing media devices.', error);
        });
  },

  MakeOffer: function() {
      Data.peerConnection.createOffer({offerToReceiveAudio: true, offerToReceiveVideo: true})
        .then(function(offer) {
            Data.peerConnection.setLocalDescription(offer)
              .then(function() {
                unityInstance.SendMessage("WebRTC", "SendOffer", offer.sdp);
                console.log("Making offer")
              });
        });
  },

  MakeAnswer: function(sdp, callerId) {
      Data.peerConnection.setRemoteDescription(new RTCSessionDescription({type: "offer", sdp: sdp}))
        .then(function() {
          Data.peerConnection.createAnswer()
            .then(function(answer) {
              Data.peerConnection.setLocalDescription(answer)
                .then(function() {
                  unityInstance.SendMessage("WebRTC", "SendAnswer", answer.sdp, callerId);
                  console.log("Making answer")
                });
              });
          });
  },

  ApplyAnswer: function(sdp) {
      console.log("Got answer")
      const answer = new RTCSessionDescription({type: "answer", sdp: sdp});
      const remoteDesc = new RTCSessionDescription(answer);
      Data.peerConnection.setRemoteDescription(remoteDesc).then(function() {
        console.log("Handle answer complete");
      });
  },

  ApplyIceCandidate: function(candidateData) {
      var candidate = new RTCIceCandidate(JSON.parse(candidateData));
      try {
        peerConnection.addIceCandidate(candidate).then(function() {
          console.log("Added ice candidate");
        });
      } catch (e) {
        console.error('Error adding received ice candidate', e);
      }
  },

  HelloString: function (str) {
    window.alert(Pointer_stringify(str));
  },

  PrintFloatArray: function (array, size) {
    for(var i = 0; i < size; i++)
    console.log(HEAPF32[(array >> 2) + i]);
  },

  AddNumbers: function (x, y) {
    return x + y;
  },

  StringReturnValueFunction: function () {
    var returnStr = "bla";
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  BindWebGLTexture: function (texture) {
    GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[texture]);
  },

};

autoAddDeps(WebRTCPlugin, '$Data');
mergeInto(LibraryManager.library, WebRTCPlugin);
