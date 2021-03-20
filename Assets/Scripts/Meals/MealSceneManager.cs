﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class MealSceneManager: MonoBehaviourPunCallbacks {   

    private List<string> playerList;

    public GameObject buttonPrefab;
    public GameObject buttonsGO;

    private List<PlayableCharacter> characters;

    [SerializeField]
    private Text header;

    private List<Player> playersLeft;

    private bool isMyTurn;
    private bool initalized = false;
    private bool started = false;

    private int numberOfPlayers = 0;
    

    public bool HasTurnTimeRemaining() {
        return NetworkManager.instance.GetTimeRemaining(Timer.TurnTimer) > 0;
    }

    // Called once all playable characters have spawned 
    void Init() {
        initalized = true;
        NetworkManager.instance.SetLocalPlayerProperty("MealSceneInitalized", true);
        Cursor.lockState = CursorLockMode.None;
        DrawButtons();
    }

    [PunRPC]
    void EndMealScene() {
        GameManager.instance.StartGame();
    }

    [PunRPC]
    // This is the master clients function inorder to setup the next turn
    void InitNextTurn() {
        if (playersLeft.Count == 0) {
            // Switch back to gamescene
            GetComponent<PhotonView>().RPC("EndMealScene", RpcTarget.All);
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

        // Tell the master client to star the next turn
        GetComponent<PhotonView>().RPC("InitNextTurn", PhotonNetwork.MasterClient);
    }

    // Update is called once per frame
    void Update() {               
        characters = new List<PlayableCharacter>(FindObjectsOfType<PlayableCharacter>());
        if (!initalized && characters.Count == NetworkManager.instance.GetPlayers().Count) {
            Debug.Log($"Found {characters.Count} characters");
            Init();
        }
        
        if (!started && NetworkManager.instance.CheckAllPlayers<bool>("MealSceneInitalized", true)) {
            // Set up the scene
            if (PhotonNetwork.LocalPlayer.IsMasterClient) {
              NetworkManager.instance.SetRoomProperty("CurrentScene", "MealScene");
              playersLeft = NetworkManager.instance.GetPlayers();
              InitNextTurn();
            }
            started = true;
        }

        if (isMyTurn && !HasTurnTimeRemaining()) {
            EndTurn();
        }
    }

    [PunRPC]
    void DrawButtons() {
        buttonsGO.DestroyChildren();
        foreach (PlayableCharacter character in characters) {
            if (!(character is Ghost)) {
                Button button = Instantiate(buttonPrefab, buttonsGO.transform).GetComponent<Button>();
                button.onClick.AddListener(() => SwapMeal(character));
                button.GetComponentInChildren<Text>().text = character.GetComponent<PhotonView>().Owner.NickName;
                button.GetComponent<Image>().color = character.GetMeal().colour;
            }
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
        GetComponent<PhotonView>().RPC("DrawButtons", RpcTarget.All);
        EndTurn();
    }
}