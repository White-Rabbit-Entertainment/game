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
    public int numberOfAgents = 10;

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
    void LoadPlayer() {
        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
          // PhotonNetwork.Instantiate(agentPrefab.name, new Vector3(1,2,-10), Quaternion.identity);
          for(int i = 0; i < numberOfAgents; i++){
            PhotonNetwork.Instantiate(agentPrefab.name, RandomNavmeshLocation(30f), Quaternion.identity);
            Debug.Log("Instantiated agent");
          }

        }
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
//
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.AI;
// using Photon.Pun;
// using Photon.Realtime;
// using System;
// using Hashtable = ExitGames.Client.Photon.Hashtable;
//
//
// public class SpawnAgents : MonoBehaviour
// {
//     public GameObject agent;
//     // private List<GameObject> agents;
//
//     [Range (0,100)]
//     public int numberOfAgents = 6;
//     private float range = 10.0f;
//
//     private NetworkManager networkManager;
//     private GameManager gameManager;
//     private string gameScene = "GameScene";
//
//     public Text agentList;
//
//
//     void Start()
//     {
//         networkManager = new NetworkManager();
//         gameManager = new GameManager();
//         // networkManager.Start();
//         // agents = new List<GameObject>(); // init as type
//         networkManager.ChangeScene(gameScene);
//         // for (int index = 0; index < numberOfAgents; index++)
//         // {
//         //     PhotonNetwork.Instantiate(agent.name, RandomNavmeshLocation(range), Quaternion.identity);
//         //     // spawned;
//         //     // agents.Add(spawned);
//         // }
//
//         PhotonNetwork.Instantiate(agent.name, new Vector3(1,2,-10), Quaternion.identity);
//         agent.SetActive(true);
//         // PhotonNetwork.Instantiate(agent.name, RandomNavmeshLocation(range), Quaternion.identity);
//         gameManager.OnStartGame();
//     }
//
//     // void SetText() {
//     //   agentList.text = networkManager.GetPlayers().Count.ToString();
//     // }
//
//     public Vector3 RandomNavmeshLocation(float radius)
//     {
//         Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
//         randomDirection += transform.position;
//         NavMeshHit hit;
//         Vector3 finalPosition = Vector3.zero;
//         if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
//         {
//             finalPosition = hit.position;
//         }
//         return finalPosition;
//     }
// }
//
