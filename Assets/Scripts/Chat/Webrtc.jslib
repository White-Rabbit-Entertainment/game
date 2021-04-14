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
