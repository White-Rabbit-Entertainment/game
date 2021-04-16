using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathCreator : MonoBehaviour {
    private LineRenderer lineRenderer;
    private List<Vector3> points = new List<Vector3>();
    public Action<IEnumerable<Vector3>> OnNewPathCreated = delegate{};
    public Target target;
    private void Awake(){
        lineRenderer = GetComponent<LineRenderer>();
        target = GetComponent<Target>();
    }
    private void Update(){
        Ray ray = Camera.main.ScreenPointToRay(target.transform.position);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray,out hitInfo)){
            points.Add(hitInfo.point);
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }

    } 
}