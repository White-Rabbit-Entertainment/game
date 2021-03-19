using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class MealUI : MonoBehaviourPunCallbacks
{   
    public Text playerListText;

    private List<string> playerList;

    [SerializeField]
    private Text header;

    [SerializeField]
    private Button[] buttons;

    private PlayableCharacter[] playableCharacters;

    private bool swapped;


    // Start is called before the first frame update
    void Start()
    {
        swapped = false;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {               
        playableCharacters = FindObjectsOfType<PlayableCharacter>();
        InitializeButtons();
        PlayableCharacter me = NetworkManager.instance.GetMe();
        PresentMenu(); 
    }

    void InitializeButtons() 
    {
         List<Player> players  = NetworkManager.instance.GetPlayers(); 
         int count = players.Count;
        //  Debug.Log(players.Count);
            //  Button button = buttons[i];
            //  int buttonIndex = i;
            //  Debug.Log(i);
            //  Debug.Log(players[i].NickName);
            if (count > 0) buttons[0].onClick.AddListener(() => SwapMeal(NetworkManager.instance.GetPlayers()[0]));
            if (count > 1) buttons[1].onClick.AddListener(() => SwapMeal(NetworkManager.instance.GetPlayers()[1]));
            if (count > 2) buttons[2].onClick.AddListener(() => SwapMeal(NetworkManager.instance.GetPlayers()[2]));
            if (count > 3) buttons[3].onClick.AddListener(() => SwapMeal(NetworkManager.instance.GetPlayers()[3]));
            if (count > 4) buttons[4].onClick.AddListener(() => SwapMeal(NetworkManager.instance.GetPlayers()[4]));
            if (count > 5) buttons[5].onClick.AddListener(() => SwapMeal(NetworkManager.instance.GetPlayers()[5]));
            if (count > 6) buttons[6].onClick.AddListener(() => SwapMeal(NetworkManager.instance.GetPlayers()[6]));
            if (count > 7) buttons[7].onClick.AddListener(() => SwapMeal(NetworkManager.instance.GetPlayers()[7]));
            if (count > 8) buttons[8].onClick.AddListener(() => SwapMeal(NetworkManager.instance.GetPlayers()[8]));
            if (count > 9) buttons[9].onClick.AddListener(() => SwapMeal(NetworkManager.instance.GetPlayers()[9]));


    }

    void PresentMenu()
    {
        header.text = "Who's meal would you like to swap with " + NetworkManager.instance.GetRoomProperty<string>("CurrentPlayer") + " ? (If not " + NetworkManager.instance.GetRoomProperty<string>("CurrentPlayer") + " buttons don't do anything.)";
        List<Player> players  = NetworkManager.instance.GetPlayers();
        PlayableCharacter me = NetworkManager.instance.GetMe();
        for (int i = 0; i < 10; i++) {
            if (i >= NetworkManager.instance.GetPlayers().Count)
            {   
                buttons[i].gameObject.SetActive(false);
            }
            else {
                buttons[i].gameObject.SetActive(true);
                buttons[i].GetComponentInChildren<Text>().text = players[i].NickName;
            }
        }        
    }

    void SwapMeal(Player p) {
        PlayableCharacter me = NetworkManager.instance.GetMe();
        foreach (PlayableCharacter pc in playableCharacters) {
            if (pc.owner == p) {
                me.SwapMeal(pc);
                NetworkManager.instance.SetRoomProperty("CurrentPlayerGuessed", true); 
                // Debug.Log(pc.owner.NickName);
            }
        }
            Debug.Log(p.NickName);
    }
}
