using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour {

    public GameObject traitorPrefab;
    public GameObject loyalPrefab;
    public GameObject captainPrefab;
    public InventoryUI inventoryUI;
    public GameObject agentPrefab;
    public GameObject interactablesGameObject;
    public int numberOfAgentsPerPlayer = 3;

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
        NetworkManager.instance.SetLocalPlayerProperty("InGameScene", true);
    }

    // Spawn in a player prefab (of the correct team) for the local players.
    // Each player runs this once all the players are in the scene.
    // TODO Potentially add mutliple spawn points, atm players are just spawned in at a set
    // location.
    void LoadAgents() {
        // Load in the Agents
        for(int i = 0; i < numberOfAgentsPerPlayer; i++){
          GameObject agent = PhotonNetwork.Instantiate(agentPrefab.name, RandomNavmeshLocation(30f), Quaternion.identity);
          agent.GetComponent<AgentController>().interactablesGameObject = interactablesGameObject;
        }
    }

    void LoadPlayer() { 
        GameObject player;
        Team team = NetworkManager.instance.GetLocalPlayerProperty<Team>("Team");
 
        // Load in the local player 
        if (NetworkManager.instance.LocalPlayerPropertyIs<Team>("Team", Team.Traitor)) {
            player = PhotonNetwork.Instantiate(traitorPrefab.name, new Vector3(1,2,-10), Quaternion.identity);
        } else if (NetworkManager.instance.LocalPlayerPropertyIs<Team>("Team", Team.Loyal)) {
            player = PhotonNetwork.Instantiate(loyalPrefab.name, new Vector3(1,2,10), Quaternion.identity);
        } else {
            player = PhotonNetwork.Instantiate(captainPrefab.name, new Vector3(1,2,10), Quaternion.identity);
        }
        PhotonView playerView = player.GetComponent<PhotonView>();
        playerView.RPC("AssignColour", RpcTarget.All, Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        player.GetComponent<Character>().inventoryUI = inventoryUI;
        //sets player layer to "raycast ignore" layer
        player.layer = 2;
    }

    void Update() {
        // Wait till all players are in the scene.
        if (NetworkManager.instance.AllPlayersInGame()) {

          // Then load in all the players
          LoadPlayer();
          LoadAgents();

          // Then this script has done its job (loaded in the player) so we can
          // destory it.
          Destroy(this);
          Destroy(gameObject);
        }
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
