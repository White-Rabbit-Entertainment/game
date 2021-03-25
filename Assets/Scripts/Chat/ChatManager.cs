using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;     
using Photon.Pun;     

public class ChatManager : MonoBehaviour, IChatClientListener 
{
    private ChatClient client;       
    private string AppID = "d0a5737b-396c-4990-8cde-19b4eadbd95e";            
    private string AppVersion;       

    public InputField msgInput;

    public GameObject chatArea;
    public ScrollRect chatScrollRect;
    public GameObject chatMessagePrefab;

    private string worldchat = "worldchat";
    
    void Start()
    {
        client = new ChatClient(this);
        AppVersion = "1.0.0";    
        client.Connect(AppID, AppVersion, new Photon.Chat.AuthenticationValues(PhotonNetwork.LocalPlayer.NickName));
    }
 
    void Update()
    {
        if (client != null)    
        {
            client.Service(); 
        }
    }

    public void GetConnected() {}
    
    public void SendMsg() {
        client.PublishMessage(worldchat, msgInput.text);
        msgInput.Clear();
    }
     
    public void OnConnected() {
        client.Subscribe(new string[] { worldchat}); 
        client.SetOnlineStatus(ChatUserStatus.Online);
    }
   
    public void OnDisconnected() {}
   
    public void OnChatStateChange(ChatState state) {}
   
    public void OnGetMessages(string channelName, string[] senders, object[] messages) {
        for(int i = 0; i< senders.Length;i++){
          GameObject item = Instantiate(chatMessagePrefab, chatArea.transform);
          Text text = item.GetComponentInChildren<Text>();
          text.text = senders[i] + ":" + messages[i] + "\n";
        }
    }
 
    public void OnPrivateMessage(string sender, object message, string channelName) {}
 
 
    public void OnSubscribed(string[] channels, bool[] results) {
        foreach(var channel in channels){
            this.client.PublishMessage(channel,"joined");
        }
    }
 
 
    public void OnUnsubscribed(string[] channels) {
        Debug.Log(channels[0] + "fail to join in ");
    }
 
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) {} 
 
    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message) {}
 
    public void OnUserSubscribed(string channel, string user) {}

    public void OnUserUnsubscribed(string channel, string user) {}

}

