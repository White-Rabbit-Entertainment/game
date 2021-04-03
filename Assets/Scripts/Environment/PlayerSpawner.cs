﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour {

    public GameObject traitorPrefab;
    public GameObject ghostPrefab;
    public GameObject loyalPrefab;
    public GameObject captainPrefab;
    public InventoryUI inventoryUI;
    public ContextTaskUI contextTaskUI;
    public TaskNotificationUI taskNotificationUI;
    public GameObject agentPrefab;
    public GameObject interactablesGameObject;
    public int numberOfAgentsPerPlayer = 0;
    public GameObject offScreenIndicator; 

    public string sceneName;

    public List<GameObject> rolesPrefabs;

    public RoleInfo roleInfo;

    void Update() {
        // Wait till all players are in the scene.
        if (NetworkManager.instance.CheckAllPlayers<string>("CurrentScene", SceneManager.GetActiveScene().name)) {

            // Then load in all the players
            LoadPlayer();
            LoadAgents();

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
        NetworkManager.instance.SetLocalPlayerProperty("CurrentScene", SceneManager.GetActiveScene().name);
    }

    // Spawn in a player prefab (of the correct team) for the local players.
    // Each player runs this once all the players are in the scene.
    // TODO Potentially add mutliple spawn points, atm players are just spawned in at a set
    // location.
    void LoadAgents() {
        // Otherwise load in n agents which have the same role as the player
        for(int i = 0; i < numberOfAgentsPerPlayer; i++){
            // Spawn in the agent
            GameObject agent = PhotonNetwork.Instantiate(agentPrefab.name, RandomNavmeshLocation(30f), Quaternion.identity);
            agent.GetComponent<AgentController>().interactablesGameObject = interactablesGameObject;

            // Assign the same role as the player to the agent
            Role role = NetworkManager.instance.GetLocalPlayerProperty<Role>("Role");
            agent.GetComponent<PhotonView>().RPC("AssignRole", RpcTarget.All, role);
        }
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
        } else if (NetworkManager.instance.LocalPlayerPropertyIs<Team>("Team", Team.Loyal)) {
            // spawnPoint = new Vector3(1,2,10);
            playerPrefab = loyalPrefab;
        } else {
            // spawnPoint = new Vector3(1,4,10);
            playerPrefab = ghostPrefab;
        }
        spawnPoint = RandomNavmeshLocation(5f);
        // Spawn in the player at the spawn point
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity);

        // Grab some useful components
        PlayableCharacter character = player.GetComponent<PlayableCharacter>();
        PhotonView playerView = player.GetComponent<PhotonView>();

        // Assign a role
        Role role = NetworkManager.instance.GetLocalPlayerProperty<Role>("Role");
        playerView.RPC("AssignRole", RpcTarget.All, role);

        // Set the inventoryUI
        character.inventoryUI = inventoryUI;
        character.contextTaskUI = contextTaskUI;
        character.taskNotificationUI = taskNotificationUI;
        NetworkManager.myCharacter = character;

        //sets player layer to "raycast ignore" layer
        player.SetLayerRecursively(2);
   
        // Set up the camera for offScreenIndicator
        offScreenIndicator.SetActive(true);
    }



    public Vector3 RandomNavmeshLocation(float radius) {
        // return new Vector3(4, 5, -3);
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
            finalPosition = hit.position;
        }
        return new Vector3 (finalPosition.x,finalPosition.y+3,finalPosition.z);
    }
}
