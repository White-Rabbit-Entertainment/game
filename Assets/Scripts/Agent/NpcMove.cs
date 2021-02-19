using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ObjectRandomizer.cs;

public class NpcMove : MonoBehaviour
{
    [SerializeField]
    Transform _destination;
    NavMeshAgent _navMeshAgent;
    public float wanderRadius = 30f;
    public int walk = 1;
    public int idle = 0;
    // public int current_animation_state;
    public string current_animation_state;
    public int frames = 0;
    public Vector3 targetVector;
    List<string> animationConditions = new List<string>() {
      "walk","idle"
    };
    // // public var animation_conditions = new List<string>();
    // animation_conditions.Add("walk");
    // animation_conditions.Add("idle");
    // animation_conditions.Add("run");
    public float speed = 30f;
    Animator animator;
    private List<GameObject> spawnedPeople;
    public int TotalNumberOfPeopleToSpawn;
    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        current_animation_state = "idle";
        spawnedPeople = new List<GameObject>();

        SpawnAllPeople();
        // current_animation_state = idle;
    }

    void Update()
    {
      frames++;
      if(frames % 30 == 0 || transform.position == targetVector)
      {
        string anim_cond = get_animation_condition();
        //animator.ResetTrigger(current)
        //animator.SetTrigger(anim_cond)
        if(anim_cond != current_animation_state){
          animator.ResetTrigger(current_animation_state);
          animator.SetTrigger(anim_cond);
          current_animation_state = anim_cond;
          // animator.SetInteger("condition",current_animation_state);
        }
      }
      SetDestination(current_animation_state);
    }

    // Update is called once per frame
    private void SetDestination(string anim_cond)
    {
        if(_destination != null){
          if(anim_cond == "walk"){
            // System.Console.WriteLine("walking...");
            targetVector = RandomWanderPoint();
          }
          // Vector3 targetVector = _destination.transform.position;
          else{
            targetVector = transform.position;
          };
          _navMeshAgent.speed = speed;
          _navMeshAgent.SetDestination(targetVector);
        }
    }

    public string get_animation_condition(){
      // return animation_conditions.PickRandom();
      System.Random r = new System.Random(System.Guid.NewGuid().GetHashCode());
      int randNum = r.Next(animationConditions.Count);
      return (string)animationConditions[randNum];
    }

    // public int get_animation_condition(){
    //   // return animation_conditions.PickRandom();
    //   System.Random r = new System.Random(System.Guid.NewGuid().GetHashCode());
    //   int randNum = r.Next(0,2);
    //   return randNum;
    // }

    public Vector3 RandomWanderPoint()
    {
      Vector3 randomPoint = (Random.insideUnitSphere * wanderRadius) + transform.position;
      NavMeshHit navHit;
      NavMesh.SamplePosition(randomPoint,out navHit,wanderRadius,-1);
      return new Vector3(navHit.position.x,transform.position.y,navHit.position.z);
    }


    private void SpawnAllPeople()
    {
      ObjectRandomizer objectRandomizer = GetComponent<ObjectRandomizer>();

        // spawn all people
      for (int personIndex = 0; personIndex < TotalNumberOfPeopleToSpawn; personIndex += 1)
      {
            // get a game location on the board
            Vector3 randomBoardLocation = GetRandomGameBoardLocation();

            // spawn a random person prefab at that location
            GameObject spawnedPerson = SpawnPersonAtLocation(randomBoardLocation, objectRandomizer);

            //Debug.Log("Spawned " + spawnedPerson.name + " at " + randomBoardLocation);

            // add the spawned person to our collection
            spawnedPeople.Add(spawnedPerson);
        }
      }


    private GameObject SpawnPersonAtLocation(Vector3 spawnPosition, ObjectRandomizer objectRandomizer)
    {
        // get a random person prefab to spawn
        GameObject randomPersonPrefab = (GameObject)objectRandomizer.RandomObject();

        // spawn the person at the current spawn point
        GameObject personGameObject = Instantiate(randomPersonPrefab, spawnPosition, Quaternion.identity);

        Vector3 randomDestination = GetRandomGameBoardLocation();
        Person person = personGameObject.GetComponent<Person>();
        person.CurrentDestination = randomDestination;

        Debug.Log("Assigned CurrentDestination to " + person.name);

        //Debug.Log("Random dest: " + randomDestination);

        personGameObject.name = person.name + "_" + spawnPosition.x + "_" + spawnPosition.y + "_" + spawnPosition.z;
        return personGameObject;
    }


    private Vector3 GetRandomGameBoardLocation()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        int maxIndices = navMeshData.indices.Length - 3;

        // pick the first indice of a random triangle in the nav mesh
        int firstVertexSelected = UnityEngine.Random.Range(0, maxIndices);
        int secondVertexSelected = UnityEngine.Random.Range(0, maxIndices);

        // spawn on verticies
        Vector3 point = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];

        Vector3 firstVertexPosition = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];
        Vector3 secondVertexPosition = navMeshData.vertices[navMeshData.indices[secondVertexSelected]];

        // eliminate points that share a similar X or Z position to stop spawining in square grid line formations
        if ((int)firstVertexPosition.x == (int)secondVertexPosition.x || (int)firstVertexPosition.z == (int)secondVertexPosition.z)
        {
            point = GetRandomGameBoardLocation(); // re-roll a position - I'm not happy with this recursion it could be better
        }
        else
        {
            // select a random point on it
            point = Vector3.Lerp(firstVertexPosition, secondVertexPosition, UnityEngine.Random.Range(0.05f, 0.95f));
        }

        return point;
    }
}
