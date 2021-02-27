using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour {

    public GameObject seekerPrefab;
    public GameObject robberPrefab;
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
        // Load in the local player 
        if (NetworkManager.instance.LocalPlayerPropertyIs<string>("Team", "Seeker")) {
            PhotonNetwork.Instantiate(seekerPrefab.name, new Vector3(1,2,-10), Quaternion.identity);
        } else if (NetworkManager.instance.LocalPlayerPropertyIs<string>("Team", "Robber")) {
            PhotonNetwork.Instantiate(robberPrefab.name, new Vector3(1,2,10), Quaternion.identity);
        }
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
