using UnityEngine;
using UnityEngine.UI;
using Unity.WebRTC;
using System.Collections;

public class Voice: MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private RawImage WebcamImage;

    private RTCPeerConnection localConnection, remoteConnection;
    private RTCDataChannel sendChannel, receiveChannel;
    private MediaStream audioStream, videoStream;

    private void Awake() {
        // Initialize WebRTC
        WebRTC.Initialize();

        SetupPeers();
    }

    private RTCOfferOptions _offerOptions = new RTCOfferOptions {
        iceRestart = false,
        offerToReceiveAudio = true,
        offerToReceiveVideo = true
    };

    private RTCAnswerOptions _answerOptions = new RTCAnswerOptions {
        iceRestart = false,
    };

    private void SetupPeers() {

        // Create local peer
        localConnection = new RTCPeerConnection();
        sendChannel = localConnection.CreateDataChannel("sendChannel");
        sendChannel.OnOpen = HandleSendChannelStatusChange;
        sendChannel.OnClose = HandleSendChannelStatusChange;
        localConnection.OnIceCandidate = e => { 
            if (!string.IsNullOrEmpty(e.Candidate)) {
                remoteConnection.AddIceCandidate(e); 
            }
        };

        localConnection.OnIceConnectionChange = state => {
            Debug.Log($"Icestate update to: {state}");
        };
        localConnection.OnNegotiationNeeded = OnNegotiationNeeded;
        
        // Create remote peer
        remoteConnection = new RTCPeerConnection();
        remoteConnection.OnDataChannel = ReceiveChannelCallback;

        remoteConnection.OnIceCandidate = e => {
            if(!string.IsNullOrEmpty(e.Candidate)) {
                localConnection.AddIceCandidate(e);
            }
        };
        remoteConnection.OnNegotiationNeeded = OnNegotiationNeeded;
    }

    private IEnumerator OnCreateOfferSuccess(RTCPeerConnection pc, RTCSessionDescription desc) {
        Debug.Log("Offer successfully created");
        Debug.Log($"setLocalDescription start");
        var op = pc.SetLocalDescription(ref desc);
        yield return op;

        if (!op.IsError)
        {
            // OnSetLocalSuccess(pc);
        }
        else
        {
            var error = op.Error;
            // OnSetSessionDescriptionError(ref error);
        }

        Debug.Log($"setRemoteDescription start");
        var op2 = localConnection.SetRemoteDescription(ref desc);
        yield return op2;
        if (!op2.IsError)
        {
            // OnSetRemoteSuccess(otherPc);
        }
        else
        {
            var error = op2.Error;
            // OnSetSessionDescriptionError(ref error);
        }
        // Since the 'remote' side has no media stream we need
        // to pass in the right constraints in order for it to
        // accept the incoming offer of audio and video.

        var op3 = remoteConnection.CreateAnswer(ref _answerOptions);
        yield return op3;
        if (!op3.IsError)
        {
            // yield return OnCreateAnswerSuccess(otherPc, op3.Desc);
        }
        else
        {
            OnCreateSessionDescriptionError(op3.Error);
        }
    }

    private static void OnCreateSessionDescriptionError(RTCError error) {
        Debug.LogError($"Error Detail Type: {error.message}");
    }

    private void SetupLocalStream() {
        audioStream = Audio.CaptureStream();
        videoStream = cam.CaptureStream(1280, 720, 1000000);
        WebcamImage.texture = cam.targetTexture;
    }

    private void HandleSendChannelStatusChange() {
        Debug.Log("Send channel status changed!");
    }

    void ReceiveChannelCallback(RTCDataChannel channel) {
        receiveChannel = channel;
        receiveChannel.OnMessage = HandleReceiveMessage;  
    }
    
    void HandleReceiveMessage(byte[] bytes) {
      var message = System.Text.Encoding.UTF8.GetString(bytes);
      Debug.Log(message);
    }

    private void OnDestroy() {
      sendChannel.Close();
      receiveChannel.Close();
    
      localConnection.Close();
      remoteConnection.Close();
    
      WebRTC.Dispose();
    }

    private void OnNegotiationNeeded() {
        StartCoroutine(OnNegotiationNeededCoroutine(localConnection));
    }

    IEnumerator OnNegotiationNeededCoroutine(RTCPeerConnection pc)
    {
        Debug.Log($"createOffer start");
        var op = localConnection.CreateOffer(ref _offerOptions);
        yield return op;

        if (!op.IsError)
        {
            yield return StartCoroutine(OnCreateOfferSuccess(pc, op.Desc));
        }
        else
        {
            OnCreateSessionDescriptionError(op.Error);
        }
    }

    private static RTCConfiguration GetSelectedSdpSemantics() {
        RTCConfiguration config = default;
        config.iceServers = new[]
        {
            new RTCIceServer { urls = new[] { "stun:stun.l.google.com:19302" } }
        };

        return config;
    }
}

