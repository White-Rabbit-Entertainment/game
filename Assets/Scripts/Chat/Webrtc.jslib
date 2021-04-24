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



};

autoAddDeps(WebRTCPlugin, '$Data');
autoAddDeps(WebRTCPlugin, 'CreatePeerConnection');
mergeInto(LibraryManager.library, WebRTCPlugin);
