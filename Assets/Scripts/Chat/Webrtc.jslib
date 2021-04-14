var WebRTCPlugin = {

  $Data: {
    peerConnection: null,
    localStream: null,
    remoteStream: null,
  },
  
  Init: function() {
    const constraints = {'video': true, 'audio': true},
    const configuration = {'iceServers': [{'urls': 'stun:stun.l.google.com:19302'}]},

    // Setup remote stream
    Data.remoteStream = new MediaStream();
    // const remoteVideo = document.querySelector('#remoteVideo');
    // Data.remoteVideo.srcObject = remoteStream;

    // Setup peer connection
    Data.peerConnection = new RTCPeerConnection(configuration);
    Data.peerConnection.ontrack = () => Data.remoteStream.addTrack(event.track, Data.remoteStream)
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
        if (Data.peerConnection.connectionState === 'connected') {
            // Peers connected!
            console.log("CONNECTED!!!!!!")
        }
    };

    // SetupLocalStream();
  }

  Hello: function () {
    window.alert("Hello, world!");
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
