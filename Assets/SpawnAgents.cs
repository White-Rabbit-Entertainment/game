using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnAgents : MonoBehaviour
{
    [SerializeField]
    Transform destination;
    NavMeshAgent navMeshAgent;
    GameObject agent;

    // Start is called before the first frame update
    void Start()
    {
      navMeshAgent = this.GetComponent<NavMeshAgent>();
      Instantiate(agent, transform.position);
    }

    public Vector3 RandomPoint()
    {
      Vector3 newSpawnPoint;
      Vector3 random = System.Random.
      newSpawnPoint = new Vector3 (random.x,0,random.z)
      Vector3 point1 = navMesh.vertices[System.Random.Range(0,navMesh.vertexCount)];
      Vector3 point2 = navMesh.vertices[System.Random.Range(0, navMesh.vertexCount)];
      return Vector3.Lerp(point1, point2, System.Random.value);
   }
}
