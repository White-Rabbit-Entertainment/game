using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnAgents : MonoBehaviour
{
    public Enemy agent;
    private List<Enemy> agents;

    [Range (0,100)]
    public int numberOfAgents = 6;
    private float range = 70.0f;

    private NetworkManager networkManager;
    private GameManager gameManager;
    private string gameScene = "GameScene";


    void Start()
    {
        networkManager = new NetworkManager();
        gameManager = new GameManager();
        agents = new List<Enemy>(); // init as type
        for (int index = 0; index < numberOfAgents; index++)
        {
            Enemy spawned = Instantiate(agent, RandomNavmeshLocation(range), Quaternion.identity) as Enemy;
            agents.Add(spawned);
        }
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
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
