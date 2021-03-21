using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class MealSceneManager: MonoBehaviourPunCallbacks {   

    public GameObject buttonPrefab;
    public GameObject buttonsGO;
    public GameObject playerInfoPrefab;
    public GameObject playerInfoGO;

    private List<PlayableCharacter> characters;

    [SerializeField]
    private Text header;

    private List<Player> playersLeft;

    private bool isMyTurn;
    private bool initialized = false;
    private bool started = false;

    private int numberOfPlayers = 0;
    

    public bool HasTurnTimeRemaining() {
        return NetworkManager.instance.GetTimeRemaining(Timer.TurnTimer) > 0;
    }

    // Called once all playable characters have spawned 
    void Init() {
        initialized = true;
        NetworkManager.instance.SetLocalPlayerProperty("MealSceneInitalized", true);
        Cursor.lockState = CursorLockMode.None;
        DrawButtons();
        DrawPlayerInfo();
    }

    [PunRPC]
    void EndMealScene() {
        NetworkManager.instance.SetLocalPlayerProperty("MealSceneInitalized", false);
        PlayableCharacter me = NetworkManager.instance.GetMe();
        if (me.GetMeal().isPoisoned) {
            me.Kill();
        }
        NetworkManager.instance.SetLocalPlayerProperty("Spawned", false);
        NetworkManager.instance.ChangeScene("GameScene");
    }

    [PunRPC]
    // This is the master clients function inorder to setup the next turn
    void InitNextTurn() {
        if (playersLeft.Count == 0) {
            // Switch back to gamescene
            NetworkManager.instance.SetRoomProperty("CurrentScene", "GameScene");
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

    PlayableCharacter GetPlayerCharacter(Player player) {
        foreach (PlayableCharacter character in characters) {
            if (character.GetComponent<PhotonView>().Owner == player) {
                return character;
            }
        }
        return characters[0];
    }

    Player FindCaptainPlayer() {
        foreach (Player player in playersLeft) {
            if (GetPlayerCharacter(player) is Captain) {
                return player;
            }
        }
        return playersLeft[0];
    }
    
    // 
    public bool AllPlayableCharactersSpawned() {
        foreach(PlayableCharacter character in characters) {
            if (!character.Spawned()) {
                return false;
            }
        }
        return true;
    }

    // Update is called once per frame
    void Update() {               
        if (!initialized) {
            characters = new List<PlayableCharacter>(FindObjectsOfType<PlayableCharacter>());

            if (characters.Count == NetworkManager.instance.GetPlayers().Count && AllPlayableCharactersSpawned()) {
                Debug.Log("Inited game");
                Debug.Log($"Found {characters.Count} characters");
                Init();
            }
        }
        
        if (!started && NetworkManager.instance.CheckAllPlayers<bool>("MealSceneInitalized", true)) {
            Debug.Log("Started game");
            // Set up the scene
            if (PhotonNetwork.LocalPlayer.IsMasterClient) {
              playersLeft = NetworkManager.instance.GetPlayers();
              playersLeft.Shuffle();

              // Move capatin to the end of the list (ie go last)
              Player captainPlayer = FindCaptainPlayer(); 
              playersLeft.Remove(captainPlayer);
              playersLeft.Add(captainPlayer);

              InitNextTurn();
            }
            started = true;
        }

        if (isMyTurn && !HasTurnTimeRemaining()) {
            EndTurn();
        }
    }
    
    [PunRPC]
    void DrawPlayerInfo() {
        playerInfoGO.DestroyChildren();
        foreach (PlayableCharacter character in characters) {
            if (!(character is Ghost)) {
                GameObject playerInfo = Instantiate(playerInfoPrefab, playerInfoGO.transform);
                Image[] images = playerInfo.GetComponentsInChildren<Image>();
                Debug.Log(character.roleInfo);
                Debug.Log(character.roleInfo.colour);
                images[0].color = character.roleInfo.colour;
                images[1].color = character.GetMeal().colour;
                Text text = playerInfo.GetComponentInChildren<Text>();
                text.text = $"{character.Owner.NickName} ({character.roleInfo.name})";
            }
        }
    }

    [PunRPC]
    void DrawButtons() {
        buttonsGO.DestroyChildren();
        foreach (PlayableCharacter character in characters) {
            if (!(character is Ghost)) {
                Button button = Instantiate(buttonPrefab, buttonsGO.transform).GetComponent<Button>();
                button.onClick.AddListener(() => SwapMeal(character));
                button.GetComponentInChildren<Text>().text = character.Owner.NickName;
                button.GetComponent<Image>().color = character.GetMeal().colour;
            }
        }
    }
    
    void DisableMenu() {
        header.text = "Not your turn";
        buttonsGO.SetActive(false);
    }

    void PresentMenu() {
        header.text = $"Who's meal would you like to swap with {PhotonNetwork.LocalPlayer.NickName}?";
        buttonsGO.SetActive(true);
    }

    void SwapMeal(PlayableCharacter player) {
        PlayableCharacter me = NetworkManager.instance.GetMe();
        me.SwapMeal(player);
        GetComponent<PhotonView>().RPC("DrawButtons", RpcTarget.All);
        GetComponent<PhotonView>().RPC("DrawPlayerInfo", RpcTarget.All);
        EndTurn();
    }
}
