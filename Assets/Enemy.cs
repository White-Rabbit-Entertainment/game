using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    private NavMeshAgent pathfinder;
    private Transform target;

    void Start()
    {
        pathfinder = this.GetComponent<NavMeshAgent>();
        target = GameObject.Find("Item 1").transform;
    }
    void Update()
    {
        pathfinder.SetDestination(target.position);
    }
}
