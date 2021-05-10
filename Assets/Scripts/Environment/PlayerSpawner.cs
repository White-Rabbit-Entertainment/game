using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour {

    public GameObject traitorPrefab;
    public GameObject ghostPrefab;
    public GameObject loyalPrefab;
    public InventoryUI inventoryUI;
    public DeathUI deathUI;
    public CurrentTaskUI currentTaskUI;
    public GameSceneManager gameSceneManager;
    public TaskNotificationUI taskNotificationUI;
    public GameObject interactablesGameObject;
    public GameObject offScreenIndicator; 

    public List<string> traitors; 

    public PlayerInfo playerInfo;

    void Update() {
        // Wait till all players are in the scene.
        if (NetworkManager.instance.CheckAllPlayers<string>("CurrentScene", SceneManager.GetActiveScene().name)) {

            // Then load in all the players
            LoadPlayer();

            NetworkManager.instance.SetLocalPlayerProperty("Spawned", true);
            // Then this script has done its job (loaded in the player) so we can
            // destory it.
            Destroy(this);
            Destroy(gameObject);
        }
    }

    void OnEnable() {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a
        //scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable() {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a
        //scene change as soon as this script is disabled. Remember to always
        //have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    // This set the player as "InGameScene" so that we can wait till all the
    // players are in the scene before spawninng any objects.
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        // Set meal scene to true when swtiching to meal scene
        NetworkManager.instance.SetLocalPlayerProperty("CurrentScene", scene.name);
    }

    void LoadPlayer() {
        GameObject player;
        Team team = NetworkManager.instance.GetLocalPlayerProperty<Team>("Team");

        Vector3 spawnPoint;
        GameObject playerPrefab;
        // Load in the local player
        if (NetworkManager.instance.LocalPlayerPropertyIs<Team>("Team", Team.Traitor)) {
            // spawnPoint = new Vector3(1,2,-10);
            playerPrefab = traitorPrefab;
            traitors.Add(PhotonNetwork.LocalPlayer.NickName);
        } else if (NetworkManager.instance.LocalPlayerPropertyIs<Team>("Team", Team.Loyal)) {
            // spawnPoint = new Vector3(1,2,10);
            playerPrefab = loyalPrefab;
        } else {
            // spawnPoint = new Vector3(1,4,10);
            playerPrefab = ghostPrefab;
        }
        spawnPoint = gameSceneManager.RandomNavmeshLocation();
        // Spawn in the player at the spawn point
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity);

        // Grab some useful components
        PlayableCharacter character = player.GetComponent<PlayableCharacter>();
        PhotonView playerView = player.GetComponent<PhotonView>();

        // Assign a role
        playerView.RPC("AssignColor", RpcTarget.All, NetworkManager.instance.GetLocalPlayerProperty<string>("Color"));

        // Set the inventoryUI
        character.inventoryUI = inventoryUI;
        character.currentTaskUI = currentTaskUI;
        character.taskNotificationUI = taskNotificationUI;
        character.deathUI = deathUI;
        NetworkManager.myCharacter = character;
       
        //sets player layer to "raycast ignore" layer
        player.SetLayerRecursively(2);
   
        // Set up the camera for offScreenIndicator
        offScreenIndicator.SetActive(true);

    }
}
