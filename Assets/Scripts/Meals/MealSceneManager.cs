using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class MealSceneManager: MonoBehaviourPunCallbacks {   

    private List<string> playerList;

    public GameObject buttonPrefab;
    public GameObject buttonsGO;

    [SerializeField]
    private Text header;

    private List<Player> playersLeft;

    private bool isMyTurn;
    private bool initalized = false;
    

    public bool HasTurnTimeRemaining() {
        return NetworkManager.instance.GetTimeRemaining(Timer.TurnTimer) > 0;
    }

    // Start is called before the first frame update
    void Init() {
        initalized = true;
        Cursor.lockState = CursorLockMode.None;
        InitializeButtons();
       
        // Set up the scene
        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
          NetworkManager.instance.SetRoomProperty("CurrentScene", "MealScene");
          playersLeft = NetworkManager.instance.GetPlayers();
          InitNextTurn();
        }
    }

    [PunRPC]
    // This is the master clients function inorder to setup the next turn
    void InitNextTurn() {
        if (playersLeft.Count == 0) {
          // Switch back to gamescene
          GameManager.instance.StartGame();
        }
        GetComponent<PhotonView>().RPC("StartTurn", playersLeft[0]);
        playersLeft.RemoveAt(0);
    }

    [PunRPC]
    void StartTurn() {
        // Start round timer
        NetworkManager.instance.StartTimer(20, Timer.TurnTimer);
        PresentMenu(); 
        isMyTurn = true;
    }

    void EndTurn() {
        isMyTurn = false;
        DisableMenu();
        GetComponent<PhotonView>().RPC("InitNextTurn", PhotonNetwork.MasterClient);
    }

    // Update is called once per frame
    void Update() {               
        if (!initalized && NetworkManager.instance.AllCharactersSpawned()) {
            Init();
        }
        if (isMyTurn && !HasTurnTimeRemaining()) {
            EndTurn();
        }
    }

    void InitializeButtons() {
        foreach (PlayableCharacter character in FindObjectsOfType<PlayableCharacter>()) {
            Button button = Instantiate(buttonPrefab, buttonsGO.transform).GetComponent<Button>();
            button.onClick.AddListener(() => SwapMeal(character));
            button.GetComponentInChildren<Text>().text = character.GetComponent<PhotonView>().Owner.NickName;
        }
    }
    
    void DisableMenu() {
        header.text = "Not your turn";
        buttonsGO.SetActive(false);
    }

    void PresentMenu() {
        header.text = "Who's meal would you like to swap with " + NetworkManager.instance.GetRoomProperty<string>("CurrentPlayer") + " ? (If not " + NetworkManager.instance.GetRoomProperty<string>("CurrentPlayer") + " buttons don't do anything.)";

        buttonsGO.SetActive(true);
    }

    void SwapMeal(PlayableCharacter player) {
        PlayableCharacter me = NetworkManager.instance.GetMe();
        me.SwapMeal(player);
        EndTurn();
    }
}
