using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcMove : MonoBehaviour
{
    [SerializeField]
    Transform _destination;
    NavMeshAgent _navMeshAgent;
    public float wanderRadius = 30f;
    public float speed = 30f;
    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();
        // if(_navMeshAgent != null){
        //   SetDestination();
        // }
    }

    void Update()
    {
      SetDestination();
    }

    // Update is called once per frame
    private void SetDestination()
    {
        if(_destination != null){
          // Vector3 targetVector = _destination.transform.position;
          Vector3 targetVector = RandomWanderPoint();
          _navMeshAgent.speed = speed;
          _navMeshAgent.SetDestination(targetVector);
        }
    }

    public Vector3 RandomWanderPoint()
    {
      Vector3 randomPoint = (Random.insideUnitSphere * wanderRadius) + transform.position;
      NavMeshHit navHit;
      NavMesh.SamplePosition(randomPoint,out navHit,wanderRadius,-1);
      return new Vector3(navHit.position.x,transform.position.y,navHit.position.z);
    }
}
