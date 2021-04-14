using System;
using System.Collections.Generic;
using System.Linq;
// using Utils;


// public class PathCreator : MonoBehaviour {
//     private LineRenderer lineRenderer;
//     private List<Vector3> points = new List<Vector3>();
//     public Action<IEnumerable<Vector3>> OnNewPathCreated = delegate{};
//     public Target target;
//     private void Awake(){
//         lineRenderer = GetComponent<LineRenderer>();
//         target = GetComponent<Target>();
//     }
//     private void Update(){
//         Ray ray = Camera.main.ScreenPointToRay(target.transform.position);
//         RaycastHit hitInfo;
//         if (Physics.Raycast(ray,out hitInfo)){
//             points.Add(hitInfo.point);
//             lineRenderer.positionCount = points.Count;
//             lineRenderer.SetPositions(points.ToArray());
//         }

//     } 
// }

using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
public class ShowGoldenPath : MonoBehaviourPun
{
    // private Vector3 targetPosition;

    // public Task task;
    private NavMeshPath path;
    private float elapsed = 0.0f;
    private LineRenderer lineRenderer;

     [System.Serializable]
    public struct SerializedVector3
    {
     public float x;
     public float y;
     public float z;
    }

    void Awake() {
        // If the player is not me (ie not some other player on the network)
        // then destory this script
        lineRenderer = GetComponent<LineRenderer>();
        if (photonView != null && !photonView.IsMine) {
            Destroy(this);
        }
        
        // Dont destory a player on scene change
        // DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        path = new NavMeshPath();
        elapsed = 0.0f;
        lineRenderer.material = new Material (Shader.Find ("Custom/Line"));
        lineRenderer.SetWidth(0.1f,0.1f);
        lineRenderer.material.color = Color.green; 
    }    
    void Update()
    {
        // Update the way to the goal every second.
        elapsed += Time.deltaTime;
        if (elapsed > 1.0f)
        {
            elapsed -= 1.0f;
            NavMesh.CalculatePath(transform.position, NetworkManager.instance.GetMe().assignedSubTask.transform.position, NavMesh.AllAreas, path);
            // NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
        }
        lineRenderer.positionCount = path.corners.Length;
        int distance = (int)GetPathLength(path);
        // Vector3[] vPath = PathToVector3(path);
        // var distance = Vector3.Distance(vPath.First(), vPath.Last());
        lineRenderer.materials[0].mainTextureScale = new Vector3(distance, 1, 1);
        lineRenderer.SetPositions(PathToVector3(path));
        // for (int i = 0; i < path.corners.Length - 1; i++)
        //     Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.green);
    }

    public static float GetPathLength( NavMeshPath path )
    {
        float lng = 0.0f;
       
        if (( path.status != NavMeshPathStatus.PathInvalid ))
        {
            for ( int i = 1; i < path.corners.Length; ++i )
            {
                lng += Vector3.Distance( path.corners[i-1], path.corners[i] );
            }
        }
       
        return lng;
    }
    public static Vector3[] PathToVector3(NavMeshPath path)
    {
        //  SerializedVector3[] pathCorners = new SerializedVector3[path.corners.Length];
        Vector3[] pathCorners = new Vector3[path.corners.Length];
         for (int i = 0; i < path.corners.Length; i++)
         { pathCorners[i] = DeserializeVector3(SerializeVector3(path.corners[i])); }
         return pathCorners;
    }

    public static NavMeshPath DeserializeNavPath(SerializedVector3[] sPathCorners)
     {
         NavMeshPath path = new NavMeshPath();
         Vector3[] pathCorners = new Vector3[sPathCorners.Length];
 
         for (int i = 0; i < pathCorners.Length; i++)
         { pathCorners[i] = DeserializeVector3(sPathCorners[i]); }
             
         path.GetCornersNonAlloc(pathCorners);
         return path;
     }

    public static SerializedVector3 SerializeVector3(Vector3 v3)
    {
     SerializedVector3 sv3;
     sv3.x = v3.x;
     sv3.y = v3.y;
     sv3.z = v3.z;
     return sv3;
    }

     public static Vector3 DeserializeVector3(SerializedVector3 sv3)
    {
     Vector3 v3;
     v3.x = sv3.x;
     v3.y = sv3.y;
     v3.z = sv3.z;
     return v3;
    }
}