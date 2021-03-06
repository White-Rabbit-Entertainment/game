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

    public void GetConnected() {
        Debug.Log("connecting...");
    }
    
    public void SendMsg() {
        client.PublishMessage(worldchat, msgInput.text);
        msgInput.Clear();
    }
     
    public void OnConnected() {
        Debug.Log("Connected!");
        client.Subscribe(new string[] { worldchat}); 
        client.SetOnlineStatus(ChatUserStatus.Online);
    }
   
    public void OnDisconnected()
    {
        Debug.Log("quit");
    }
   
    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("statue：" + state);
    }
   
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for(int i = 0; i< senders.Length;i++){
          GameObject item = Instantiate(chatMessagePrefab, chatArea.transform);
          Text text = item.GetComponentInChildren<Text>();
          text.text = senders[i] + ":" + messages[i] + "\n";
        }
        Debug.Log("channel："+channelName+",sender："+senders[0]+", messages："+messages[0]);
    }
 
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log("channel：" + channelName + ",sender" + sender + ", messages：" + message);
    }
 
 
    public void OnSubscribed(string[] channels, bool[] results)
    {
        foreach(var channel in channels){
            this.client.PublishMessage(channel,"joined");
        }
      
        Debug.Log("channel" + channels[0] + "result：" + results[0]);
    }
 
 
    public void OnUnsubscribed(string[] channels)
    {
        Debug.Log(channels[0] + "fail to join in ");
    }
 
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
 
    }
 
    public void Test()
    {
        client.AddFriends(new string[] { "fu", "zhu" }); 
        client.SetOnlineStatus(1); 
    }
 
 
    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {
 
    }
 
    public void OnUserSubscribed(string channel, string user)
    {
        Debug.LogFormat("OnUserSubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.LogFormat("OnUserUnsubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
    }

    }

