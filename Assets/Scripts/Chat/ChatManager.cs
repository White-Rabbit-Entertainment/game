using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;     
using Photon.Pun;     
using Photon.Realtime;     

public class ChatManager : MonoBehaviour, IChatClientListener 
{
    private ChatClient Client {
        get {return NetworkManager.chatClient;}
    }     
    private string AppID = "d0a5737b-396c-4990-8cde-19b4eadbd95e";            
    private string AppVersion;       

    [SerializeField] private InputField msgInput;
    [SerializeField] private Button sendButton;

    [SerializeField] private GameObject chatArea;
    [SerializeField] private ScrollRect chatScrollRect;
    [SerializeField] private GameObject chatMessagePrefab;
    
    public void Init() {
        if (NetworkManager.chatClient == null) {
            NetworkManager.chatClient = new ChatClient(this);
            AppVersion = "1.0.0";    
            NetworkManager.chatClient.Connect(AppID, AppVersion, new Photon.Chat.AuthenticationValues(PhotonNetwork.LocalPlayer.NickName));
            sendButton.onClick.AddListener(SendMsg);
            DontDestroyOnLoad(gameObject);
        }
    }
 
    void Update() {
        if (Client != null)    
        {
            Client.Service(); 
        }
    }
    
    public void JoinRoomChat(Room room) {
        Debug.Log("Joining romm chat");
        Client.Subscribe(new string[] { room.Name }); 
        Client.SetOnlineStatus(ChatUserStatus.Online);
    }
    
    public void LeaveRoomChat(Room room) {
        Client.Unsubscribe(new string[] { room.Name }); 
    }

    public void SendMsg() {
        Debug.Log(msgInput.text);
        Client.PublishMessage(PhotonNetwork.CurrentRoom.Name, msgInput.text);
        msgInput.Clear();
    }
     
    public void OnGetMessages(string channelName, string[] senders, object[] messages) {
        Debug.Log($"Got message: {senders[0]}: {messages[0]}");
        for(int i = 0; i< senders.Length;i++){
          GameObject item = Instantiate(chatMessagePrefab, chatArea.transform);
          Text text = item.GetComponentInChildren<Text>();
          text.text = senders[i] + ":" + messages[i] + "\n";
        }
    }
 
    public void OnSubscribed(string[] channels, bool[] results) {
        foreach(var channel in channels){
            Client.PublishMessage(channel,"joined");
        }
    }
 
    public void OnUnsubscribed(string[] channels) {}
    
    public void GetConnected() {}
    
    public void OnConnected() {}
    
    public void OnDisconnected() {}
    
    public void OnChatStateChange(ChatState state) {}
    
    public void OnPrivateMessage(string sender, object message, string channelName) {}
 
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) {} 
 
    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message) {}
 
    public void OnUserSubscribed(string channel, string user) {}

    public void OnUserUnsubscribed(string channel, string user) {}
}

