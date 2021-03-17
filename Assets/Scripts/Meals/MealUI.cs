using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class MealUI : MonoBehaviourPun
{   
    public Text playerListText;

    private List<string> playerList;

    [SerializeField]
    private Text header;

    [SerializeField]
    private Button[] buttons;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {   
        // playerList = "";
        // players = NetworkManager.instance.GetPlayers();
        // playerList = new List<string>();
        // foreach (Player p in NetworkManager.instance.GetPlayers()) {
        //     playerList.Add(p.NickName);
        // }
        
        // string playerListString = System.String.Empty;
        // for (int i = 0; i < playerList.Count; i++) {
        //     playerListString = playerListString + " " + playerList[0];
        // }
        // playerListText.text = playerListString;
        PresentMenu();
    }

    void InitializeButtons() 
    {
        
    }

    void PresentMenu()
    {
        header.text = "Who's meal would you like to swap with?";

        for (int i = 0; i < 10; i++) {
            if (i >= NetworkManager.instance.GetPlayers().Count)
            {
                buttons[i].gameObject.SetActive(false);
            }
            else {
                buttons[i].gameObject.SetActive(true);
                buttons[i].GetComponentInChildren<Text>().text = NetworkManager.instance.GetRoomProperty<string>("CurrentPlayer");
            }
        }        
    }
}
